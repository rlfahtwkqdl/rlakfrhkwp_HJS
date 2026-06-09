using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    [SerializeField] private TextMeshProUGUI scoreText;

    // OnEnable 대신 Start를 사용해 타이밍 문제를 방지합니다.
    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;
            UpdateScoreUI(ScoreManager.Instance.CurrentScore); // 현재 점수로 초기화
        }
        else
        {
            Debug.LogError("ScoreUI: ScoreManager 인스턴스를 찾을 수 없습니다! 씬에 ScoreManager가 있는지 확인하세요.");
        }
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 구독 해제
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreUI;
        }
    }

    private void UpdateScoreUI(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE : {newScore:N0}";
        }
    }
}