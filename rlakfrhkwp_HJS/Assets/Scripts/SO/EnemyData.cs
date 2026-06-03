using UnityEngine;

// 에디터의 Create 메뉴에 이 SO를 만들 수 있는 항목을 추가합니다.
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("이동 관련 능력치")]
    [SerializeField] private float moveSpeed = 3f;

    

    // 외부(추적 스크립트)에서 안전하게 값을 읽어갈 수 있도록 프로퍼티(Property) 제공
    public float MoveSpeed => moveSpeed;
    
}
