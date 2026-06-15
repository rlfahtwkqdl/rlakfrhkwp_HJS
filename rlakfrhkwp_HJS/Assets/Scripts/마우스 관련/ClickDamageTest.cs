using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ClickDamageTest : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private GunData gunData;
    [SerializeField] private string teamKillSceneName = "TeamKillEndingScene";

    [Header("시각 효과 (Tracer)")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private LineRenderer tracerEffectPrefab;
    [SerializeField] private float tracerDuration = 0.05f;

    [Header("★ [추가] 적 피격 이펙트 (파티클)")]
    [Tooltip("Enemy 태그를 맞췄을 때 (빠르게 투명해지며 사라지는 이펙트)")]
    [SerializeField] private ParticleSystem bodyHitPrefab;
    [Tooltip("Head 태그를 맞췄을 때 (빨간 작은 입자로 펑 터지는 이펙트)")]
    [SerializeField] private ParticleSystem headHitPrefab;

    [Header("헤드샷 카메라 진동 설정")]
    [Range(0f, 1f)] // 인스펙터에서 슬라이더로 조절할 수 있게 만듭니다.
    [SerializeField] private float shakeDuration = 0.15f; // 진동 시간 (초)

    [Range(0f, 2f)] // 너무 세면 화면이 뒤집히니 최대 2 정도로 제한
    [SerializeField] private float shakeMagnitude = 0.2f; // 진동 세기

    [Header("🔴 [추가] 사운드 설정 (오디오 클립)")]
    [SerializeField] private AudioClip fireSound;       // 총 발사 소리
    [SerializeField] private AudioClip bodyHitSound;    // 몸통 맞은 소리
    [SerializeField] private AudioClip headHitSound;    // 헤드샷 소리

    private AudioSource audioSource; // 소리를 재생할 컴포넌트 변수
    private bool isReloading = false;

    // 🔴 [추가] 시작할 때 AudioSource 컴포넌트를 세팅합니다.
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // 만약 오브젝트에 AudioSource가 없다면 자동으로 추가해 줍니다.
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (isReloading || gunData == null) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            CastRayFromMouse();
        }
    }

    void CastRayFromMouse()
    {
        if (muzzlePoint == null || tracerEffectPrefab == null)
        {
            Debug.LogError("ClickDamageTest: Muzzle Point 또는 Tracer Prefab이 연결되지 않았습니다!");
            return;
        }

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        bool shouldReload = true;

        // [라인 이펙트] 시작점과 끝점 지정
        Vector3 targetPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
        StartCoroutine(SpawnTracer(targetPosition));

        // 🔴 [추가] 발사하는 순간 총소리 재생
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        // 충돌 결과 처리
        if (hit.collider != null)
        {
            // ★ [추가] 실제 총알이 부딪힌 정확한 2D 좌표 구하기
            Vector3 hitPoint = new Vector3(hit.point.x, hit.point.y, -1f);

            // 1. 오인 사격 처리
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("<color=red><b>오인사격! 작전 실패!</b></color>");
                if (ScoreManager.Instance != null) ScoreManager.Instance.CalculateFinalScore();
                SceneManager.LoadScene(teamKillSceneName);
                return;
            }

            // 2. 머리 맞췄을 때 (Head)
            else if (hit.collider.CompareTag("Head"))
            {
                Debug.Log("<color=yellow><b>머리통! 장전 시간 초기화!</b></color>");

                // 🔴 [추가] 헤드샷 사운드 재생
                if (audioSource != null && headHitSound != null)
                {
                    audioSource.PlayOneShot(headHitSound);
                }

                // ★ 안전장치 추가 및 재생 강제화
                if (headHitPrefab != null)
                {
                    ParticleSystem effectInstance = Instantiate(headHitPrefab, hitPoint, Quaternion.identity);
                    effectInstance.Play(); // ◀ 눈 딱 감고 한 번 더 강제로 틀어버리기

                    Debug.Log($"<color=green>[파티클 성공] {effectInstance.name} 오브젝트가 맵에 생성되었습니다!</color>");
                }
                else
                {
                    // 만약 인스펙터 연결이 풀렸다면 콘솔창에 이게 뜹니다.
                    Debug.LogError("[파티클 에러] headHitPrefab이 인스펙터에 연결되지 않았습니다! 확인해보세요.");
                }

                // [변경] 고정 수치(0.15f, 0.2f) 대신 인스펙터에서 유저님이 조절하는 슬라이더 변수값으로 연동 완료!
                if (CameraShake.Instance != null)
                {
                    CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);
                }

                Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
                if (enemy != null) enemy.InstantKill();
                shouldReload = false;
            }

            // 3. 몸통 맞췄을 때 (Enemy)
            else if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("<color=orange>몸 샷</color>");

                // 🔴 [추가] 몸샷 사운드 재생
                if (audioSource != null && bodyHitSound != null)
                {
                    audioSource.PlayOneShot(bodyHitSound);
                }

                // ★ [이펙트 소환] 몸통 피격 이펙트 생성
                if (bodyHitPrefab != null)
                {
                    Instantiate(bodyHitPrefab, hitPoint, Quaternion.identity);
                }

                Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
                if (enemy != null) enemy.TakeDamage(1);
            }
        }

        if (shouldReload)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    IEnumerator SpawnTracer(Vector3 targetPos)
    {
        LineRenderer tracer = Instantiate(tracerEffectPrefab, muzzlePoint.position, Quaternion.identity);
        tracer.SetPosition(0, muzzlePoint.position);
        tracer.SetPosition(1, targetPos);

        Color startColor = tracer.startColor;
        Color endColor = tracer.endColor;
        float currentTime = 0f;

        while (currentTime < tracerDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / tracerDuration);
            tracer.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            tracer.endColor = new Color(endColor.r, endColor.g, endColor.b, alpha);
            yield return null;
        }

        Destroy(tracer.gameObject);
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(gunData.ReloadTime);
        isReloading = false;
    }
}