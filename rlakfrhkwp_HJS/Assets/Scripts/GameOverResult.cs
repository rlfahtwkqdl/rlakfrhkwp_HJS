using UnityEngine;
using TMPro;

public class GameOverResult : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    [SerializeField] private TextMeshProUGUI finalScoreText;

    void Start()
    {
        // 씬이 시작되자마자 저장되어 있던 최종 배드엔딩 점수를 불러옵니다.
        DisplayFinalScore();
    }

    private void DisplayFinalScore()
    {
        if (finalScoreText != null)
        {
            // ScoreManager의 싱글톤 인스턴스가 파괴되었어도 static 변수라 안전하게 가져옵니다.
            int finalScore = ScoreManager.FinalCalculatedScore;

            // 마이너스 기호가 붙은 점수를 예쁘게 출력 (예: FINAL SCORE : -24,500)
            finalScoreText.text = $"FINAL SCORE : {finalScore:N0}";

            // 컨셉을 살려 점수가 음수일 때 텍스트 색상을 빨간색으로 변경하는 센스!
            if (finalScore < 0)
            {
                finalScoreText.color = Color.red;
            }
        }
    }
}