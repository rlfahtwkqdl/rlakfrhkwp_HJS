using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance { get; private set; }

    void Awake()
    {
        // DontDestroyOnLoad를 하지 않으므로, 씬이 바뀔 때 알아서 파괴됩니다.
        // 다시 타이틀로 돌아오면 새로 태어난 매니저가 새 UI들과 깨끗하게 연결됩니다.
        Instance = this;
    }

    [Header("이동할 인게임 씬 이름")]
    [SerializeField] private string inGameSceneName = "InGameScene";

    [Header("UI 패널 연결 (나중에 추가할 패널도 여기에 슬롯 늘리시면 됩니다)")]
    [SerializeField] private GameObject scorePanel;

    [Header("상점 패널 연결")]
    [SerializeField] private GameObject shopPanel;

    [Header("상점 패널 연결")]
    [SerializeField] private GameObject helpPanel;

    void Start()
    {
        // 게임을 처음 켰을 때는 스코어 패널이 닫혀있도록 초기화
        if (scorePanel != null)
            scorePanel.SetActive(false);
    }

    // ==========================================
    // 1. 게임 시작 기능
    // ==========================================
    public void ClickStartGame()
    {
        Debug.Log("인게임 씬으로 이동하여 게임을 시작합니다.");
        SceneManager.LoadScene(inGameSceneName);
    }

    // ==========================================
    // 2. 패널 제어 기능 (스코어 패널)
    // ==========================================

    // 버튼 하나로 켜고 끄고를 동시에 하고 싶을 때 (토글)
    public void ToggleScorePanel()
    {
        if (scorePanel != null)
        {
            bool nextState = !scorePanel.activeSelf;
            scorePanel.SetActive(nextState);
            Debug.Log($"스코어 패널 토글: {nextState}");
        }
    }

    // 명시적으로 열기만 할 때 (X 버튼이나 닫기 버튼용으로 분리하고 싶을 때 사용)
    public void OpenScorePanel()
    {
        if (scorePanel != null) scorePanel.SetActive(true);
    }

    public void CloseScorePanel()
    {
        if (scorePanel != null) scorePanel.SetActive(false);
    }


    public void OpenShopPanel()
    {
        if (scorePanel != null) shopPanel.SetActive(true);
    }

    public void CloseShopPanel()
    {
        if (scorePanel != null) shopPanel.SetActive(false);
    }

    public void OpenHelpPanel()
    {
        if (scorePanel != null) helpPanel.SetActive(true);
    }

    public void CloseHelpPanel()
    {
        if (scorePanel != null) helpPanel.SetActive(false);
    }

    // ==========================================
    // 3. 게임 종료 기능
    // ==========================================
    public void ClickQuitGame()
    {
        Debug.Log("게임 종료를 요청했습니다.");

#if UNITY_EDITOR
        // 유니티 에디터 환경에서 테스트할 때도 꺼지도록 처리
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 빌드된 PC/모바일 게임에서 작동
        Application.Quit();
#endif
    }
}