using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private SpawnConfig config;

    private Transform playerTransform;
    private int currentPhaseIndex = 0;

    private float phaseTimer = 0f;
    private float enemySpawnTimer = 0f; // (명확하게 이름 변경)
    private float moneySpawnTimer = 0f; // ★ 돈 스폰 타이머 추가

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

        // 2. 적 스폰 타이머 작동
        enemySpawnTimer += Time.deltaTime;
        if (enemySpawnTimer >= currentPhase.spawnInterval)
        {
            SpawnEnemyInDonutArea(currentPhase);
            enemySpawnTimer = 0f;
        }

        // 3. ★ 재화(Money) 스폰 타이머 작동 (적과 완전히 독립적)
        // 설정에 돈 풀이 비어있지 않고, 생성 주기가 0보다 클 때만 작동하게 예외 처리
        if (currentPhase.moneyPool != null && currentPhase.moneyPool.Count > 0 && currentPhase.moneySpawnInterval > 0)
        {
            moneySpawnTimer += Time.deltaTime;
            if (moneySpawnTimer >= currentPhase.moneySpawnInterval)
            {
                SpawnMoneyInDonutArea(currentPhase);
                moneySpawnTimer = 0f;
            }
        }
    }

    void NextPhase()
    {
        currentPhaseIndex++;
        phaseTimer = 0f;
        enemySpawnTimer = 0f;
        moneySpawnTimer = 0f; // 다음 페이즈로 갈 때 돈 타이머도 초기화

        if (currentPhaseIndex >= config.phases.Count)
        {
            isAllPhasesFinished = true;
            Debug.Log("<color=cyan><b>[모든 페이즈 종료]</b></color>");
            return;
        }

        Debug.Log($"<color=yellow><b>[페이즈 전환]</b></color> {config.phases[currentPhaseIndex].phaseName} 시작!");
    }

    // --- 스폰 로직 ---

    void SpawnEnemyInDonutArea(PhaseInfo currentPhase)
    {
        if (currentPhase.enemyPool == null || currentPhase.enemyPool.Count == 0) return;

        GameObject selectedPrefab = GetWeightedRandomEnemy(currentPhase.enemyPool);
        if (selectedPrefab == null) return;

        // 위치 계산 함수 호출
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }

    // ★ 재화 스폰 전용 함수 추가
    void SpawnMoneyInDonutArea(PhaseInfo currentPhase)
    {
        GameObject selectedPrefab = GetWeightedRandomMoney(currentPhase.moneyPool);
        if (selectedPrefab == null) return;

        // 적과 동일하게 도넛 모양 범위 내에 스폰
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }

    // 도넛 모양 스폰 위치를 구하는 로직을 재사용하기 위해 따로 분리했습니다.
    Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(config.minSpawnRadius, config.maxSpawnRadius);
        return playerTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0f) * randomDistance;
    }

    // --- 가중치 기반 뽑기 로직 ---

    GameObject GetWeightedRandomEnemy(List<EnemyWeightInfo> pool)
    {
        int totalWeight = 0;
        foreach (var item in pool) totalWeight += item.weight;
        int randomValue = Random.Range(0, totalWeight);

        foreach (var item in pool)
        {
            if (randomValue < item.weight) return item.enemyPrefab;
            randomValue -= item.weight;
        }
        return pool[0].enemyPrefab;
    }

    // ★ 재화 가중치 뽑기 로직 추가
    GameObject GetWeightedRandomMoney(List<MoneyWeightInfo> pool)
    {
        int totalWeight = 0;
        foreach (var item in pool) totalWeight += item.weight;
        int randomValue = Random.Range(0, totalWeight);

        foreach (var item in pool)
        {
            if (randomValue < item.weight) return item.moneyPrefab;
            randomValue -= item.weight;
        }
        return pool[0].moneyPrefab;
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