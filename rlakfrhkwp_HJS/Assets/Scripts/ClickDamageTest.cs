using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDamageTest : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private GunData gunData;

    private bool isReloading = false;

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
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // 이번 사격 후 장전을 할지 말지 결정하는 변수 (기본값은 장전 함)
        bool shouldReload = true;

        // 충돌 결과 처리
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("<color=red>오인사격</color>");
            }
            // Head나 Enemy 태그를 가진 오브젝트를 쐈을 때
            else if (hit.collider.CompareTag("Head") || hit.collider.CompareTag("Enemy"))
            {
                // 부모를 포함하여 상위에 있는 Enemy 스크립트를 검색
                Enemy enemy = hit.collider.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    // ★ [경우 1] 적 머리 타격 (즉사 및 장전 스킵)
                    if (hit.collider.CompareTag("Head"))
                    {
                        Debug.Log("<color=yellow><b>머리통! 장전 시간 초기화 (즉시 재사격 가능)!</b></color>");
                        enemy.InstantKill();
                        shouldReload = false; // 헤드샷 성공 시 장전 건너뜀
                    }
                    // ★ [경우 2] 몸샷 타격 (데미지 1)
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

        // 헤드샷 성공 시(shouldReload가 false가 됨) 코루틴을 실행하지 않아 즉시 또 쏠 수 있습니다.
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