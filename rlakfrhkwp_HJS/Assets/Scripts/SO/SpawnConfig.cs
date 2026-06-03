using System.Collections.Generic;
using UnityEngine;

// 페이즈 하나당 들어갈 기획 데이터 구조 구조체
[System.Serializable]
public struct PhaseInfo
{
    public string phaseName;       // 에디터 확인용 페이즈 이름 (예: "1페이즈 - 물량공세")
    public float duration;         // 이 페이즈가 유지될 시간 (초)
    public float spawnInterval;    // 이 페이즈에서의 적 생성 주기 (초)
    public List<GameObject> enemyPrefabs; // 이 페이즈에서 등장할 적 종류들
}

[CreateAssetMenu(fileName = "NewPhaseSpawnConfig", menuName = "ScriptableObjects/PhaseSpawnConfig")]
public class SpawnConfig : ScriptableObject
{
    [Header("스폰 거리 범위 설정")]
    [Tooltip("플레이어로부터의 최소 스폰 거리 (화면 안쪽 가리기용)")]
    public float minSpawnRadius = 12f;
    [Tooltip("플레이어로부터의 최대 스폰 거리")]
    public float maxSpawnRadius = 18f;

    [Header("페이즈 리스트")]
    [Tooltip("순서대로 페이즈가 진행됩니다.")]
    public List<PhaseInfo> phases;
}