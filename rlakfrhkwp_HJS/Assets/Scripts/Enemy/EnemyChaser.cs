using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private EnemyData enemyData; // 이제 모든 데이터는 여기서 가져옵니다.

    private Transform playerTransform;
    private float baseScaleX; // 적의 원래 오리지널 X 크기를 기억할 변수

    void Start()
    {
        // 시작하자마자 인스펙터에 설정된 이 오브젝트의 X축 크기를 기억합니다.
        baseScaleX = transform.localScale.x;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("씬에 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        if (playerTransform != null && enemyData != null)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        float speed = enemyData.MoveSpeed;
        float force = enemyData.SeparationForce; // SO에서 분산 힘 가져오기

        // 1. 플레이어를 향하는 기본 방향 계산 (★ 이 값을 좌우반전에 사용합니다)
        Vector2 directionToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;

        // 2. 주변 적들과 겹치지 않으려는 밀어내는 힘(분산력) 계산
        Vector2 separation = CalculateSeparation();

        // 3. 두 힘을 합산하여 최종 이동 방향 결정 (SO의 힘 적용)
        Vector2 finalDirection = (directionToPlayer + separation * force).normalized;

        // 4. 최종 방향으로 이동
        transform.position += (Vector3)finalDirection * speed * Time.deltaTime;

        // 🔴 [변경] 최종 이동 방향(finalDirection)이 아닌, 실제 플레이어 위치(directionToPlayer)를 기준으로 좌우반전!
        if (directionToPlayer.x > 0.01f)
        {
            // 플레이어가 내 기준 오른쪽에 있으면 원래 크기(양수) 유지
            transform.localScale = new Vector3(Mathf.Abs(baseScaleX), transform.localScale.y, transform.localScale.z);
        }
        else if (directionToPlayer.x < -0.01f)
        {
            // 플레이어가 내 기준 왼쪽에 있으면 X축 크기를 마이너스로 뒤집음
            transform.localScale = new Vector3(-Mathf.Abs(baseScaleX), transform.localScale.y, transform.localScale.z);
        }
    }

    Vector2 CalculateSeparation()
    {
        Vector2 separationVector = Vector2.zero;

        // SO에 설정된 반경(SeparationRadius)으로 주변 적 감지
        Collider2D[] OverlappedColliders = Physics2D.OverlapCircleAll(transform.position, enemyData.SeparationRadius);
        int neighborCount = 0;

        foreach (var collider in OverlappedColliders)
        {
            if (collider.transform.root != transform.root && collider.CompareTag("Enemy"))
            {
                Vector2 awayFromNeighbor = (Vector2)transform.position - (Vector2)collider.transform.position;
                float distance = awayFromNeighbor.magnitude;

                if (distance < 0.01f)
                {
                    separationVector += Random.insideUnitCircle.normalized;
                }
                else
                {
                    separationVector += awayFromNeighbor.normalized / distance;
                }
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            // 평균 분산 벡터 계산
            separationVector /= neighborCount;
        }

        return separationVector;
    }

    // 에디터 뷰에서 감지 반경을 눈으로 확인하기 위한 기즈모
    private void OnDrawGizmosSelected()
    {
        if (enemyData != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, enemyData.SeparationRadius);
        }
    }
}