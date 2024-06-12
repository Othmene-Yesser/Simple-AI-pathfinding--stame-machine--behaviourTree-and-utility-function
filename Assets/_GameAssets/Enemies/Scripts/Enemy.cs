using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyEnumsData;
using UnityEngine.AI;
using TheKiwiCoder;
using TreeEditor;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyScriptableObject enemyData;
    [SerializeField] BehaviourTree tree;

    MeshRenderer meshRenderer;
    NavMeshAgent enemyAgent;

    private void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        enemyAgent = GetComponent<NavMeshAgent>();

        switch (enemyData.enemyType)
        {
            case EnemyType.FreeRoaming:
                enemyAgent.agentTypeID = NavMesh.GetSettingsByIndex(((int)enemyData.enemyType) + 1).agentTypeID;
                meshRenderer.material.color = Color.black;
                break;

            case EnemyType.LevelLocked:
                enemyAgent.agentTypeID = NavMesh.GetSettingsByIndex(((int)enemyData.enemyType) + 1).agentTypeID;
                meshRenderer.material.color = Color.yellow;
                break;

            case EnemyType.Step15:
                enemyAgent.agentTypeID = NavMesh.GetSettingsByIndex(((int)enemyData.enemyType) + 1).agentTypeID;
                meshRenderer.material.color = Color.red;
                break;

            case EnemyType.Step30:
                enemyAgent.agentTypeID = NavMesh.GetSettingsByIndex(((int)enemyData.enemyType) + 1).agentTypeID;
                meshRenderer.material.color = Color.blue;
                break;
        }
        switch (enemyData.behaviourType)
        {
            case EnemyBehaviourType.StateMachine:
                //! Use stateMachine
                EnemyStateMachine stateMachine = gameObject.AddComponent<EnemyStateMachine>();
                stateMachine.enemyData = enemyData;
                SetAgentVariables();
                break;
            case EnemyBehaviourType.BehaviourTree:
                //! Use behaviour tree
                tree.blackboard.enemyData = enemyData;
                gameObject.AddComponent<BehaviourTreeRunner>().tree = tree;
                break;
            case EnemyBehaviourType.UtilityFunction:
                break;
            default:
                break;
        }


        Destroy(this);
    }
    private void SetAgentVariables()
    {
        enemyAgent.speed = enemyData.speed;
        enemyAgent.updateRotation = enemyData.useNavMeshRotation;
        enemyAgent.angularSpeed = enemyData.angularSpeed;
        enemyAgent.acceleration = enemyData.acceleration;
        enemyAgent.stoppingDistance = enemyData.stoppingDistance;
    }
}
