using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ★ 이름 충돌을 피하기 위해 'ScoreSaveData'로 이름을 변경했습니다!
[System.Serializable]
public class ScoreSaveData
{
    public int highScore = -999999;   // 최고 기록
    public int lowestScore = 999999;  // 최저 기록
    public List<int> recentScores = new List<int>(); // 최근 5판 기록
    public bool hasPlayedBefore = false; // 첫 플레이 판별용
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event Action<int> OnScoreChanged;

    [Header("데이터 연결")]
    [SerializeField] private ScoreData scoreData;

    private int currentScore = 0;
    private float survivalTimer = 0f;
    private string savePath;

    // 타이틀 패널에서 꺼내 쓸 변수들
    private int highScore = 0;
    private int lowestScore = 0;
    private List<int> recentScores = new List<int>();
    private bool hasPlayedBefore = false;

    public int CurrentScore => currentScore;
    public int HighScore => highScore;
    public int LowestScore => lowestScore;
    public List<int> RecentScores => recentScores;

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

        // 스코어 전용 세이브 파일명도 scoreSaveData.json으로 변경하여 안전하게 분리합니다.
        savePath = Path.Combine(Application.persistentDataPath, "scoreSaveData.json");
        LoadGameData();
    }

    void Start()
    {
        OnScoreChanged?.Invoke(currentScore);
    }

    void Update()
    {
        if (scoreData == null) return;

        survivalTimer += Time.deltaTime;
        if (survivalTimer >= scoreData.SurvivalInterval)
        {
            AddScore(scoreData.ScorePerInterval);
            survivalTimer -= scoreData.SurvivalInterval;
        }
    }

    public void AddKillScore(bool isHeadshot)
    {
        if (scoreData == null) return;
        int scoreToAdd = isHeadshot ? scoreData.HeadshotKillScore : scoreData.RegularKillScore;
        AddScore(scoreToAdd);
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
    }

    private void UpdateScoreHistory(int finalScore)
    {
        if (!hasPlayedBefore)
        {
            highScore = finalScore;
            lowestScore = finalScore;
            hasPlayedBefore = true;
        }
        else
        {
            if (finalScore > highScore) highScore = finalScore;
            if (finalScore < lowestScore) lowestScore = finalScore;
        }

        recentScores.Insert(0, finalScore);

        if (recentScores.Count > 5)
        {
            recentScores.RemoveAt(5);
        }

        SaveGameData();
    }

    public void SaveGameData()
    {
        // ★ ScoreSaveData 클래스를 사용하도록 변경
        ScoreSaveData data = new ScoreSaveData
        {
            highScore = this.highScore,
            lowestScore = this.lowestScore,
            recentScores = this.recentScores,
            hasPlayedBefore = this.hasPlayedBefore
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("[ScoreManager] 최고/최저/최근 전적 저장 완료!");
    }

    public void LoadGameData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            // ★ ScoreSaveData 클래스를 사용하도록 변경
            ScoreSaveData data = JsonUtility.FromJson<ScoreSaveData>(json);

            highScore = data.highScore;
            lowestScore = data.lowestScore;
            recentScores = data.recentScores ?? new List<int>();
            hasPlayedBefore = data.hasPlayedBefore;
            Debug.Log("[ScoreManager] 전적 데이터 로드 완료.");
        }
        else
        {
            highScore = 0;
            lowestScore = 0;
            recentScores = new List<int>();
            hasPlayedBefore = false;
        }
    }

    public static int FinalCalculatedScore { get; private set; }

    public void CalculateFinalScore()
    {
        if (MoneyManager.Instance == null) return;

        int score = currentScore;
        int sessionMoney = MoneyManager.Instance.CurrentMoney;

        FinalCalculatedScore = (score + sessionMoney) * -1;

        Debug.Log($"[ScoreManager] 최종 점수 계산 완료: {FinalCalculatedScore}");
        UpdateScoreHistory(FinalCalculatedScore);
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}