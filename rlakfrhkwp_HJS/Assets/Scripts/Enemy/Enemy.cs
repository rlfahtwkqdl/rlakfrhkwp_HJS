using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private string gameOverSceneName = "CreditsScene";

    [Header("페이드아웃 설정")]
    [SerializeField] private float fadeDuration = 1.0f;

    private int currentHp;
    private bool isDead = false;

    // ★ [변경] 단수가 아닌 복수형(배열)으로 자식들의 컴포넌트들을 담습니다.
    private SpriteRenderer[] childSpriteRenderers;
    private Collider2D[] childColliders;

    void Start()
    {
        // ★ [핵심] GetComponent 대신 뒤에 'InChildren'이 붙은 함수를 써서 자식들 것을 다 쓸어 담습니다.
        childSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        childColliders = GetComponentsInChildren<Collider2D>();

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

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        Debug.Log($"{gameObject.name}이(가) {damage}의 데미지를 받았습니다. (남은 체력: {currentHp})");

        if (currentHp <= 0)
        {
            Die(isHeadshot: false);
        }
    }

    public void InstantKill()
    {
        if (isDead) return;

        Debug.Log($"{gameObject.name}이(가) 헤드샷으로 즉사했습니다!");
        Die(isHeadshot: true);
    }

    private void Die(bool isHeadshot)
    {
        if (isDead) return;
        isDead = true; // "나 지금 죽는 중이야!" 선언

        // 🚨 [새로 추가] 죽는 순간 플레이어 추적 스크립트(EnemyChaser)를 통째로 꺼버립니다!
        // 이렇게 하면 EnemyChaser의 Update()와 FollowPlayer()가 즉시 멈춥니다.
        EnemyChaser chaser = GetComponent<EnemyChaser>();
        if (chaser != null)
        {
            chaser.enabled = false;
        }

        // [기존 코드] 자식들의 모든 콜라이더를 루프를 돌며 다 꺼버립니다.
        if (childColliders != null)
        {
            foreach (Collider2D col in childColliders)
            {
                if (col != null) col.enabled = false;
            }
        }

        // 스코어 매니저 점수 지급
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddKillScore(isHeadshot);
        }

        if (isHeadshot)
        {
            Debug.Log($"{gameObject.name} 헤드샷으로 즉시 파괴됨.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"{gameObject.name} 몸샷 사망 - 모든 자식 오브젝트 페이드아웃 시작.");
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    // ★ [변경] 모든 자식 스프라이트를 동시에 서서히 지우는 코루틴
    private IEnumerator FadeOutAndDestroy()
    {
        if (childSpriteRenderers != null && childSpriteRenderers.Length > 0)
        {
            // 자식들이 각자 가지고 있던 원래 색상들을 배열로 기억해 둡니다.
            Color[] startColors = new Color[childSpriteRenderers.Length];
            for (int i = 0; i < childSpriteRenderers.Length; i++)
            {
                if (childSpriteRenderers[i] != null)
                    startColors[i] = childSpriteRenderers[i].color;
            }

            Vector3 startScale = transform.localScale;
            Vector3 startPosition = transform.position;
            float currentTime = 0f;

            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                float progress = currentTime / fadeDuration;

                float alpha = Mathf.Lerp(1f, 0f, progress);

                // ★ [핵심] 루프를 돌며 머리 스프라이트, 몸통 스프라이트의 알파값을 동시에 깎습니다.
                for (int i = 0; i < childSpriteRenderers.Length; i++)
                {
                    if (childSpriteRenderers[i] != null)
                    {
                        childSpriteRenderers[i].color = new Color(startColors[i].r, startColors[i].g, startColors[i].b, alpha);
                    }
                }

                // 크기와 위치는 부모(이동 중심축)만 움직여도 자식들이 알아서 따라옵니다.
                //transform.localScale = Vector3.Lerp(startScale, startScale * 0.8f, progress);
               // transform.position = Vector3.Lerp(startPosition, startPosition + Vector3.down * 0.2f, progress);

                yield return null;
            }
        }

        // 완전히 투명해지면 부모-자식 세트 전체 삭제
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Player"))
        {
            Debug.Log("<color=red><b>플레이어가 적과 접촉! 게임 오버 시퀀스를 시작합니다.</b></color>");
            if (MoneyManager.Instance != null) MoneyManager.Instance.SaveMoneyData();
            if (ScoreManager.Instance != null) ScoreManager.Instance.CalculateFinalScore();
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}