using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("이동 관련 능력치")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("분산 설정 (뭉침 방지)")]
    [SerializeField] private float separationRadius = 0.5f; // 다른 적을 감지할 반경
    [SerializeField] private float separationForce = 1.5f;  // 밀어내는 힘의 세기

    // 외부(추적 스크립트)에서 안전하게 값을 읽어갈 수 있도록 프로퍼티(Property) 제공
    public float MoveSpeed => moveSpeed;
    public float SeparationRadius => separationRadius;
    public float SeparationForce => separationForce;
}