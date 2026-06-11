using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("이동할 씬 이름")]
    [SerializeField] private string gameOverSceneName = "CreditsScene"; // 에디터에서 변경 가능

    // 무언가 이 적(Enemy)의 콜라이더 안으로 플레이어가 "쏙" 들어왔을 때 실행
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 부딪힌 물체의 태그가 "Player" 인지 확인
        if (collision.CompareTag("Player"))
        {
            Debug.Log("<color=red><b>플레이어가 적과 접촉! 게임 오버 시퀀스를 시작합니다.</b></color>");

            // ==========================================
            // [게임오버 시퀀스 3단계 실행]
            // ==========================================

            // 1. 뒤에서 쌓이던 총 누적 자산을 JSON 파일로 안전하게 저장
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.SaveMoneyData();
            }
            else
            {
                Debug.LogWarning("Enemy: MoneyManager를 찾을 수 없어 저장하지 못했습니다.");
            }

            // 2. 이번 판 성적(스코어 + 돈 * -1)을 토대로 배드엔딩 최종 점수를 계산
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.CalculateFinalScore();
            }
            else
            {
                Debug.LogWarning("Enemy: ScoreManager를 찾을 수 없어 최종 점수를 계산하지 못했습니다.");
            }

            // 3. 크레딧(게임오버 결과) 씬으로 전환
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}