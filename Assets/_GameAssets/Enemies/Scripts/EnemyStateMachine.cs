using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{

    [SerializeField] AgentStates state;

    [HideInInspector]
    public EnemyScriptableObject enemyData;
    
    NavMeshAgent enemyAgent;
    PlayerManager player;

    Vector3 lastSeenPosition;

    float patrolTime = 5.0f;

    bool lostVision = false;
    bool debugReset = false;

    IEnumerator StartSearch;

    enum AgentStates
    {
        Patrol,
        Chasing,
        Searching,
    }

    private void OnEnable()
    {
        //Debug.Log("EnemyStarted");
        state = AgentStates.Patrol;
        player = FindAnyObjectByType<PlayerManager>();
        enemyAgent = GetComponent<NavMeshAgent>();
        
    }
    private void FixedUpdate()
    {
        StateHandler();
        if (!enemyData.useNavMeshRotation)
            RotationHandler();

    }

    private void StateHandler()
    {
        switch (state)
        {
            case AgentStates.Patrol:
                //! Patrol
                Patrol();

                //! If then found starts chasing
                if (CanSeePlayer(out lastSeenPosition))
                {
                    state= AgentStates.Chasing;
                }

                break;
            case AgentStates.Chasing:
                //! Chase
                enemyAgent.SetDestination(player.transform.position);


                //! If lost vision the start the counter to lose sight
                if (CanSeePlayer(out lastSeenPosition) == false && lostVision == false)
                {
                    StartSearch = LostVisionOfThePlayer();
                    StartCoroutine(StartSearch);
                }
                else if (CanSeePlayer(out lastSeenPosition) == true && lostVision == true)
                {
                    lostVision = false;
                    StopCoroutine(StartSearch);
                }
                break;
            case AgentStates.Searching:
                //! Go to last seen position
                enemyAgent.SetDestination(lastSeenPosition);
                if (!debugReset)
                {
                    Invoke(nameof(ResetStatusAI), enemyData.resetAiIfBuggedTime + enemyData.patrolTime);//! We do this just in case the ai gets stuck in searching mode
                    debugReset = true;
                }
                //! If player was located then chase
                if (CanSeePlayer(out lastSeenPosition))
                {
                    state = AgentStates.Chasing;
                }

                //! If reached the postion then patrol
                if (ReachedLastSeenPosition())
                {
                    state = AgentStates.Patrol;
                }
                break;
        }
    }
    private void RotationHandler()
    {
        Vector3 direction = enemyAgent.destination - transform.position;
        direction.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * enemyData.rotationSpeed);
    }


    void Patrol()
    {
        patrolTime += Time.deltaTime;
        if (patrolTime > enemyData.patrolTime || ReachedLastSeenPosition(true))
        {
            patrolTime = 0.0f;
            if (patrolTime > enemyData.patrolTime)
            {
                enemyAgent.SetDestination(-enemyAgent.destination);
            }
            
            Vector3 patrolPoint = transform.position;

            //! Get a random place to go to 
            float alleatoirX = Random.Range(-enemyData.patrolDistance, enemyData.patrolDistance);
            float alleatoirZ = Random.Range(-enemyData.patrolDistance, enemyData.patrolDistance);
            patrolPoint += new Vector3(alleatoirX, 0, alleatoirZ);

            enemyAgent.SetDestination(patrolPoint);
        }
    }
    /// <summary>
    /// CanSeePlayer function :
    /// Sends a ray forward then send a ray to then right with an angle of 25 degrees then left with the same angle
    /// if the ray was hit then it outputs the position of the player and it returns true
    /// </summary>
    bool CanSeePlayer(out Vector3 position)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * enemyData.detectionLength, Color.blue);

        if (!Physics.Raycast(ray, out RaycastHit hit, enemyData.detectionLength, enemyData.detectionLayerMask))
        {
            ray = new Ray(transform.position, Quaternion.Euler(0, (enemyData.detectionAngle / 2), 0) * transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * enemyData.detectionLength, Color.blue);
            if (!Physics.Raycast(ray, out hit, enemyData.detectionLength, enemyData.detectionLayerMask))
            {
                ray = new Ray(transform.position, Quaternion.Euler(0, -(enemyData.detectionAngle/2), 0) * transform.forward);
                Debug.DrawRay(ray.origin, ray.direction * enemyData.detectionLength, Color.blue);
                if (!Physics.Raycast(ray, out hit, enemyData.detectionLength, enemyData.detectionLayerMask))
                {
                    ray = new Ray(transform.position, Quaternion.Euler(0, enemyData.detectionAngle, 0) * transform.forward);
                    Debug.DrawRay(ray.origin, ray.direction * enemyData.detectionLength, Color.blue);
                    if (!Physics.Raycast(ray, out hit, enemyData.detectionLength, enemyData.detectionLayerMask))
                    {
                        ray = new Ray(transform.position, Quaternion.Euler(0, -enemyData.detectionAngle, 0) * transform.forward);
                        Debug.DrawRay(ray.origin, ray.direction * enemyData.detectionLength, Color.blue);
                        if (!Physics.Raycast(ray, out hit, enemyData.detectionLength, enemyData.detectionLayerMask))
                        {
                            //! Can't see the player anymore
                            position = lastSeenPosition;
                            return false;
                        }
                        else//! Was found
                        {
                            position = hit.point;
                            return true;
                        }
                    }
                    else//! Was found
                    {
                        position = hit.point;
                        return true;
                    }
                }
                else//! Was found
                {
                    position = hit.point;
                    return true;
                }
            }
            else//! Was found
            {
                position = hit.point;
                return true;
            }
        }
        else //! Was found
        {
            position = hit.point;
            return true;
        }
    }
    IEnumerator LostVisionOfThePlayer()
    {
        lostVision= true;
        yield return new WaitForSeconds(enemyData.chaseTimeAfterVisionLost);
        state = AgentStates.Searching;
        lastSeenPosition = player.transform.position;
        lostVision = false;
    }
    void ResetStatusAI()
    {
        debugReset = false;
        if (state == AgentStates.Patrol || state == AgentStates.Chasing)
            return;
        Debug.Log("Ai was reset by debug :" + gameObject.name);
        state = AgentStates.Patrol;
    }
    bool ReachedLastSeenPosition(bool useNavMeshDestination = false)
    {
        if (useNavMeshDestination)
        {
            Vector3 destination = new Vector3(enemyAgent.destination.x, transform.position.y, enemyAgent.destination.z);
            if (Vector3.Distance(transform.position, destination) <= enemyData.stoppingDistance)
                return true;
            return false;
        }
        else
        {
            Vector3 destination = new Vector3(lastSeenPosition.x, transform.position.y, lastSeenPosition.z);
            if (Vector3.Distance(transform.position, destination) <= enemyData.stoppingDistance)
                return true;
            return false;
        }
    }
}
