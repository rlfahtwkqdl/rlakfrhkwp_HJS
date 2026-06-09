using System; // ★ Action 사용을 위해 추가
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // ★ 점수가 변경되었을 때 UI에 알릴 이벤트 선언
    public event Action<int> OnScoreChanged;

    [Header("데이터 연결")]
    [SerializeField] private ScoreData scoreData;

    private int currentScore = 0;
    private float survivalTimer = 0f;

    public int CurrentScore => currentScore;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        // 게임 시작 시 초기 점수(0점)를 UI에 한 번 전달
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

        // ★ 점수가 바뀔 때마다 이벤트를 구독한 UI들에게 새로운 점수를 쏴줍니다!
        OnScoreChanged?.Invoke(currentScore);
    }
}