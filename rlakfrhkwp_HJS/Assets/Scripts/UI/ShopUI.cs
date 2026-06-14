using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("UI 텍스트 컴포넌트 연결")]
    [SerializeField] private TextMeshProUGUI totalMoneyText;
    [SerializeField] private TextMeshProUGUI reloadLevelText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;

    [Header("UI 버튼 컴포넌트 연결")]
    [SerializeField] private Button upgradeButton;

    void Start()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }
        else
        {
            Debug.LogError("[ShopUI] 인스펙터에서 Upgrade Button이 연결되지 않았습니다!");
        }

        UpdateShopUI();
    }

    private void OnUpgradeButtonClicked()
    {
        if (UpgradeManager.Instance == null) return;

        bool success = UpgradeManager.Instance.UpgradeReload();

        if (success)
        {
            Debug.Log("<color=green>[ShopUI] 강화 성공! UI를 갱신합니다.</color>");
            UpdateShopUI();
        }
        else
        {
            Debug.LogWarning("[ShopUI] 강화 실패! (잔액 부족 또는 이미 최고 레벨)");
        }
    }

    public void UpdateShopUI()
    {
        // 🚨 [원인 분석존] 어디서 걸려서 UI 반영이 안 되는지 체크합니다.
        if (MoneyManager.Instance == null)
        {
            Debug.LogError("[ShopUI] 빨간불! MoneyManager가 씬에 없습니다. (Null)");
            return;
        }

        if (UpgradeManager.Instance == null)
        {
            Debug.LogError("[ShopUI] 빨간불! UpgradeManager가 씬에 없습니다. (Null)");
            return;
        }

        if (totalMoneyText == null || reloadLevelText == null || upgradeCostText == null)
        {
            Debug.LogError("[ShopUI] 빨간불! 인스펙터에 텍스트(TMP) 컴포넌트 중 일부가 비어있습니다!");
            return;
        }

        // --- 여기서부터 실제 UI 반영 로직 ---
        Debug.Log("[ShopUI] 모든 검사 통과! 정상적으로 UI를 그립니다.");

        // 1. 보유 자산 반영
        totalMoneyText.text = $"보유 자산 : {MoneyManager.Instance.TotalMoney:N0} GOLD";

        // 2. 장전 레벨 및 비용 반영
        int currentLevel = UpgradeManager.Instance.ReloadLevel;
        int maxLevel = UpgradeManager.Instance.MaxLevel;

        if (currentLevel >= maxLevel)
        {
            reloadLevelText.text = $"장전 속도 : Lv.{currentLevel} (MAX)";
            upgradeCostText.text = "UPGRADE MAX";
            upgradeButton.interactable = false;
        }
        else
        {
            reloadLevelText.text = $"장전 속도 : Lv.{currentLevel} -> Lv.{currentLevel + 1}";
            upgradeCostText.text = $"강화 비용 : {UpgradeManager.Instance.GetUpgradeCost():N0} GOLD";
            upgradeButton.interactable = true;
        }
    }
}