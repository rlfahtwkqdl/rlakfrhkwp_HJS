using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("데이터 연결")]
    [SerializeField] private EnemyData enemyData; // 위에서 만든 ScriptableObject를 여기에 넣습니다.

    private Transform playerTransform;

    void Start()
    {
        // 씬에서 "Player" 태그를 가진 오브젝트를 찾아서 위치(Transform)를 가져옵니다.
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("씬에 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다! 플레이어 오브젝트의 Tag를 확인하세요.");
        }
    }

    void Update()
    {
        // 플레이어가 존재할 때만 무조건 추적합니다.
        if (playerTransform != null)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        // ScriptableObject(EnemyData)에 설정된 속도 값을 가져옵니다.
        float speed = enemyData.MoveSpeed;

        // 현재 나의 위치에서 플레이어의 위치로 매 프레임 이동합니다 (탑뷰 2D 정석 코드)
        transform.position = Vector2.MoveTowards(
            transform.position,
            playerTransform.position,
            speed * Time.deltaTime
        );
    }
}