using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // ★ 씬 전환을 위해 반드시 추가!

public class ClickDamageTest : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private GunData gunData;
    [SerializeField] private string teamKillSceneName = "TeamKillEndingScene"; // ★ 팀킬 엔딩 씬 이름

    private bool isReloading = false;

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
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        bool shouldReload = true;

        if (hit.collider != null)
        {
            // ★ [수정] 오인 사격 (아군/플레이어 타격) 시 처리
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("<color=red><b>오인사격! 작전 실패!</b></color>");

                // 1. 점수 매니저를 호출해 점수 음수화(* -1) 및 JSON 저장 실행
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.CalculateFinalScore();
                }

                // 2. 미련 없이 즉시 팀킬 엔딩 씬으로 이동 (아래 장전 루틴 등 실행 방지)
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
                else
                {
                    Debug.LogWarning($"{hit.collider.name}에 Enemy 스크립트가 부착되어 있지 않습니다!");
                }
            }
            else
            {
                Debug.Log($"{hit.collider.name} 타격! 데미지를 입혔습니다.");
            }
        }
        else
        {
            Debug.Log("허공을 클릭했습니다 (맞은 오브젝트 없음).");
        }

        if (shouldReload)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("<color=cyan>[장전 중...]</color>");

        yield return new WaitForSeconds(gunData.ReloadTime);

        isReloading = false;
        Debug.Log("<color=green>[장전 완료! 사격 가능]</color>");
    }
}