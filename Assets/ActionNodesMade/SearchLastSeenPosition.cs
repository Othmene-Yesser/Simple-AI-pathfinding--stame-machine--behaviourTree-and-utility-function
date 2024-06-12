using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SearchLastSeenPosition : ActionNode
{
    float counter;
    protected override void OnStart() 
    {
        context.agent.SetDestination(blackboard.playerPosition);
        counter = 0;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (ReachedLastSeenPosition())
        {
            return State.Failure;
        }
        else if (counter > 5)
        {
            return State.Failure;
        }
        else
        {
            counter += Time.deltaTime;
            return State.Running;
        }
    }
    private bool ReachedLastSeenPosition()
    {
        Vector3 destination = new Vector3(blackboard.moveToPosition.x, context.transform.position.y, blackboard.moveToPosition.z);
        if (Vector3.Distance(context.transform.position, destination) <= blackboard.enemyData.stoppingDistance)
            return true;
        return false;
    }
}
