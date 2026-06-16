using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;
    private const string VolumeParameter = "MasterVolume"; // 1단계에서 지정한 이름

    private void Awake()
    {
        // 싱글톤 구현: 이미 존재하면 새로 생겨난 오브젝트를 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 다른 씬으로 넘어가도 파괴되지 않음

        LoadVolume();
    }

    public void SetVolume(float sliderValue)
    {
        // 슬라이더 값(0.0001~1)을 데시벨(-80~20dB)로 변환하는 공식
        float volumeInDb = Mathf.Log10(sliderValue) * 20;

        // 오디오 믹서 값 변경
        audioMixer.SetFloat(VolumeParameter, volumeInDb);

        // 데이터 저장 (게임을 껐다 켜도 유지되도록)
        PlayerPrefs.SetFloat(VolumeParameter, sliderValue);
    }

    public float GetVolume()
    {
        // 저장된 값이 없으면 기본값인 0.75f를 반환
        return PlayerPrefs.GetFloat(VolumeParameter, 0.75f);
    }

    private void LoadVolume()
    {
        SetVolume(GetVolume());
    }
}