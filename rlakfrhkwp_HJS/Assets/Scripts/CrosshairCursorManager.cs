using UnityEngine;

public class CrosshairCursorManager : MonoBehaviour
{
    [Header("조준선 이미지 (Cursor Type으로 설정 필수)")]
    public Texture2D crosshairTexture;

    void Start()
    {
        if (crosshairTexture != null)
        {
            // [핵심] 이미지의 가로/세로 크기의 절반을 계산하여 핫스팟으로 설정
            // 이렇게 해야 조준선 이미지의 정중앙이 실제 클릭 지점이 됩니다.
            Vector2 hotspot = new Vector2(crosshairTexture.width / 2f, crosshairTexture.height / 2f);

            // 커서 변경 (CursorMode.Auto는 하드웨어 커서를 사용해 지연을 없앱니다)
            Cursor.SetCursor(crosshairTexture, hotspot, CursorMode.Auto);
        }
        else
        {
            Debug.LogError("조준선 이미지가 할당되지 않았습니다!");
        }
    }

    void OnDestroy()
    {
        // 씬을 나갈 때는 기본 커서로 리셋
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}