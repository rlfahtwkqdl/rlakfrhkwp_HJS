using UnityEngine;

[CreateAssetMenu(fileName = "NewEscapeConfig", menuName = "ScriptableObjects/EscapeConfig")]
public class EscapeConfig : ScriptableObject
{
    [Header("탈출 설정")]
    [Tooltip("탈출 버튼을 누른 후 성공할 때까지 버텨야 하는 시간 (초)")]
    public float escapeDuration = 5f;
}