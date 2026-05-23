using UnityEngine;

public class SpecificSceneCursor : MonoBehaviour
{
    public Texture2D customCursor;
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        // 1. 씬이 시작되면 커서를 변경합니다.
        Cursor.SetCursor(customCursor, hotspot, CursorMode.Auto);
    }

    void OnDestroy()
    {
        // 2. 중요! 이 씬을 벗어나서 오브젝트가 파괴될 때 커서를 기본값으로 되돌립니다.
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}