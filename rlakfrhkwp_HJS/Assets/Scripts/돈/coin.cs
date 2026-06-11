using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("이 코인의 데이터 설정")]
    [SerializeField] private CoinData coinData; // ★ 은화 프리팹에는 은화 SO를, 금화에는 금화 SO를 조립!

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (MoneyManager.Instance != null && coinData != null)
            {
                // ★ 핵심: 이 코인 SO에 적힌 고유한 가치(Value)를 매니저에게 보냅니다.
                MoneyManager.Instance.AddMoney(coinData.CoinValue);

                Debug.Log($"<color=yellow><b>{coinData.CoinName} 획득! (+{coinData.CoinValue}원)</b></color>");
            }
            else if (coinData == null)
            {
                Debug.LogError($"{gameObject.name}: CoinData가 연결되지 않았습니다!");
            }

            Destroy(gameObject);
        }
    }
}