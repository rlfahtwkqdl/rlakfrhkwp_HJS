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
    // [설정 1] 총구 위치 오브젝트 (Hierarchy에서 총구 쪽에 빈 오브젝트 만들어서 연결)
    [SerializeField] private Transform muzzlePoint;
    // [설정 2] 아까 만든 LineRenderer 프리팹
    [SerializeField] private LineRenderer tracerEffectPrefab;
    // [설정 3] 이펙트가 화면에 머무는 아주 짧은 시간 (초)
    [SerializeField] private float tracerDuration = 0.05f;

    private bool isReloading = false;
    // 이펙트를 미리 만들어두고 돌려쓰기 위한 변수 (최적화)
    private LineRenderer currentTracer;

    void Update()
    {
        if (isReloading || gunData == null) return;

        // 마우스 왼쪽 버튼 클릭 감지
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            CastRayFromMouse();
        }
    }

    void CastRayFromMouse()
    {
        // 총구 위치나 프리팹 설정이 안 되어있으면 사격 불가
        if (muzzlePoint == null || tracerEffectPrefab == null)
        {
            Debug.LogError("ClickDamageTest: Muzzle Point 또는 Tracer Prefab이 인스펙터에 연결되지 않았습니다!");
            return;
        }

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        bool shouldReload = true;

        // --- ★ [시각 효과 로직 시작] ---

        // 1. 클릭한 지점이 최종 목표물 지점 (2D이므로 z는 0)
        Vector3 targetPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

        // 2. 이펙트 생성 및 그리기 루틴 시작
        StartCoroutine(SpawnTracer(targetPosition));

        // --- ★ [시각 효과 로직 끝] ---


        // 충돌 결과 처리 (기존 로직 유지)
        if (hit.collider != null)
        {
            // 오인 사격 처리
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("<color=red><b>오인사격! 작전 실패!</b></color>");

                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.CalculateFinalScore();
                }

                SceneManager.LoadScene(teamKillSceneName);
                return;
            }
            // Head나 Enemy 태그를 가진 오브젝트를 쐈을 때
            else if (hit.collider.CompareTag("Head") || hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    if (hit.collider.CompareTag("Head"))
                    {
                        Debug.Log("<color=yellow><b>머리통! 장전 시간 초기화 (즉시 재사격 가능)!</b></color>");
                        enemy.InstantKill();
                        shouldReload = false;
                    }
                    else if (hit.collider.CompareTag("Enemy"))
                    {
                        Debug.Log("<color=orange>몸 샷</color>");
                        enemy.TakeDamage(1);
                    }
                }
            }
        }

        if (shouldReload)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    // --- ★ [히트스캔 이펙트 코루틴] ---
    IEnumerator SpawnTracer(Vector3 targetPos)
    {
        // 1. 프리팹 소환
        LineRenderer tracer = Instantiate(tracerEffectPrefab, muzzlePoint.position, Quaternion.identity);

        // 2. 선의 두 점 설정
        // [0번 점] : 총구 위치 (시작점)
        tracer.SetPosition(0, muzzlePoint.position);
        // [1번 점] : 마우스 클릭 위치 (끝점)
        tracer.SetPosition(1, targetPos);

        // 3. 아주 짧은 시간(tracerDuration) 동안 화면에 띄움
        yield return new WaitForSeconds(tracerDuration);

        // 4. 시간 지나면 이펙트 파괴
        Destroy(tracer.gameObject);
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(gunData.ReloadTime);
        isReloading = false;
    }
}