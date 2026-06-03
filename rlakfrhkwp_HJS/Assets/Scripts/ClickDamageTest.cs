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
        // 1. 마우스 현재 화면 좌표 읽기 (Z축 보정을 위해 Vector3로 받음)
        Vector3 mousePos = Mouse.current.position.ReadValue();

        // 2. 카메라와 게임 평면 사이의 Z축 거리를 계산해서 넣어줌 (원근 카메라 버그 방지)
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);

        // 3. 화면 좌표를 게임 속 월드 좌표로 변환
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);

        // ★ [디버깅 로그] 클릭할 때마다 좌표가 바뀌는지 콘솔창에서 꼭 확인해보세요!
        Debug.Log($"클릭한 월드 좌표: {mousePosition}");

        // 4. 레이캐스트로 충돌 감지
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // 5. 충돌 결과 처리
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("<color=red>오인사격</color>");
            }
            else if (hit.collider.CompareTag("Head"))
            {
                // 콘솔창에 노란색 굵은 글씨로 표시됩니다.
                Debug.Log("<color=yellow><b>머리통!");
            }
            else
            {
                Debug.Log($"{hit.collider.name} 타격! 데미지를 입혔습니다.");
            }
        }
        else
        {
            // 아무것도 맞지 않았을 때도 로그가 뜨게 하면 디버깅이 편합니다.
            Debug.Log("허공을 클릭했습니다 (맞은 오브젝트 없음).");
        }
    }
}