using UnityEngine;
using UnityEngine.InputSystem; // 신형 인풋 시스템

public class ClickDamageTest : MonoBehaviour
{
    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 감지
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            CastRayFromMouse();
        }
    }

    void CastRayFromMouse()
    {
        // 1. 마우스 현재 화면 좌표 읽기
        Vector3 mousePos = Mouse.current.position.ReadValue();

        // 2. Z축 거리를 계산해서 넣어줌 (원근 카메라 버그 방지)
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);

        // 3. 화면 좌표를 게임 속 월드 좌표로 변환
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);

        // 4. 레이캐스트로 충돌 감지
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // 5. 충돌 결과 처리
        if (hit.collider != null)
        {
            // [경우 1] 플레이어 타격
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("<color=red>오인사격</color>");
            }
            // [경우 2] 적 머리 타격
            else if (hit.collider.CompareTag("Head"))
            {
                Debug.Log("<color=yellow><b>머리통!</b></color>");
                // 머리를 맞췄으므로 부모(적 전체)를 파괴합니다.
                DestroyEnemy(hit.transform);
            }
            // [경우 3] 적 몸통 타격 (추가된 기능)
            else if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("<color=orange>몸 샷</color>");
                // 몸통을 맞췄으므로 부모(적 전체)를 파괴합니다.
                DestroyEnemy(hit.transform);
            }
            // [경우 4] 그 외 프리팹이나 타일맵 등
            else
            {
                Debug.Log($"{hit.collider.name} 타격! 데미지를 입혔습니다.");
            }
        }
        else
        {
            // 허공을 클릭했을 때
            Debug.Log("허공을 클릭했습니다 (맞은 오브젝트 없음).");
        }
    }

    // ★ 적의 부모 오브젝트를 찾아 안전하게 파괴하는 전용 함수
    void DestroyEnemy(Transform hitTransform)
    {
        // 맞은 콜라이더의 부모(parent)가 존재한다면 그 부모 오브젝트를 삭제
        if (hitTransform.parent != null)
        {
            Destroy(hitTransform.parent.gameObject);
        }
        else
        {
            // 혹시 부모가 없는 예외적인 상황이라면 본인 자가 파괴
            Destroy(hitTransform.gameObject);
        }
    }
}