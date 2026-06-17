using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ScoreSaveData
{
    public int highScore = -999999;
    public int lowestScore = 999999;
    public List<int> recentScores = new List<int>();
    public bool hasPlayedBefore = false;
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

    private int highScore = 0;
    private int lowestScore = 0;
    private List<int> recentScores = new List<int>();
    private bool hasPlayedBefore = false;

    // 🔴 [버그 수정용] 현재 인게임 플레이 중인지 판별하는 플래그
    private bool isGameplayActive = false;

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

        savePath = Path.Combine(Application.persistentDataPath, "scoreSaveData.json");
        LoadGameData();
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainGame")
        {
            ResetScoreForNewGame();
            isGameplayActive = true; // 🔴 인게임 진입 시 타이머 작동 활성화
        }
        else
        {
            isGameplayActive = false; // 🔴 타이틀이나 엔딩 씬에서는 타이머 작동 차단
        }
    }

    private void ResetScoreForNewGame()
    {
        currentScore = 0;
        survivalTimer = 0f;

        OnScoreChanged?.Invoke(currentScore);
        Debug.Log("[ScoreManager] 점수 및 타이머 완전히 초기화됨.");
    }

    void Start()
    {
        OnScoreChanged?.Invoke(currentScore);
    }

    void Update()
    {
        // 🔴 [버그 수정] scoreData가 없거나, 인게임('MainGame')이 아니면 생존 타이머를 안 굴립니다.
        if (!isGameplayActive || scoreData == null) return;

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
        ScoreSaveData data = new ScoreSaveData
        {
            highScore = this.highScore,
            lowestScore = this.lowestScore,
            recentScores = this.recentScores,
            hasPlayedBefore = this.hasPlayedBefore
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadGameData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            ScoreSaveData data = JsonUtility.FromJson<ScoreSaveData>(json);

            highScore = data.highScore;
            lowestScore = data.lowestScore;
            recentScores = data.recentScores ?? new List<int>();
            hasPlayedBefore = data.hasPlayedBefore;
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
        // 🔴 [버그 수정] MoneyManager가 혹시나 없을 때 에러가 나거나 세이브를 건너뛰는 현상 방지
        int score = currentScore;
        int sessionMoney = (MoneyManager.Instance != null) ? MoneyManager.Instance.CurrentMoney : 0;

        FinalCalculatedScore = (score + sessionMoney) * -1;
        UpdateScoreHistory(FinalCalculatedScore);
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    public void RecordSuccessScore()
    {
        int score = currentScore;
        int sessionMoney = (MoneyManager.Instance != null) ? MoneyManager.Instance.CurrentMoney : 0;

        int finalSuccessScore = score + sessionMoney;

        UpdateScoreHistory(finalSuccessScore);
        Debug.Log($"[ScoreManager] 탈출 성공 기록 완료! 최종 양수 점수: {finalSuccessScore} PTS");
    }
}