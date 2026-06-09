using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class MoneySaveData
{
    public int totalMoney;
}

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public event Action<int> OnMoneyChanged;

    private int currentMoney = 0;
    private string savePath;

    public int CurrentMoney => currentMoney;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        savePath = Path.Combine(Application.persistentDataPath, "moneyData.json");
        LoadMoneyData();
    }

    void Start()
    {
        OnMoneyChanged?.Invoke(currentMoney);
    }

    // ★ 코인이든, 퀘스트 보상이든 주는 액수(amount)대로 지갑에 추가하는 만능 함수
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public void SaveMoneyData()
    {
        MoneySaveData data = new MoneySaveData { totalMoney = currentMoney };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadMoneyData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            MoneySaveData data = JsonUtility.FromJson<MoneySaveData>(json);
            currentMoney = data.totalMoney;
        }
    }

    private void OnApplicationQuit()
    {
        SaveMoneyData();
    }
}