using UnityEngine;

[CreateAssetMenu(fileName = "NewMoneyData", menuName = "ScriptableObjects/MoneyData")]
public class MoneyData : ScriptableObject
{
    [Header("코인 가치 설정")]
    [SerializeField] private int standardCoinValue = 100; // 코인 1개당 지급할 금액

    public int StandardCoinValue => standardCoinValue;
}