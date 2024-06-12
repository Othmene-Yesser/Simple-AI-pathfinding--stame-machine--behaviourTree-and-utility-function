using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class ICanSeePlayer : ActionNode
{

    protected override void OnStart() 
    {
    }

    protected override void OnStop() 
    {
    }

    protected override State OnUpdate() {
        Ray ray = new Ray(context.agent.transform.position, context.agent.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * blackboard.enemyData.detectionLength, Color.blue);
        if (!Physics.Raycast(ray, out RaycastHit hit, blackboard.enemyData.detectionLength, blackboard.enemyData.detectionLayerMask))
        {
            ray = new Ray(context.agent.transform.position, Quaternion.Euler(0, (blackboard.enemyData.detectionAngle / 2), 0) * context.agent.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * blackboard.enemyData.detectionLength, Color.blue);
            if (!Physics.Raycast(ray, out hit, blackboard.enemyData.detectionLength, blackboard.enemyData.detectionLayerMask))
            {
                ray = new Ray(context.agent.transform.position, Quaternion.Euler(0, -(blackboard.enemyData.detectionAngle / 2), 0) * context.agent.transform.forward);
                Debug.DrawRay(ray.origin, ray.direction * blackboard.enemyData.detectionLength, Color.blue);
                if (!Physics.Raycast(ray, out hit, blackboard.enemyData.detectionLength, blackboard.enemyData.detectionLayerMask))
                {
                    ray = new Ray(context.agent.transform.position, Quaternion.Euler(0, blackboard.enemyData.detectionAngle, 0) * context.agent.transform.forward);
                    Debug.DrawRay(ray.origin, ray.direction * blackboard.enemyData.detectionLength, Color.blue);
                    if (!Physics.Raycast(ray, out hit, blackboard.enemyData.detectionLength, blackboard.enemyData.detectionLayerMask))
                    {
                        ray = new Ray(context.agent.transform.position, Quaternion.Euler(0, -blackboard.enemyData.detectionAngle, 0) * context.agent.transform.forward);
                        Debug.DrawRay(ray.origin, ray.direction * blackboard.enemyData.detectionLength, Color.blue);
                        if (!Physics.Raycast(ray, out hit, blackboard.enemyData.detectionLength, blackboard.enemyData.detectionLayerMask))
                        {
                            //! Can't see the player anymore
                            return State.Failure;
                        }
                        else//! Was found
                        {
                            blackboard.playerPosition = hit.point;
                            return State.Success;
                        }
                    }
                    else//! Was found
                    {
                        blackboard.playerPosition = hit.point;
                        return State.Success;
                    }
                }
                else//! Was found
                {
                    blackboard.playerPosition = hit.point;
                    return State.Success;
                }
            }
            else//! Was found
            {
                blackboard.playerPosition = hit.point;
                return State.Success;
            }
        }
        else //! Was found
        {
            blackboard.playerPosition = hit.point;
            return State.Success;
        }
    }
}
