using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private EnemyData enemyData; // ★ 생성한 EnemyData SO를 여기에 드래그 앤 드롭 하세요.

    private int currentHp;

    void Start()
    {
        if (enemyData != null)
        {
            // ★ ScriptableObject에 저장된 MaxHp 값을 가져와 초기화합니다.
            currentHp = enemyData.MaxHp;
        }
        else
        {
            Debug.LogError($"{gameObject.name}에 EnemyData(ScriptableObject)가 연결되지 않았습니다!");
            currentHp = 3; // 에러 방지용 기본값
        }
    }

    // [몸샷] 일반 데미지 처리 (데미지 1)
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"{gameObject.name}이(가) {damage}의 데미지를 받았습니다. (남은 체력: {currentHp})");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    // [헤드샷] 즉사 처리
    public void InstantKill()
    {
        Debug.Log($"{gameObject.name}이(가) 헤드샷으로 즉사했습니다!");
        Die();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 파괴됨.");
        Destroy(gameObject);
    }
}