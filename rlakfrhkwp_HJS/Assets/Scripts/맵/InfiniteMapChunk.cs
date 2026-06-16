using UnityEngine;

public class InfiniteMapChunk : MonoBehaviour
{
    private Transform playerTransform;

    [Header("맵 블록 크기 설정")]
    [Tooltip("타일맵 한 블록의 실제 가로/세로 크기 (유니티 Grid 단위)")]
    [SerializeField] private Vector2 chunkSize = new Vector2(40f, 40f);

    void Start()
    {
        // 플레이어 찾기
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("InfiniteMapChunk: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 플레이어와 이 맵 블록 사이의 거리 계산
        float diffX = playerTransform.position.x - transform.position.x;
        float diffY = playerTransform.position.y - transform.position.y;

        // 플레이어가 이동한 방향 (왼쪽/오른쪽, 위/아래)
        float dirX = diffX > 0 ? 1 : -1;
        float dirY = diffY > 0 ? 1 : -1;

        // 절대값 변환
        diffX = Mathf.Abs(diffX);
        diffY = Mathf.Abs(diffY);

        // 🔴 [핵심 로직] 거리가 블록 크기의 1.5배 이상 멀어지면, 플레이어 진행 방향 앞으로 3블록만큼 순간이동!
        // X축 (좌우) 체크
        if (diffX > chunkSize.x * 1.5f)
        {
            transform.position += Vector3.right * dirX * chunkSize.x * 3f;
        }

        // Y축 (위아래) 체크
        if (diffY > chunkSize.y * 1.5f)
        {
            transform.position += Vector3.up * dirY * chunkSize.y * 3f;
        }
    }
}