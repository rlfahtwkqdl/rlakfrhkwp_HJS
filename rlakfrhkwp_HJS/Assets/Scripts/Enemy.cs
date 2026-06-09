using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private EnemyData enemyData;

    private int currentHp;

    void Start()
    {
        if (enemyData != null)
        {
            currentHp = enemyData.MaxHp;
        }
        else
        {
            Debug.LogError($"{gameObject.name}에 EnemyData가 연결되지 않았습니다!");
            currentHp = 3;
        }
    }

    // [몸샷] 일반 데미지 처리 (데미지 1)
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"{gameObject.name}이(가) {damage}의 데미지를 받았습니다. (남은 체력: {currentHp})");

        if (currentHp <= 0)
        {
            // 몸샷으로 체력이 다 닳아 죽었으므로 이 속성은 headshot = false 입니다.
            Die(isHeadshot: false);
        }
    }

    // [헤드샷] 즉사 처리
    public void InstantKill()
    {
        Debug.Log($"{gameObject.name}이(가) 헤드샷으로 즉사했습니다!");
        // 헤드샷으로 즉사했으므로 headshot = true 입니다.
        Die(isHeadshot: true);
    }

    // 사망 처리 및 오브젝트 삭제 (점수 전달 기능 추가)
    private void Die(bool isHeadshot)
    {
        // ★ ScoreManager가 존재한다면 킬 종류에 따른 점수 지급 요청
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddKillScore(isHeadshot);
        }

        Debug.Log($"{gameObject.name} 파괴됨.");
        Destroy(gameObject);
    }
}