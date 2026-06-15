using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // 어디서나 편하게 부를 수 있도록 싱글톤(Singleton)으로 만듭니다.
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 카메라를 흔드는 함수
    /// </summary>
    /// <param name="duration">흔들릴 시간 (초)</param>
    /// <param name="magnitude">흔들릴 세기 (강도)</param>
    public void Shake(float duration, float magnitude)
    {
        // 중복 실행 시 원래 위치가 꼬이는 걸 방지하기 위해 이전 코루틴을 멈추고 시작
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // 무작위로 -1 ~ 1 사이의 값을 구한 뒤 세기(magnitude)를 곱함
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // 카메라의 Z축은 유지한 채 X, Y축만 흔듭니다.
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 진동이 끝나면 카메라를 원래 안전한 좌표로 복구
        transform.localPosition = originalPos;
    }
}