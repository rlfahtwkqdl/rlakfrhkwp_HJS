using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SpawnConfig config;

    private Transform playerTransform;
    private int currentPhaseIndex = 0;
    private float phaseTimer = 0f;
    private float spawnTimer = 0f;
    private bool isAllPhasesFinished = false;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null || config == null || isAllPhasesFinished) return;

        PhaseInfo currentPhase = config.phases[currentPhaseIndex];

        // 1. 페이즈 시간 추적 및 전환
        phaseTimer += Time.deltaTime;
        if (phaseTimer >= currentPhase.duration)
        {
            NextPhase();
            return;
        }

        // 2. 스폰 타이머 작동
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentPhase.spawnInterval)
        {
            SpawnEnemyInDonutArea(currentPhase);
            spawnTimer = 0f;
        }
    }

    void NextPhase()
    {
        currentPhaseIndex++;
        phaseTimer = 0f;
        spawnTimer = 0f;

        if (currentPhaseIndex >= config.phases.Count)
        {
            isAllPhasesFinished = true;
            Debug.Log("<color=cyan><b>[모든 페이즈 종료]</b></color>");
            return;
        }

        Debug.Log($"<color=yellow><b>[페이즈 전환]</b></color> {config.phases[currentPhaseIndex].phaseName} 시작!");
    }

    void SpawnEnemyInDonutArea(PhaseInfo currentPhase)
    {
        if (currentPhase.enemyPool == null || currentPhase.enemyPool.Count == 0) return;

        // ★ [핵심 고친 곳] 가중치 기반으로 적 풀에서 단 하나의 적 프리팹을 추려냅니다.
        GameObject selectedPrefab = GetWeightedRandomEnemy(currentPhase.enemyPool);
        if (selectedPrefab == null) return;

        // 위치 계산 및 스폰 (기존과 동일)
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(config.minSpawnRadius, config.maxSpawnRadius);
        Vector3 spawnPosition = playerTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0f) * randomDistance;

        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }

    // ★ 가중치 랜덤 선택 알고리즘 함수
    GameObject GetWeightedRandomEnemy(List<EnemyWeightInfo> enemyPool)
    {
        // 1. 이번 페이즈에 등록된 모든 적의 가중치 총합을 구합니다.
        int totalWeight = 0;
        foreach (var enemy in enemyPool)
        {
            totalWeight += enemy.weight;
        }

        // 2. 0부터 총합 사이의 무작위 숫자를 하나 뽑습니다.
        int randomValue = Random.Range(0, totalWeight);

        // 3. 가중치를 차감해가며 무작위 숫자가 어느 구간에 속하는지 확인합니다.
        foreach (var enemy in enemyPool)
        {
            if (randomValue < enemy.weight)
            {
                return enemy.enemyPrefab; // 당첨된 적 프리팹 반환
            }
            randomValue -= enemy.weight;
        }

        return enemyPool[0].enemyPrefab; // 예외 처리용 백업
    }

    private void OnDrawGizmosSelected()
    {
        if (config != null)
        {
            Vector3 center = playerTransform != null ? playerTransform.position : transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(center, config.minSpawnRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, config.maxSpawnRadius);
        }
    }
}