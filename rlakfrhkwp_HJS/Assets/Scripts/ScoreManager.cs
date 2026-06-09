using System;
using System.IO; // ★ 파일 입출력을 위해 추가
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event Action<int> OnScoreChanged;

    [Header("데이터 연결")]
    [SerializeField] private ScoreData scoreData;

    private int currentScore = 0;
    private int highScore = 0; // ★ 최고 점수 변수 추가
    private float survivalTimer = 0f;

    private string savePath; // ★ JSON 파일이 저장될 경로

    public int CurrentScore => currentScore;
    public int HighScore => highScore; // 외부에서 최고 점수를 볼 수 있게 프로퍼티 추가

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        // ★ 기기별로 안전한 저장 경로 설정 (AppData/LocalLow/회사명/프로젝트명 안으로 들어갑니다)
        savePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        // ★ 게임이 시작될 때 기존에 저장된 데이터가 있다면 불러옵니다.
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

        // 현재 점수가 최고 점수를 넘으면 실시간으로 최고 점수 갱신
        if (currentScore > highScore)
        {
            highScore = currentScore;
        }
    }

    // ★ 데이터를 JSON으로 저장하는 함수
    public void SaveGameData()
    {
        SaveData data = new SaveData();
        data.highScore = highScore; // 현재 최고 점수를 데이터 객체에 담기

        // 객체를 JSON 문자열로 변환 (true를 넣으면 사람이 보기 좋게 줄바꿈이 됨)
        string json = JsonUtility.ToJson(data, true);

        // 파일로 저장
        File.WriteAllText(savePath, json);
        Debug.Log($"[ScoreManager] 데이터 저장 완료! 경로: {savePath}");
    }

    // ★ JSON 파일을 읽어와 데이터를 복원하는 함수
    public void LoadGameData()
    {
        if (File.Exists(savePath))
        {
            // 파일이 존재하면 읽어옴
            string json = File.ReadAllText(savePath);

            // JSON 문자열을 다시 SaveData 객체로 변환
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScore = data.highScore;
            Debug.Log($"[ScoreManager] 데이터 로드 완료! 최고 점수: {highScore}");
        }
        else
        {
            Debug.Log("[ScoreManager] 저장된 파일이 없어 초기 상태로 시작합니다.");
            highScore = 0;
        }
    }

    // 팁: 테스트용으로 게임이 꺼질 때 자동으로 저장되게 하고 싶다면?
    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}