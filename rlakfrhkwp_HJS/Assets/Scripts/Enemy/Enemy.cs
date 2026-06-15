using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // ★ 코루틴(IEnumerator) 사용을 위해 추가

public class Enemy : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private string gameOverSceneName = "CreditsScene";

    [Header("페이드아웃 설정")]
    [SerializeField] private float fadeDuration = 0.25f; // ★ 몸샷 시 빠르게 투명해질 시간 (초)

    private int currentHp;
    private SpriteRenderer spriteRenderer; // ★ 투명도를 조절하기 위한 컴포넌트
    private Collider2D enemyCollider;      // ★ 죽는 순간 충돌을 끌 컴포넌트
    private bool isDead = false;           // ★ 중복 사망 방지용 방어선

    void Start()
    {
        // 필요한 컴포넌트 미리 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();

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
        if (isDead) return; // 이미 죽어가는 상태라면 데미지 연산 무시

        currentHp -= damage;
        Debug.Log($"{gameObject.name}이(가) {damage}의 데미지를 받았습니다. (남은 체력: {currentHp})");

        if (currentHp <= 0)
        {
            Die(isHeadshot: false);
        }
    }

    // [헤드샷] 즉사 처리
    public void InstantKill()
    {
        if (isDead) return; // 이미 죽어가는 상태라면 무시

        Debug.Log($"{gameObject.name}이(가) 헤드샷으로 즉사했습니다!");
        Die(isHeadshot: true);
    }

    // 사망 처리 및 오브젝트 삭제
    private void Die(bool isHeadshot)
    {
        if (isDead) return;
        isDead = true; // "나 지금 죽는 중이야!" 선언

        // 🚨 [가장 중요] 죽는 순간 콜라이더(충돌 판정)를 즉시 꺼버립니다!
        // 이게 없으면 투명해지며 사라지는 와중에 플레이어와 부딪혀서 억울하게 게임오버가 됩니다.
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        // 스코어 매니저 점수 지급
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddKillScore(isHeadshot);
        }

        if (isHeadshot)
        {
            // 1. 헤드샷: 아까 만든 빨간 파편 파티클이 펑 터지므로, 적 본체는 딜레이 없이 즉시 파괴!
            Debug.Log($"{gameObject.name} 헤드샷으로 즉시 파괴됨.");
            Destroy(gameObject);
        }
        else
        {
            // 2. 몸샷: 제자리에서 빠르게 스르륵 투명해지며 사라지기 시작
            Debug.Log($"{gameObject.name} 몸샷 사망 - 페이드아웃 시퀀스 시작.");
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    // ★ [새로 추가] 몸통 사격 사망 시 스르륵 사라지는 코루틴
    private IEnumerator FadeOutAndDestroy()
    {
        if (spriteRenderer != null)
        {
            Color startColor = spriteRenderer.color;
            float currentTime = 0f;

            // 설정한 시간(fadeDuration) 동안 매 프레임 투명도(Alpha)를 1에서 0으로 깎음
            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);

                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null; // 다음 프레임까지 대기
            }
        }

        // 완전히 투명해지면 그제서야 메모리에서 삭제
        Destroy(gameObject);
    }

    // 플레이어가 적과 부딪혔을 때 (게임오버)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return; // ★ 이미 총 맞아 죽어가는 적이라면 게임오버 처리를 하지 않음!

        if (collision.CompareTag("Player"))
        {
            Debug.Log("<color=red><b>플레이어가 적과 접촉! 게임 오버 시퀀스를 시작합니다.</b></color>");

            if (MoneyManager.Instance != null) MoneyManager.Instance.SaveMoneyData();
            if (ScoreManager.Instance != null) ScoreManager.Instance.CalculateFinalScore();

            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}