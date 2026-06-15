using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("공통 UI")]
    [SerializeField] private TextMeshProUGUI totalMoneyText;

    [Header("1. 장전 속도 UI")]
    [SerializeField] private TextMeshProUGUI reloadLevelText;
    [SerializeField] private TextMeshProUGUI reloadCostText;
    [SerializeField] private Button reloadUpgradeButton;

    [Header("2. 탈출 시간 UI ★ [추가]")]
    [SerializeField] private TextMeshProUGUI escapeLevelText;
    [SerializeField] private TextMeshProUGUI escapeCostText;
    [SerializeField] private Button escapeUpgradeButton;

    void Start()
    {
        // 버튼 1: 장전 속도 강화 이벤트 연결
        if (reloadUpgradeButton != null)
            reloadUpgradeButton.onClick.AddListener(OnReloadUpgradeClicked);

        // 버튼 2: 탈출 시간 감소 강화 이벤트 연결 ★ [추가]
        if (escapeUpgradeButton != null)
            escapeUpgradeButton.onClick.AddListener(OnEscapeUpgradeClicked);

        UpdateShopUI();
    }

    private void OnReloadUpgradeClicked()
    {
        if (UpgradeManager.Instance == null) return;

        if (UpgradeManager.Instance.UpgradeReload())
        {
            Debug.Log("[ShopUI] 장전 속도 강화 성공!");
            UpdateShopUI();
        }
    }

    private void OnEscapeUpgradeClicked()
    {
        if (UpgradeManager.Instance == null) return;

        // ★ [추가] 탈출 시간 업그레이드 실행
        if (UpgradeManager.Instance.UpgradeEscapeTime())
        {
            Debug.Log("[ShopUI] 탈출 시간 단축 성공!");
            UpdateShopUI();
        }
    }

    public void UpdateShopUI()
    {
        if (MoneyManager.Instance == null || UpgradeManager.Instance == null) return;

        // 0. 보유 자산 갱신
        totalMoneyText.text = $"보유 자산 : {MoneyManager.Instance.TotalMoney:N0} GOLD";

        int maxLevel = UpgradeManager.Instance.MaxLevel;

        // 1. 장전 속도 UI 갱신
        int rLevel = UpgradeManager.Instance.ReloadLevel;
        if (rLevel >= maxLevel)
        {
            reloadLevelText.text = $"장전 속도 : Lv.{rLevel} (MAX)";
            reloadCostText.text = "UPGRADE MAX";
            reloadUpgradeButton.interactable = false;
        }
        else
        {
            reloadLevelText.text = $"장전 속도 : Lv.{rLevel} -> Lv.{rLevel + 1}";
            reloadCostText.text = $"강화 비용 : {UpgradeManager.Instance.GetReloadUpgradeCost():N0} GOLD";
            reloadUpgradeButton.interactable = true;
        }

        // 2. 탈출 시간 UI 갱신 ★ [추가]
        int eLevel = UpgradeManager.Instance.EscapeLevel;
        if (eLevel >= maxLevel)
        {
            escapeLevelText.text = $"탈출 대기 : Lv.{eLevel} (MAX)";
            escapeCostText.text = "UPGRADE MAX";
            escapeUpgradeButton.interactable = false;
        }
        else
        {
            escapeLevelText.text = $"탈출 대기 : Lv.{eLevel} -> Lv.{eLevel + 1}";
            escapeCostText.text = $"강화 비용 : {UpgradeManager.Instance.GetEscapeUpgradeCost():N0} GOLD";
            escapeUpgradeButton.interactable = true;
        }
    }
}