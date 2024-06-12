using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyData", menuName = "Enemy/Data/New EnenmyDataFU")]
public class EnemyScriptableObjectFU : EnemyScriptableObject
{
    [Header("Fonction Utulitaire VARS")]
    public AnimationCurve shooting;
    public AnimationCurve Sprinting;
    public float maxStamina = 40.0f;
    public int maxAmmo = 20;
    [Tooltip("the time when the AI is in Idle state where he is preforming an action like reloading or resting")]
    public float actionTime = 2.3f;
    public float sprintAddedAmount = 3.5f;
    public float shootingDelay = 0.5f;
    [Range(0.01f,0.1f)]
    public float staminaTickRate = 0.05f;
    [Range(0, 0.9f)]
    public float requiredPrecentageToRestFromRunning = 0.12f;
    [Range(0, 0.9f)]
    public float requiredPrecentageToReload = 0.1f;
    public GameObject bulletPrefab;
    public float bulletForce;
    public float bulletUpwardForce;

}
