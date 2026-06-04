using UnityEngine;

public class Coin : MonoBehaviour
{
    // 무언가 이 코인의 콜라이더 안으로 "쏙" 들어왔을 때 유니티가 자동으로 실행해 주는 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 부딪힌 물체의 태그가 "Player" 인지 확인
        if (collision.CompareTag("Player"))
        {
            // 나중에 여기에 점수 추가 로직(예: ScoreManager.Instance.AddScore(1);)을 넣으면 됩니다!
            Debug.Log("<color=yellow><b>코인 획득!</b></color>");

            // 코인 오브젝트 파괴 (사라지게 만듦)
            Destroy(gameObject);
        }
    }
}