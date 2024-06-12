using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class Fuite : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        Vector3 direction =  (blackboard.playerPosition - context.transform.position).normalized;
        float rng = Random.Range(4f, blackboard.enemyData.patrolDistance);
        Vector3 destination = rng * -direction;
        context.agent.SetDestination(destination);
        return State.Success;
    }
}
