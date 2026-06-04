using UnityEngine;

[CreateAssetMenu(fileName = "NewGunData", menuName = "ScriptableObjects/GunData")]
public class GunData : ScriptableObject
{
    [Header("총기 설정")]
    [SerializeField] private float reloadTime = 1.0f; // 장전 시간 (초 단위)

    // 외부에서 안전하게 읽어갈 수 있도록 프로퍼티 제공
    public float ReloadTime => reloadTime;
}