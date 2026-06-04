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

        // ★ [핵심] 이번 사격 후 장전을 할지 말지 결정하는 변수 (기본값은 장전 함)
        bool shouldReload = true;

        // 충돌 결과 처리
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("<color=red>오인사격</color>");
            }
            // ★ [경우 2] 적 머리 타격
            else if (hit.collider.CompareTag("Head"))
            {
                Debug.Log("<color=yellow><b>머리통! 장전 시간 초기화 (즉시 재사격 가능)!</b></color>");
                DestroyEnemy(hit.transform);

                // ★ 헤드샷을 맞췄으므로 장전 과정을 건너뜁니다!
                shouldReload = false;
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("<color=orange>몸 샷</color>");
                DestroyEnemy(hit.transform);
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

        // ★ 헤드샷 성공 시(shouldReload가 false가 됨) 코루틴을 실행하지 않아 즉시 또 쏠 수 있습니다.
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

    void DestroyEnemy(Transform hitTransform)
    {
        if (hitTransform.parent != null)
        {
            Destroy(hitTransform.parent.gameObject);
        }
        else
        {
            Destroy(hitTransform.gameObject);
        }
    }
}