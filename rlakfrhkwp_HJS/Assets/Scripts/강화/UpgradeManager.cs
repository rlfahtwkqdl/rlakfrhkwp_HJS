using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class UpgradeSaveData
{
    public int reloadLevel; // 장전 강화 레벨
    public int escapeLevel; // ★ [추가] 탈출 강화 레벨
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("공통 설정")]
    [SerializeField] private int maxLevel = 5;       // 모든 강화의 최대 레벨
    [SerializeField] private int baseCost = 500;     // 1레벨 기본 강화 비용

    [Header("1. 장전 속도 설정")]
    [SerializeField] private float timeReductionPerLevel = 0.15f; // 레벨당 감소할 장전 시간 (초)

    [Header("2. 탈출 시간 설정")]
    [SerializeField] private float escapeReductionPerLevel = 3.0f; // ★ [추가] 레벨당 단축될 탈출 시간 (초)

    private int reloadLevel = 0;
    private int escapeLevel = 0; // ★ [추가] 탈출 레벨 변수
    private string savePath;

    public int ReloadLevel => reloadLevel;
    public int EscapeLevel => escapeLevel; // ★ [추가]
    public int MaxLevel => maxLevel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        savePath = Path.Combine(Application.persistentDataPath, "upgradeData.json");
        LoadUpgradeData();
    }

    // --- [장전 속도 관련 함수들] ---
    public int GetReloadUpgradeCost()
    {
        if (reloadLevel >= maxLevel) return 0;
        return baseCost * (reloadLevel + 1);
    }

    public bool UpgradeReload()
    {
        if (reloadLevel >= maxLevel) return false;
        int cost = GetReloadUpgradeCost();

        if (MoneyManager.Instance != null && MoneyManager.Instance.TrySpendMoney(cost))
        {
            reloadLevel++;
            SaveUpgradeData();
            return true;
        }
        return false;
    }

    public float GetUpgradedReloadTime(float originalTime)
    {
        float finalTime = originalTime - (reloadLevel * timeReductionPerLevel);
        return Mathf.Max(finalTime, 0.2f);
    }


    // --- ★ [추가] 2. 탈출 시간 관련 함수들 ---
    public int GetEscapeUpgradeCost()
    {
        if (escapeLevel >= maxLevel) return 0;
        // 장전 비용과 차별화를 두고 싶다면 상수를 곱하거나 다르게 설계 가능합니다 (예: 기본 비용의 1.2배)
        return Mathf.RoundToInt(baseCost * 1.2f * (escapeLevel + 1));
    }

    public bool UpgradeEscapeTime()
    {
        if (escapeLevel >= maxLevel) return false;
        int cost = GetEscapeUpgradeCost();

        if (MoneyManager.Instance != null && MoneyManager.Instance.TrySpendMoney(cost))
        {
            escapeLevel++;
            SaveUpgradeData();
            return true;
        }
        return false;
    }

    // 인게임 탈출 시스템이 "탈출 타이머 몇 초로 세팅해?" 하고 물어볼 때 답해줄 함수
    // UpgradeManager.cs 내부의 해당 함수를 이 로그가 추가된 코드로 덮어써 보세요.
    public float GetUpgradedEscapeTime(float originalTime)
    {
        // 코드 자체는 분명히 마이너스(-)가 맞습니다!
        float finalTime = originalTime - (escapeLevel * escapeReductionPerLevel);
        float clampedTime = Mathf.Max(finalTime, 0.1f);

        // 🚨 콘솔창에 실시간으로 범인을 고발하는 디버그 로그
        Debug.Log($"<color=cyan><b>[탈출 업그레이드 계산기]</b></color>\n" +
                  $"- SO 기본 시간: {originalTime}초\n" +
                  $"- 현재 탈출 레벨: {escapeLevel} Lv\n" +
                  $"- 인펙터에 적은 감소량: {escapeReductionPerLevel}초\n" +
                  $"- 연산된 최종 시간: <color=yellow><b>{clampedTime}초</b></color> (최소 방어선 5초)");

        return clampedTime;
    }


    // --- [저장 / 로드] ---
    public void SaveUpgradeData()
    {
        UpgradeSaveData data = new UpgradeSaveData
        {
            reloadLevel = this.reloadLevel,
            escapeLevel = this.escapeLevel // ★ [추가]
        };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadUpgradeData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            UpgradeSaveData data = JsonUtility.FromJson<UpgradeSaveData>(json);
            reloadLevel = data.reloadLevel;
            escapeLevel = data.escapeLevel; // ★ [추가]
        }
        else
        {
            reloadLevel = 0;
            escapeLevel = 0;
        }
    }
}