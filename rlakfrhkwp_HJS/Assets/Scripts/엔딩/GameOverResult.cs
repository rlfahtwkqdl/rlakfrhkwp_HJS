using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverResult : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI evaluationText; // ★ 추가: 배드엔딩 평가 문구

    [Header("이동할 씬 이름")]
    [SerializeField] private string inGameSceneName = "InGameScene";
    [SerializeField] private string titleSceneName = "TitleScene";

    void Start()
    {
        // 씬이 시작되자마자 결과 표시
        DisplayFinalScore();
    }

    private void DisplayFinalScore()
    {
        if (finalScoreText != null)
        {
            int finalScore = ScoreManager.FinalCalculatedScore;

            // 1. 점수 텍스트 표시
            finalScoreText.text = $"FINAL SCORE : {finalScore:N0}";

            if (finalScore < 0)
            {
                finalScoreText.color = Color.red;
            }

            // 2. [컨셉용] 점수대별 매운맛 평가 문구 분기 (★ 추가)
            if (evaluationText != null)
            {
                if (finalScore <= -10000)
                {
                    evaluationText.text = "잡힘3.";
                }
                else if (finalScore <= -5000)
                {
                    evaluationText.text = "잡힘2.";
                }
                else
                {
                    evaluationText.text = "잡힘.";
                }
            }
        }
    }

    // ==========================================
    // ★ 추가: 버튼 클릭 처리 함수들
    // ==========================================

    public void ClickRetry()
    {
        Debug.Log("게임을 다시 시작합니다.");
        SceneManager.LoadScene(inGameSceneName);
    }

    public void ClickMainMenu()
    {
        Debug.Log("메인 타이틀로 이동합니다.");
        SceneManager.LoadScene(titleSceneName);
    }
}