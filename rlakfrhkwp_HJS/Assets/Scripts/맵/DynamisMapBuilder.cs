using System.Collections.Generic;
using UnityEngine;

public class DynamicMapBuilder : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private Transform playerTransform;

    [Header("맵 청크 설정")]
    [SerializeField] private GameObject mapChunkPrefab; // LEGO 블록 프리랩

    [Tooltip("5칸짜리 Cell 0.16이면 -> 0.8 입력")]
    [SerializeField] private Vector2 chunkSize = new Vector2(0.8f, 0.8f);

    [Header("생성 반경 설정")]
    [Tooltip("플레이어 중심으로 상하좌우 몇 블록까지 생성할 것인가?")]
    [SerializeField] private int viewRadius = 1;

    // 이미 생성된 맵 좌표를 기억하는 저장소 (중복 생성 방지)
    private Dictionary<Vector2Int, GameObject> spawnedChunks = new Dictionary<Vector2Int, GameObject>();

    // 주기적으로 맵을 확인하기 위한 변수들
    private Vector2Int currentPlayerGridCoords;
    private float updateInterval = 0.3f; // 0.3초마다 맵 체크 (Update 부하 방지)
    private float timer;

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        if (playerTransform != null)
        {
            // 시작하자마자 플레이어 주위에 맵을 깐다.
            CheckAndSpawnChunks(true);
        }
        else
        {
            Debug.LogError("DynamicMapBuilder: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            CheckAndSpawnChunks(false); // 이동에 따른 새 맵 생성
            CleanOldChunks();        // 멀어진 맵 삭제
        }
    }

    // 플레이어의 현재 격자 좌표를 계산하고 필요하면 맵 생성
    private void CheckAndSpawnChunks(bool firstTime)
    {
        int playerGridX = Mathf.FloorToInt(playerTransform.position.x / chunkSize.x);
        int playerGridY = Mathf.FloorToInt(playerTransform.position.y / chunkSize.y);
        Vector2Int currentCoords = new Vector2Int(playerGridX, playerGridY);

        // 이전 프레임과 격자 좌표가 달라졌다면 (새로운 칸으로 이동했다면)
        if (firstTime || currentCoords != currentPlayerGridCoords)
        {
            currentPlayerGridCoords = currentCoords;

            // 플레이어 중심 viewRadius 반경 내의 모든 격자 칸을 검사
            for (int x = -viewRadius; x <= viewRadius; x++)
            {
                for (int y = -viewRadius; y <= viewRadius; y++)
                {
                    Vector2Int targetChunkCoord = new Vector2Int(currentPlayerGridCoords.x + x, currentPlayerGridCoords.y + y);

                    // 해당 격자 칸이 비어있다면 프리랩 생성
                    if (!spawnedChunks.ContainsKey(targetChunkCoord))
                    {
                        SpawnChunkAt(targetChunkCoord);
                    }
                }
            }
        }
    }

    // 실제 해당 격자 좌표에 맵을 생성하고 Dictionary에 저장
    private void SpawnChunkAt(Vector2Int gridCoord)
    {
        float spawnX = gridCoord.x * chunkSize.x;
        float spawnY = gridCoord.y * chunkSize.y;
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        GameObject chunkInstance = Instantiate(mapChunkPrefab, spawnPosition, Quaternion.identity);
        chunkInstance.transform.SetParent(this.transform); // 정리용 자식 설정
        chunkInstance.name = $"Chunk_{gridCoord.x}_{gridCoord.y}";

        spawnedChunks.Add(gridCoord, chunkInstance);
    }

    // 플레이어와 너무 멀어진 맵은 지운다 (메모리 관리)
    private void CleanOldChunks()
    {
        // 🔴 [오류 수정 완료] 잘못 선언되었던 복합 List 구조를 깨끗하게 리팩토링했습니다.
        List<Vector2Int> keysToRemove = new List<Vector2Int>();

        int destroyRadius = viewRadius + 1;

        foreach (var chunk in spawnedChunks)
        {
            // 두 격자 좌표의 차이를 구한 뒤 거리를 계산합니다.
            Vector2Int diff = chunk.Key - currentPlayerGridCoords;
            float distSqr = diff.sqrMagnitude;

            // 설정한 반경보다 멀어진 블록의 '좌표 Key'만 수집합니다.
            if (distSqr > destroyRadius * destroyRadius)
            {
                keysToRemove.Add(chunk.Key);
            }
        }

        // 수집한 좌표들을 기반으로 실제 게임 오브젝트 파괴 및 메모리 해제
        foreach (var key in keysToRemove)
        {
            if (spawnedChunks.TryGetValue(key, out GameObject chunkInstance))
            {
                if (chunkInstance != null)
                {
                    Destroy(chunkInstance);
                }
                spawnedChunks.Remove(key);
            }
        }
    }
}