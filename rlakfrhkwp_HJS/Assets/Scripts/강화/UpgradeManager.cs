using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class UpgradeSaveData
{
    public int reloadLevel; // 저장할 장전 강화 레벨
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("강화 설정")]
    [SerializeField] private int maxLevel = 5;       // 최대 강화 레벨
    [SerializeField] private int baseCost = 500;     // 1레벨 강화 비용 (레벨마다 비싸짐)
    [SerializeField] private float timeReductionPerLevel = 0.15f; // 레벨당 감소할 장전 시간 (초)

    private int reloadLevel = 0;
    private string savePath;

    public int ReloadLevel => reloadLevel;
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

    // 현재 레벨에 따른 업그레이드 비용 계산 (예: 500원 -> 1000원 -> 1500원...)
    public int GetUpgradeCost()
    {
        if (reloadLevel >= maxLevel) return 0; // 만렙이면 비용 0
        return baseCost * (reloadLevel + 1);
    }

    // [핵심] 장전 시간 업그레이드 시도 함수
    public bool UpgradeReload()
    {
        if (reloadLevel >= maxLevel) return false;

        int cost = GetUpgradeCost();

        // MoneyManager에게 돈 차감을 요청하고 성공하면 레벨업!
        if (MoneyManager.Instance != null && MoneyManager.Instance.TrySpendMoney(cost))
        {
            reloadLevel++;
            SaveUpgradeData();
            return true;
        }

        return false;
    }

    // [핵심] 인게임 총기 스크립트가 "나 지금 장전 몇 초 걸려?" 하고 물어볼 때 답해주는 함수
    public float GetUpgradedReloadTime(float originalTime)
    {
        // 원본 시간에서 (레벨 * 0.15초)만큼 차감하되, 최소 0.2초보다는 빠르게 안 내려가도록 방어선 구축
        float finalTime = originalTime - (reloadLevel * timeReductionPerLevel);
        return Mathf.Max(finalTime, 0.2f);
    }

    public void SaveUpgradeData()
    {
        UpgradeSaveData data = new UpgradeSaveData { reloadLevel = this.reloadLevel };
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
        }
        else
        {
            reloadLevel = 0;
        }
    }
}