using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyUtilityFunction : MonoBehaviour
{
    [SerializeField] AnimationCurve ammoFulingCurve;
    [SerializeField] AnimationCurve staminaCurve;
    [SerializeField] AnimationCurve currentCurve;
    [SerializeField] EnemyScriptableObjectFU enemyData;
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float maxAmmo = 10;

    [SerializeField] float stamina;
    [SerializeField] float ammo;
    bool agentInAction = false;

    [SerializeField] UtilityAIState state;

    NavMeshAgent enemyAgent;
    PlayerManager player;

    Vector3 lastSeenPosition;

    float patrolTime = 5.0f;

    bool lostVision = false;
    bool debugReset = false;

    IEnumerator StartSearch;

    private void OnEnable()
    {
        player = FindAnyObjectByType<PlayerManager>();
        enemyAgent = GetComponent<NavMeshAgent>();
        //! Set variables
        state = UtilityAIState.Patrol;
        stamina = maxStamina;
        ammo = maxAmmo;

        //temp
        enemyAgent.updateRotation = enemyData.useNavMeshRotation;
    }
    private void FixedUpdate()
    {
        if (agentInAction)
            return;

        StateHandler();
        if (!enemyData.useNavMeshRotation)
            RotationHandler();

        HandleDecisionMaking();
        //temp
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ammo = 5;
            stamina= 25;
        }
    }

    private void StateHandler()
    {
        switch (state)
        {
            case UtilityAIState.Patrol:
                //! Patrol
                Patrol();

                //! If then found starts chasing
                if (CanSeePlayer(out lastSeenPosition))
                {
                    state = UtilityAIState.Chasing;
                }

                break;
            case UtilityAIState.Chasing:
                //! Chase
                enemyAgent.SetDestination(player.transform.position);
                stamina -= Time.deltaTime;

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
                //! If within shooting distance shoot
                if (Vector3.Distance(transform.position, player.transform.position) <= 1.5f)
                {
                    state = UtilityAIState.Shooting;
                }
                break;
            case UtilityAIState.Searching:
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
                    state = UtilityAIState.Chasing;
                }

                //! If reached the postion then patrol
                if (ReachedLastSeenPosition())
                {
                    state = UtilityAIState.Patrol;
                }
                break;
            case UtilityAIState.Resting: break;
            case UtilityAIState.Reloading: break;
            case UtilityAIState.Shooting:
                //! Shoot
                HandleShooting();
                if (Vector3.Distance(transform.position, player.transform.position) > 1.5f)
                {
                    state = UtilityAIState.Chasing;
                }
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
        if (patrolTime > enemyData.patrolTime)
        {
            patrolTime = 0.0f;
            Vector3 patrolPoint = lastSeenPosition;

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
                ray = new Ray(transform.position, Quaternion.Euler(0, -(enemyData.detectionAngle / 2), 0) * transform.forward);
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
        lostVision = true;
        yield return new WaitForSeconds(enemyData.chaseTimeAfterVisionLost);
        state = UtilityAIState.Searching;
        lastSeenPosition = player.transform.position;
        lostVision = false;
    }
    void ResetStatusAI()
    {
        debugReset = false;
        if (state == UtilityAIState.Patrol || state == UtilityAIState.Chasing)
            return;
        Debug.Log("Ai was reset by debug :" + gameObject.name);
        state = UtilityAIState.Patrol;
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
    void HandleShooting()
    {
        Debug.Log("Shoot");
        ammo--;
        
    }
    void HandleDecisionMaking()
    {
        float currentAction = currentCurve.Evaluate(1);
        float shootValue = ammoFulingCurve.Evaluate(Percentage(ammo, maxAmmo));
        float staminaValue = staminaCurve.Evaluate(Percentage(stamina, maxStamina));
        //Debug.Log(currentAction);
        //Debug.Log(Percentage(ammo, maxAmmo));
        //Debug.Log(shootValue);
        //Debug.Log(staminaValue);
        if (currentAction > shootValue && currentAction > staminaValue) //! Won't do anything if the current curve bigger than one of the curves
        {
            //Debug.Log("???");
            
            return;
        }
        switch (shootValue > staminaValue)
        {
            case true:
                //! Refuel ammo
                state = UtilityAIState.Reloading;
                agentInAction = true;
                Debug.Log("Resting Ammo");
                StartCoroutine(RestAmmo());
                break;
            case false:
                //! Rest stamina
                state = UtilityAIState.Resting;
                agentInAction = true;
                Debug.Log("Resting Stamina");
                StartCoroutine(RestStamina());
                break;
        }
    }
    IEnumerator RestStamina()
    {
        yield return new WaitForSeconds(3);
        state = UtilityAIState.Patrol;
        agentInAction = false;
        stamina = maxStamina;
    }
    IEnumerator RestAmmo()
    {
        yield return new WaitForSeconds(3);
        state = UtilityAIState.Patrol;
        agentInAction = false;
        ammo = maxAmmo;
    }
    float Percentage(float val1, float val2)
    {
        float value = val1 / val2;
        return (value > 0) ? value : 0;
    }
}

public enum UtilityAIState
{
    Patrol,
    Chasing,
    Searching,
    Shooting,
    Reloading,
    Resting,
}
