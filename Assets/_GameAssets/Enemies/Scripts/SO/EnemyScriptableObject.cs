using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyEnumsData;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New EnemyData", menuName = "Enemy/Data/New EnenmyData")]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Ai Settings")]
    public EnemyType enemyType;
    public EnemyBehaviourType behaviourType;
    public LayerMask detectionLayerMask;
    [Space]
    public float speed = 3.0f;
    [Tooltip("This variable is only used when you check off the use nav mesh roation")]
    public float rotationSpeed = 3.5f;
    public float angularSpeed = 120.0f;
    public float acceleration = 7.0f;
    public float stoppingDistance = 0.8f;
    [Space,Range(0.1f,90.0f)]
    public float detectionAngle = 15.0f;
    public float detectionLength = 5.0f;
    public float chaseTimeAfterVisionLost = 3.0f;
    public float patrolTime = 5.0f;
    public float patrolDistance = 15.0f;
    [Space]
    public float resetAiIfBuggedTime = 5.0f;
    public bool useNavMeshRotation = true;
}

namespace EnemyEnumsData
{
    public enum EnemyType
    {
        FreeRoaming,
        LevelLocked,
        Step15,
        Step30,
    }
    public enum EnemyBehaviourType
    {
        StateMachine,
        BehaviourTree,
        UtilityFunction
    }
}

