using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private bool isGameOver = false;

    // 2D 트리거 콜라이더와 겹쳐지기 시작할 때 유니티가 호출하는 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 이미 게임 오버 상태라면 무시
        if (isGameOver) return;

        // 2. 겹쳐진 상대방(Collider2D)의 태그가 "Enemy"인지 확인
        // 트리거에서는 collision.gameObject.CompareTag 대신 collision.CompareTag를 바로 쓸 수 있습니다.
        if (collision.CompareTag("Enemy"))
        {
            // 3. 플래그를 true로 잠가서 딱 한 번만 실행되도록 제한
            isGameOver = true;

            // 4. 콘솔 로그 출력
            Debug.Log("<color=orange>잡힘 (게임 오버 - 트리거 방식)</color>");

            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        // 실제 게임오버 처리 로직을 넣을 곳
    }
}