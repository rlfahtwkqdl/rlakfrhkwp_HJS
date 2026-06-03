using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SpawnConfig config; // 새로 만든 SO 연결

    private Transform playerTransform;

    private int currentPhaseIndex = 0; // 현재 진행 중인 페이즈 인덱스
    private float phaseTimer = 0f;      // 현재 페이즈의 경과 시간 타이머
    private float spawnTimer = 0f;      // 적 생성 주기 타이머
    private bool isAllPhasesFinished = false;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (config == null || config.phases.Count == 0)
        {
            Debug.LogError("SpawnConfig가 비어있거나 페이즈 설정이 없습니다!");
        }
    }

    void Update()
    {
        if (playerTransform == null || config == null || isAllPhasesFinished) return;

        // 현재 페이즈 데이터 가져오기
        PhaseInfo currentPhase = config.phases[currentPhaseIndex];

        // 1. 페이즈 시간 추적 및 전환 로직
        phaseTimer += Time.deltaTime;
        if (phaseTimer >= currentPhase.duration)
        {
            NextPhase();
            return; // 페이즈가 바뀌는 프레임은 스킵
        }

        // 2. 현재 페이즈의 주기에 맞춘 스폰 타이머 작동
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentPhase.spawnInterval)
        {
            SpawnEnemyInDonutArea(currentPhase);
            spawnTimer = 0f;
        }
    }

    // 다음 페이즈로 넘어가는 함수
    void NextPhase()
    {
        currentPhaseIndex++;
        phaseTimer = 0f;
        spawnTimer = 0f;

        if (currentPhaseIndex >= config.phases.Count)
        {
            isAllPhasesFinished = true;
            Debug.Log("<color=cyan><b>[모든 페이즈 종료]</b></color> 준비된 모든 웨이브가 끝났습니다!");
            // 지옥 무한 모드로 변환하거나, 스테이지 클리어 로직을 여기에 넣을 수 있습니다.
            return;
        }

        Debug.Log($"<color=yellow><b>[페이즈 전환]</b></color> {config.phases[currentPhaseIndex].phaseName} 시작!");
    }

    // 최소/최대 거리 사이(도넛 형태 영역)에 적 생성
    void SpawnEnemyInDonutArea(PhaseInfo currentPhase)
    {
        if (currentPhase.enemyPrefabs == null || currentPhase.enemyPrefabs.Count == 0) return;

        // 1. 현재 페이즈 적 풀에서 무작위로 하나 선택
        GameObject selectedPrefab = currentPhase.enemyPrefabs[Random.Range(0, currentPhase.enemyPrefabs.Count)];

        // 2. 무작위 360도 방향 벡터 구하기
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // 3. 최소 거리와 최대 거리 사이의 무작위 거리 구하기
        float randomDistance = Random.Range(config.minSpawnRadius, config.maxSpawnRadius);

        // 4. 최종 스폰 위치 계산 (플레이어 위치 + 방향 * 거리)
        Vector3 spawnPosition = playerTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0f) * randomDistance;

        // 5. 생성
        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }

    // 에디터 씬 뷰에서 스폰 범위를 시각적으로 확인하는 코드 (도넛 모양 가이드라인)
    private void OnDrawGizmosSelected()
    {
        if (config != null)
        {
            Vector3 center = playerTransform != null ? playerTransform.position : transform.position;

            Gizmos.color = Color.green; // 최소 거리는 녹색선
            Gizmos.DrawWireSphere(center, config.minSpawnRadius);

            Gizmos.color = Color.red;   // 최대 거리는 빨간선
            Gizmos.DrawWireSphere(center, config.maxSpawnRadius);
            // 이 두 선 사이에만 적이 생성됩니다.
        }
    }
}