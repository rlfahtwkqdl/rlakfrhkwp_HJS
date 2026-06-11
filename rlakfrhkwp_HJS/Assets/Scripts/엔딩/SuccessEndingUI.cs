using UnityEngine;
using TMPro;

public class SuccessEndingUI : MonoBehaviour
{
    [Header("성공 결과 UI 텍스트 컴포넌트 연결")]
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI totalMoneyText;

    private void Start()
    {
        DisplayFinalResults();
    }

    private void DisplayFinalResults()
    {
        // 1. 점수 반영 (양수 그대로 가져옴)
        if (ScoreManager.Instance != null)
        {
            int finalScore = ScoreManager.Instance.CurrentScore;
            totalScoreText.text = $"총 획득 점수 : {finalScore:N0} PTS";
        }
        else
        {
            totalScoreText.text = "총 획득 점수 : 0 PTS";
        }

        // 2. 이번 판에 획득한 돈 반영
        if (MoneyManager.Instance != null)
        {
            // MoneyManager의 CurrentMoney(이번 판 번 돈)을 안전하게 가져옵니다.
            int finalMoney = MoneyManager.Instance.CurrentMoney;
            totalMoneyText.text = $"획득한 돈 : {finalMoney:N0} GOLD";
        }
        else
        {
            totalMoneyText.text = "획득한 돈 : 0 GOLD";
        }
    }
}