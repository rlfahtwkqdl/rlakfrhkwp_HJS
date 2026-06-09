using UnityEngine;

[CreateAssetMenu(fileName = "NewCoinData", menuName = "ScriptableObjects/CoinData")]
public class CoinData : ScriptableObject
{
    [Header("코인 정보")]
    [SerializeField] private string coinName = "일반 동전";
    [SerializeField] private int coinValue = 100; // ★ 이 코인이 줄 돈의 액수

    public int CoinValue => coinValue;
    public string CoinName => coinName;
}