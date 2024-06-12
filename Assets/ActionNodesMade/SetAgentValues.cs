using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SetAgentValues : ActionNode
{
    
    protected override void OnStart() 
    {
        context.agent.stoppingDistance = blackboard.enemyData.stoppingDistance;
        context.agent.speed = blackboard.enemyData.speed;
        context.agent.updateRotation = blackboard.enemyData.useNavMeshRotation;
        context.agent.acceleration = blackboard.enemyData.acceleration;
        context.agent.angularSpeed= blackboard.enemyData.angularSpeed;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Failure;
    }
}
