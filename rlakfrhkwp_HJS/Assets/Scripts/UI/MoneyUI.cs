using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Start()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnMoneyChanged += UpdateMoneyUI;
            UpdateMoneyUI(MoneyManager.Instance.CurrentMoney); // 현재 금액으로 초기화
        }
        else
        {
            Debug.LogError("MoneyUI: MoneyManager를 찾을 수 없습니다.");
        }
    }

    private void OnDestroy()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnMoneyChanged -= UpdateMoneyUI;
        }
    }

    private void UpdateMoneyUI(int newMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = $"GOLD : {newMoney:N0}"; // "GOLD : 1,250" 형태로 출력
        }
    }
}