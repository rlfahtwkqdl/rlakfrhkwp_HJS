using UnityEngine;
using TMPro;

public class SuccessEndingUI : MonoBehaviour
{
    [Header("성공 결과 UI 텍스트")]
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI totalMoneyText;

    private void Start()
    {
        DisplayFinalResults();
    }

    private void DisplayFinalResults()
    {
        // 1. 파괴되지 않고 넘어온 ScoreManager에서 온전한 총 획득 점수 반영
        if (ScoreManager.Instance != null)
        {
            int finalScore = ScoreManager.Instance.CurrentScore;
            totalScoreText.text = $"총 획득 점수 : {finalScore:N0} PTS";
        }
        else
        {
            totalScoreText.text = "총 획득 점수 : 0 PTS";
        }

        // 2. 파괴되지 않고 넘어온 MoneyManager(가칭)에서 총 획득 골드 반영
        if (MoneyManager.Instance != null)
        {
            int finalMoney = MoneyManager.Instance.CurrentMoney; // 매니저에 구현된 변수명에 맞게 수정하세요!
            totalMoneyText.text = $"획득한 돈 : {finalMoney:N0} GOLD";
        }
        else
        {
            totalMoneyText.text = "획득한 돈 : 0 GOLD";
        }
    }
}