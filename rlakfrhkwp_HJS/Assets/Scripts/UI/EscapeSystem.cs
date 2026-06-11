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
        // 게임 시작 시에는 텍스트를 숨기지 않고, 인스펙터에 써둔 그대로(예: "탈출") 둡니다.
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

        // SO에서 설정한 대기 시간을 가져옴
        float remainingTime = config.escapeDuration;

        // [핵심] 버튼 누른 '즉시' 첫 프레임 타이머 표시
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
                // :F1 구문이 소수점 아래 첫째 자리까지 세팅해 줍니다 (예: 4.9, 3.0)
                statusText.text = $"{remainingTime:F1}초";
            }

            yield return null;
        }

       

        SceneManager.LoadScene(successSceneName);
    }
}