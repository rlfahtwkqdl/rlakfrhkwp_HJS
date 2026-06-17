using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 🔴 [추가] UI 위에 마우스가 있는지 감지하기 위해 필요합니다.

public class ClickDamageTest : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private GunData gunData;
    [SerializeField] private string teamKillSceneName = "TeamKillEndingScene";

    [Header("시각 효과 (Tracer)")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private LineRenderer tracerEffectPrefab;
    [SerializeField] private float tracerDuration = 0.05f;

    [Header("★ 적 피격 이펙트 (파티클)")]
    [Tooltip("Enemy 태그를 맞췄을 때")]
    [SerializeField] private ParticleSystem bodyHitPrefab;
    [Tooltip("Head 태그를 맞췄을 때")]
    [SerializeField] private ParticleSystem headHitPrefab;

    [Header("헤드샷 카메라 진동 설정")]
    [Range(0f, 1f)][SerializeField] private float shakeDuration = 0.15f;
    [Range(0f, 2f)][SerializeField] private float shakeMagnitude = 0.2f;

    [Header("🔴 사운드 설정 (오디오 클립)")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip bodyHitSound;
    [SerializeField] private AudioClip headHitSound;
    [SerializeField] private AudioClip reloadSound;

    [Header("🔴 장전 UI 설정")]
    [Tooltip("마우스를 따라다닐 UI의 최상위 부모 오브젝트 (켜고 끄기용)")]
    [SerializeField] private GameObject reloadUiParent;
    [Tooltip("실제로 줄어들게 만들 UI 이미지")]
    [SerializeField] private Image reloadGaugeImage;
    [Tooltip("마우스 커서와 게이지 사이의 간격 조정")]
    [SerializeField] private Vector2 uiOffset = new Vector2(0f, -40f);

    private AudioSource audioSource;
    private bool isReloading = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (reloadUiParent != null) reloadUiParent.SetActive(false);
    }

    void Update()
    {
        if (isReloading && reloadUiParent != null && Mouse.current != null)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            reloadUiParent.transform.position = mouseScreenPos + uiOffset;
        }

        if (isReloading || gunData == null) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 🔴 [★새로 추가] 현재 마우스가 UI 요소(버튼, 이미지 등) 위에 있다면 발사하지 않고 취소!
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

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

        Vector3 targetPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
        StartCoroutine(SpawnTracer(targetPosition));

        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        if (hit.collider != null)
        {
            Vector3 hitPoint = new Vector3(hit.point.x, hit.point.y, -1f);

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("<color=red><b>오인사격! 작전 실패!</b></color>");
                if (ScoreManager.Instance != null) ScoreManager.Instance.CalculateFinalScore();
                SceneManager.LoadScene(teamKillSceneName);
                return;
            }
            else if (hit.collider.CompareTag("Head"))
            {
                Debug.Log("<color=yellow><b>머리통! 장전 시간 초기화!</b></color>");

                if (audioSource != null && headHitSound != null)
                {
                    audioSource.PlayOneShot(headHitSound);
                }

                if (headHitPrefab != null)
                {
                    ParticleSystem effectInstance = Instantiate(headHitPrefab, hitPoint, Quaternion.identity);
                    effectInstance.Play();
                }

                if (CameraShake.Instance != null)
                {
                    CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);
                }

                Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
                if (enemy != null) enemy.InstantKill();
                shouldReload = false;
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("<color=orange>몸 샷</color>");

                if (audioSource != null && bodyHitSound != null)
                {
                    audioSource.PlayOneShot(bodyHitSound);
                }

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

        if (reloadUiParent != null) reloadUiParent.SetActive(true);
        if (reloadGaugeImage != null) reloadGaugeImage.transform.localScale = Vector3.one;

        float elapsed = 0f;
        float duration = gunData.ReloadTime;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (reloadGaugeImage != null)
            {
                float progress = Mathf.Clamp01(1f - (elapsed / duration));
                reloadGaugeImage.transform.localScale = new Vector3(progress, 1f, 1f);
            }
            yield return null;
        }

        if (reloadUiParent != null) reloadUiParent.SetActive(false);

        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        isReloading = false;
    }
}