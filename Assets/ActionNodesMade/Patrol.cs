using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class Patrol : ActionNode
{
    float patrolTime;
    protected override void OnStart() 
    {
        patrolTime = blackboard.enemyData.patrolTime;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        patrolTime += Time.deltaTime;
        if (patrolTime > blackboard.enemyData.patrolTime)
        {
            patrolTime = 0.0f;
            Vector3 patrolPoint = context.transform.position;

            //! Get a random place to go to 
            float alleatoirX = Random.Range(-blackboard.enemyData.patrolDistance, blackboard.enemyData.patrolDistance);
            float alleatoirZ = Random.Range(-blackboard.enemyData.patrolDistance, blackboard.enemyData.patrolDistance);
            patrolPoint += new Vector3(alleatoirX, 0, alleatoirZ);

            context.agent.SetDestination(patrolPoint);
        }
        return State.Running;

    }
}
