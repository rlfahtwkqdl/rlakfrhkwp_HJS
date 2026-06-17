using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TitleScorePanel : MonoBehaviour
{
    [Header("상단 고정 기록 UI")]
    [SerializeField] private TextMeshProUGUI highScoreText;  // 최고 기록 텍스트
    [SerializeField] private TextMeshProUGUI lowestScoreText; // 최저 기록 텍스트

    [Header("최근 5판 기록 UI (1번이 가장 최신판)")]
    [SerializeField] private TextMeshProUGUI[] recentScoreTexts; // 크기 5짜리 배열

    void OnEnable()
    {
        // 패널이 활성화될 때마다 갱신된 데이터를 화면에 그립니다.
        RefreshScorePanel();
    }

    void Start()
    {
        // 게임이 처음 켜질 때, OnEnable 단계에서 ScoreManager가 아직 안 깨어났을 상황을 대비해
        // 모든 초기화가 끝난 Start 타이밍에 안전하게 한 번 더 데이터를 짜잔! 하고 뿌려줍니다.
        RefreshScorePanel();
    }

    public void RefreshScorePanel()
    {
        if (ScoreManager.Instance == null) return;

        // 1. 최고 / 최저 기록 뿌리기
        int high = ScoreManager.Instance.HighScore;
        int low = ScoreManager.Instance.LowestScore;

        if (highScoreText != null)
        {
            highScoreText.text = $"최고 기록 : {high:N0} PTS";
            highScoreText.color = high < 0 ? Color.red : Color.white;
        }

        if (lowestScoreText != null)
        {
            lowestScoreText.text = $"최저 기록 : {low:N0} PTS";
            lowestScoreText.color = Color.red; // 최저 기록은 무조건 시뻘겋게 강조
        }

        // 2. 최근 5판 기록 리스트 뿌리기
        List<int> recents = ScoreManager.Instance.RecentScores;

        for (int i = 0; i < recentScoreTexts.Length; i++)
        {
            if (recentScoreTexts[i] == null) continue;

            if (i < recents.Count)
            {
                int score = recents[i];
                recentScoreTexts[i].text = $"[{i + 1}] 최근 플레이 : {score:N0} PTS";
                recentScoreTexts[i].color = score < 0 ? Color.red : Color.white;
            }
            else
            {
                // 아직 플레이 안 한 빈 슬롯 처리
                recentScoreTexts[i].text = $"[{i + 1}] 최근 플레이 : ---";
                recentScoreTexts[i].color = Color.gray;
            }
        }
    }
}