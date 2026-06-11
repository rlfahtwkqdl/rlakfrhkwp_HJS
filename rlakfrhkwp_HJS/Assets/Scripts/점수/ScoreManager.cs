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
            // 중복된 매니저가 파괴될 때는 단순 return하여 기존 Instance를 절대 건드리지 않음
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

    // ★ 안전장치: 진짜 싱글톤 오브젝트가 완전히 파괴될 때만 주소를 비워줍니다.
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 인게임 씬 이름 대소문자 확인 필수
        if (scene.name == "MainGame")
        {
            ResetScoreForNewGame();
        }
    }

    private void ResetScoreForNewGame()
    {
        currentScore = 0;
        survivalTimer = 0f;

        // 새로 태어날 UI들을 위해 이벤트를 한 번 쏴줌
        OnScoreChanged?.Invoke(currentScore);
        Debug.Log("[ScoreManager] 점수 및 타이머 완전히 초기화됨.");
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
        if (MoneyManager.Instance == null) return;

        int score = currentScore;
        int sessionMoney = MoneyManager.Instance.CurrentMoney;

        FinalCalculatedScore = (score + sessionMoney) * -1;
        UpdateScoreHistory(FinalCalculatedScore);
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}