using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDamageTest : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private GunData gunData; // ★ 무기 SO를 여기에 연결합니다.

    private bool isReloading = false;

    void Update()
    {
        // ★ 장전 중이거나 무기 데이터가 할당되지 않았다면 사격 불가
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

        // 한 발 발사했으므로 장전 코루틴 실행
        StartCoroutine(ReloadRoutine());

        // 충돌 결과 처리
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("<color=red>오인사격</color>");
            }
            else if (hit.collider.CompareTag("Head"))
            {
                Debug.Log("<color=yellow><b>머리통!</b></color>");
                DestroyEnemy(hit.transform);
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
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("<color=cyan>[장전 중...]</color>");

        // ★ SO(WeaponData)에 설정된 장전 시간만큼 대기합니다.
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