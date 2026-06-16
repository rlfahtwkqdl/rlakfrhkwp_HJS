using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (SoundManager.Instance != null)
        {
            // 1. 기존에 저장되어 있던 볼륨 값으로 슬라이더 바 위치 초기화
            slider.value = SoundManager.Instance.GetVolume();

            // 2. 슬라이더를 조작할 때마다 SoundManager의 SetVolume 함수가 실행되도록 리스너 등록
            slider.onValueChanged.AddListener(SoundManager.Instance.SetVolume);
        }
    }
}