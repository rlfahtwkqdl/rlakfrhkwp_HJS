using UnityEngine;
using TMPro;

public class TeamKillEndingUI : MonoBehaviour
{
    [Header("팀킬 결과 UI 텍스트")]
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private void Start()
    {
        DisplayTeamKillResult();
    }

    private void DisplayTeamKillResult()
    {
        if (ScoreManager.Instance != null)
        {
            // CalculateFinalScore()에 의해 마이너스 연산 처리가 완료된 점수를 가져옵니다.
            int negativeScore = ScoreManager.FinalCalculatedScore;

            finalScoreText.text = $"최종 점수 : {negativeScore:N0} PTS";
            finalScoreText.color = Color.red; // 패널티 느낌 충만하게 붉은색 고정
        }
        else
        {
            finalScoreText.text = "최종 점수 : 0 PTS";
        }
    }
}