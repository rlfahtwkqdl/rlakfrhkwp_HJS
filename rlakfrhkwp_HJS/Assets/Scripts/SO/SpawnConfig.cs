using System.Collections.Generic;
using UnityEngine;

// 적 프리팹과 등장 확률(가중치)을 묶는 구조체
[System.Serializable]
public struct EnemyWeightInfo
{
    public GameObject enemyPrefab;
    [Tooltip("출현 가중치 (비율을 100 기준으로 맞추면 편합니다.)")]
    public int weight;
}

// ★ 돈(재화) 프리팹과 등장 확률을 묶는 구조체 추가
[System.Serializable]
public struct MoneyWeightInfo
{
    public GameObject moneyPrefab;
    [Tooltip("출현 가중치 (비율을 100 기준으로 맞추면 편합니다.)")]
    public int weight;
}

[System.Serializable]
public struct PhaseInfo
{
    public string phaseName;       // 페이즈 이름
    public float duration;         // 페이즈 유지 시간

    [Header("적 스폰 설정")]
    public float spawnInterval;    // 적 생성 주기
    public List<EnemyWeightInfo> enemyPool;

    [Header("재화(Money) 스폰 설정")]
    public float moneySpawnInterval; // ★ 돈 생성 주기 (적과 별개로 작동)
    public List<MoneyWeightInfo> moneyPool; // ★ 스폰될 돈 프리팹 풀
}

[CreateAssetMenu(fileName = "NewPhaseSpawnConfig", menuName = "ScriptableObjects/PhaseSpawnConfig")]
public class SpawnConfig : ScriptableObject
{
    [Header("스폰 거리 범위 설정")]
    public float minSpawnRadius = 12f;
    public float maxSpawnRadius = 18f;

    [Header("페이즈 리스트")]
    public List<PhaseInfo> phases;
}