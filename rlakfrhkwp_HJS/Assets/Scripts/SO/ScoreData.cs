using UnityEngine;

[CreateAssetMenu(fileName = "NewScoreData", menuName = "ScriptableObjects/ScoreData")]
public class ScoreData : ScriptableObject
{
    [Header("처치 점수 설정")]
    [SerializeField] private int regularKillScore = 100; // 일반 처치 점수
    [SerializeField] private int headshotKillScore = 250; // 헤드샷 처치 점수

    [Header("생존 점수 설정")]
    [SerializeField] private int scorePerInterval = 10;   // 주기마다 얻을 점수
    [SerializeField] private float survivalInterval = 1f;  // 점수를 줄 시간 간격 (초 단위, 예: 1초마다)

    // 외부에서 읽기 위한 프로퍼티
    public int RegularKillScore => regularKillScore;
    public int HeadshotKillScore => headshotKillScore;
    public int ScorePerInterval => scorePerInterval;
    public float SurvivalInterval => survivalInterval;
}