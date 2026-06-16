using UnityEngine;
using UnityEngine.UI; // ◀ UI 컴포넌트를 제어하기 위해 반드시 필요합니다!

public class VolumeController : MonoBehaviour
{
    [Header("슬라이더 UI 연결")]
    [SerializeField] private Slider volumeSlider;

    // 볼륨 설정을 저장할 때 사용할 키 이름
    private const string VolumeKey = "MasterVolume";

    void Start()
    {
        if (volumeSlider != null)
        {
            // 1. 기존에 저장된 볼륨 값이 있으면 가져오고, 없으면 기본값(1.0 = 최대)으로 설정합니다.
            float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

            // 2. 슬라이더의 위치와 실제 게임 볼륨을 저장된 값으로 동기화합니다.
            volumeSlider.value = savedVolume;
            AudioListener.volume = savedVolume;

            // 3. 슬라이더를 마우스로 움직일 때마다 OnVolumeChanged 함수가 실행되도록 리스너를 등록합니다.
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        else
        {
            Debug.LogError("VolumeController: Slider가 인스펙터에 연결되지 않았습니다!");
        }
    }

    /// <summary>
    /// 슬라이더 값이 변경될 때 실시간으로 호출되는 함수 (0.0 ~ 1.0)
    /// </summary>
    public void OnVolumeChanged(float value)
    {
        // 실제 게임의 전체 볼륨을 슬라이더 값으로 변경
        AudioListener.volume = value;

        // 게임을 껐다 켜도 유지되도록 현재 볼륨 값 저장
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save(); // 즉시 저장 반영
    }
}