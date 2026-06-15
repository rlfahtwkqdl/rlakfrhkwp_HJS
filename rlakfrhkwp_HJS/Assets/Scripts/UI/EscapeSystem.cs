using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class EscapeSystem : MonoBehaviour
{
    [Header("설정 데이터 (SO)")]
    [SerializeField] private EscapeConfig config;
    [SerializeField] private string successSceneName = "SuccessEndingScene";

    [Header("인게임 UI 컴포넌트")]
    [SerializeField] private Button escapeButton;

    [Tooltip("기존 문구(ex: 탈출)에서 버튼 클릭 시 즉시 타이머로 변할 텍스트")]
    [SerializeField] private TextMeshProUGUI statusText;

    private bool isEscaping = false;

    private void Start()
    {
        if (escapeButton != null)
        {
            escapeButton.onClick.AddListener(StartEscapeProcess);
        }
    }

    public void StartEscapeProcess()
    {
        if (isEscaping) return;
        StartCoroutine(EscapeCoroutine());
    }

    private IEnumerator EscapeCoroutine()
    {
        isEscaping = true;

        if (escapeButton != null)
            escapeButton.interactable = false; // 버튼 중복 클릭 방지 비활성화

        // 1. 기본 대기 시간을 SO(ScriptableObject)에서 우선 가져옵니다.
        float remainingTime = config.escapeDuration;

        // =============================================================
        // ★ [상점 연동 추가] UpgradeManager가 존재하면 업그레이드로 깎인 최종 시간을 계산해옵니다.
        if (UpgradeManager.Instance != null)
        {
            remainingTime = UpgradeManager.Instance.GetUpgradedEscapeTime(config.escapeDuration);
        }
        // =============================================================

        // 버튼 누른 '즉시' 첫 프레임 타이머 표시
        if (statusText != null)
        {
            statusText.text = $"{remainingTime:F1}초";
        }

        // 0초가 될 때까지 매 프레임 감소하며 0.1초 단위로 갱신
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;

            // 음수로 떨어지는 것을 방지 (0.0초 고정)
            if (remainingTime < 0f) remainingTime = 0f;

            if (statusText != null)
            {
                statusText.text = $"{remainingTime:F1}초";
            }

            // 🚨 [기존 버그 수정] RecordSuccessScore()는 매 프레임 호출하면 안 되므로 
            // while 루프 밖(타이머가 완전히 끝난 시점)으로 이동시켰습니다.

            yield return null;
        }

        // =============================================================
        // 타이머가 완전히 끝난 뒤 처리 (탈출 성공!)
        if (statusText != null)
        {
            statusText.text = "튀는 중";
        }

        // ★ [이동 및 복구] 매 프레임 돌던 함수를 일로 옮겨서 딱 1번만 깔끔하게 저장하게 만들었습니다.
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RecordSuccessScore();
        }

        // "탈출 성공!" 이라는 문구를 플레이어가 눈으로 읽을 수 있게 0.5초만 대기 후 씬 전환
        yield return new WaitForSeconds(0.5f);
        // =============================================================

        SceneManager.LoadScene(successSceneName);
    }
}