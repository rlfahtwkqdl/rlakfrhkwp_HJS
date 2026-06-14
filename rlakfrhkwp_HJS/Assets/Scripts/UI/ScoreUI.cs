using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        // [안전장치 1] 혹시라도 싱글톤 인스턴스 링크가 일시적으로 깨졌다면 직접 찾아내기
        if (ScoreManager.Instance == null)
        {
            ScoreManager foundManager = FindObjectOfType<ScoreManager>();
            if (foundManager != null)
            {
                // 찾았다면 강제로 이벤트 연결 및 초기화 후 종료
                foundManager.OnScoreChanged -= UpdateScoreUI; // 중복 구독 방지
                foundManager.OnScoreChanged += UpdateScoreUI;
                UpdateScoreUI(foundManager.CurrentScore);
                return;
            }
        }

        // [안전장치 2] 정상적으로 인스턴스가 존재할 때의 완벽한 갱신
        if (ScoreManager.Instance != null)
        {
            // 혹시 모를 이전 구독의 잔재를 지우고 깨끗하게 새로 구독
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreUI;
            ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;

            // 현재 점수(0점)로 화면을 즉시 강제 리프레시
            UpdateScoreUI(ScoreManager.Instance.CurrentScore);
        }
        else
        {
            Debug.LogError("[ScoreUI] ScoreManager를 씬 전체에서 눈 씻고 찾아봐도 찾을 수 없습니다!");
        }
    }

    private void OnDestroy()
    {
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
        else
        {
            // ★ 중요: 에디터에서 실수로 텍스트 컴포넌트 연결을 빼먹었는지 실시간 검사
            Debug.LogWarning("[ScoreUI] scoreText 컴포넌트 연결이 비어있습니다! 인스펙터를 확인하세요.");
        }
    }
}