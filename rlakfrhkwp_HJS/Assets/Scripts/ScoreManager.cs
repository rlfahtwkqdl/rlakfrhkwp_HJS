using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // 어디서나 접근할 수 있도록 싱글톤(Singleton) 구현
    public static ScoreManager Instance { get; private set; }

    [Header("데이터 연결")]
    [SerializeField] private ScoreData scoreData; // 생성한 ScoreData SO를 연결하세요.

    private int currentScore = 0;
    private float survivalTimer = 0f;

    // 다른 스크립트에서 현재 점수를 읽을 수 있게 프로퍼티 제공
    public int CurrentScore => currentScore;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 씬이 바뀌어도 파괴되지 않게 하려면 아래 주석을 해제하세요.
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (scoreData == null) return;

        // 버티는 시간(생존 시간)에 따른 점수 자동 추가 로직
        survivalTimer += Time.deltaTime;
        if (survivalTimer >= scoreData.SurvivalInterval)
        {
            AddScore(scoreData.ScorePerInterval);
            survivalTimer -= scoreData.SurvivalInterval; // 타이머 초기화 (오차 방지)
            Debug.Log($"<color=white>[생존 점수 +{scoreData.ScorePerInterval}] 현재 점수: {currentScore}</color>");
        }
    }

    // 적을 처치했을 때 Enemy 스크립트에서 호출할 메서드
    public void AddKillScore(bool isHeadshot)
    {
        if (scoreData == null) return;

        // 헤드샷 여부에 따라 SO에서 알맞은 점수를 가져옴
        int scoreToAdd = isHeadshot ? scoreData.HeadshotKillScore : scoreData.RegularKillScore;
        string killType = isHeadshot ? "<color=yellow>헤드샷 킬!</color>" : "일반 킬";

        AddScore(scoreToAdd);
        Debug.Log($"{killType} <color=green>[+{scoreToAdd}] 현재 점수: {currentScore}</color>");
    }

    // 공통 점수 추가 메서드
    public void AddScore(int amount)
    {
        currentScore += amount;
        // 추후 여기에 UI 업데이트 코드를 넣으면 편리합니다. (예: scoreText.text = currentScore.ToString();)
    }
}