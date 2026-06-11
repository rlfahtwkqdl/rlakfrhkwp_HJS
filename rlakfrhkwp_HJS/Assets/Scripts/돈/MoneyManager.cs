using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement; // ★ 씬 감지를 위해 추가

[System.Serializable]
public class MoneySaveData
{
    public int totalMoney; // JSON에는 오직 '전체 누적 금액'만 저장됩니다.
}

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public event Action<int> OnMoneyChanged;

    private int totalMoney = 0;    // 뒤에서 몰래 계속 쌓이는 전체 누적 자산 (JSON 저장/로드 대상)
    private int sessionMoney = 0;  // 이번 게임(스테이지)에서만 획득한 돈 (UI 표시용)

    // 기존 UI 코드가 CurrentMoney를 쓰고 있으므로, 이번 판에 번 돈을 반환하게 합니다.
    public int CurrentMoney => sessionMoney;

    // 나중에 타이틀 화면이나 상점 스크립트에서 전체 자산을 확인하고 싶을 때 쓸 프로퍼티
    public int TotalMoney => totalMoney;

    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ★ [수정] 이게 빠져있어서 매니저가 죽었던 것입니다! 영원히 살려둡니다.
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        savePath = Path.Combine(Application.persistentDataPath, "moneyData.json");

        // 게임 시작 시 이전 판까지 모았던 총 자산을 불러옵니다.
        LoadMoneyData();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // ★ [추정] 새로운 판(인게임 씬)이 열릴 때 자동으로 이번 판 돈을 0원으로 리셋해 줍니다.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 본인의 실제 인게임 씬 이름과 똑같이 맞추세요 (대소문자 구별 필수)
        if (scene.name == "MainGame")
        {
            ResetMoneyForNewGame();
        }
    }

    public void ResetMoneyForNewGame()
    {
        sessionMoney = 0;
        OnMoneyChanged?.Invoke(sessionMoney);
        Debug.Log("[MoneyManager] 이번 판 획득 금액이 0원으로 초기화되었습니다.");
    }

    void Start()
    {
        OnMoneyChanged?.Invoke(sessionMoney);
    }

    // 코인을 먹을 때마다 실행되는 함수
    public void AddMoney(int amount)
    {
        // 1. 이번 판 획득량 증가 (화면 표시용)
        sessionMoney += amount;

        // 2. 전체 누적 자산도 동시에 증가 (뒤에서 몰래 쌓이는 중)
        totalMoney += amount;

        // UI에는 이번 판에 번 돈(sessionMoney)만 알려줍니다!
        OnMoneyChanged?.Invoke(sessionMoney);
    }

    public void SaveMoneyData()
    {
        // 저장할 때는 이번 판에 번 돈이 아니라, '누적된 총액'을 저장합니다.
        MoneySaveData data = new MoneySaveData { totalMoney = totalMoney };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"[MoneyManager] 데이터 백업 완료! 현재 총 자산: {totalMoney}원 (이번 판 획득: {sessionMoney}원)");
    }

    public void LoadMoneyData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            MoneySaveData data = JsonUtility.FromJson<MoneySaveData>(json);

            // 불러올 때도 전체 자산 변수에 넣어줍니다.
            totalMoney = data.totalMoney;
        }
        else
        {
            totalMoney = 0;
        }
    }

    private void OnApplicationQuit()
    {
        SaveMoneyData();
    }
}