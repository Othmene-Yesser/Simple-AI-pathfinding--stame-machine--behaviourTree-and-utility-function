using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAIFonctionUtulitaire : MonoBehaviour
{
    [SerializeField] FonctionUtulitaire state;
    FonctionUtulitaire State
    {
        get
        { 
            return state;
        }
        set 
        {
            state = value;
        }
    }

    NavMeshAgent enemyAgent;
    public EnemyScriptableObjectFU enemyData;

    [SerializeField] Transform raySpawnPosition;
    [SerializeField] Vector3 lastSeenPosition;
    [SerializeField] Vector3 destination;
    [SerializeField] Transform shootPoint;
    Vector3 _destination => enemyAgent.destination;

    [SerializeField] bool playerInVision = false;

    [SerializeField] bool isIdle = false;
    [SerializeField] bool shootingCoroutineIsRunning = false;
    [SerializeField] int ammo;
    [SerializeField] bool sprintingCoroutineIsRunning = false;
    [SerializeField] float stamina;
    [SerializeField] bool patrolCoroutineIsRunning = false;
    [SerializeField] bool chaseCoroutineIsRunning = false;
    [SerializeField] bool searchCoroutineIsRunning = false;

    //! Player Detection Variables
    bool[] rayStatus;
    Ray[] rays;
    RaycastHit[] hits;

    bool timerDoneSearch;



    private void OnEnable()
    {
        State = FonctionUtulitaire.Patrol;
        enemyAgent = GetComponent<NavMeshAgent>();

        rayStatus = new bool[17];
        rays = new Ray[17];
        hits = new RaycastHit[17];

        Debug.Log("Started Utility Function");
        State = FonctionUtulitaire.Patrol;
        StartCoroutine(Patrol());
        timerDoneSearch = false;

        stamina = enemyData.maxStamina;
        ammo = enemyData.maxAmmo;
        if (raySpawnPosition == null)
        {
            raySpawnPosition = transform;
        }
        enemyAgent.stoppingDistance = enemyData.stoppingDistance;
    }
    private void Update()
    {
        playerInVision = CanSeePlayer();
        destination = _destination;
    }

    //void AIStateHandler()
    //{
    //    switch (State)
    //    {
    //        case FonctionUtulitaire.Idle:
    //            break;
    //        case FonctionUtulitaire.Patrol:
    //            if (playerInVision)
    //            {
    //                State = FonctionUtulitaire.Chase;
    //            }
    //            break;
    //        case FonctionUtulitaire.Chase:
    //            if (DistanceFromLastRegisteredPosition() > (enemyData.detectionLength / 2))
    //            {
    //                //! Sprint to player
    //                State = FonctionUtulitaire.Sprint;
    //            }
    //            break;
    //        case FonctionUtulitaire.Sprint:
    //            if (DistanceFromLastRegisteredPosition() < (enemyData.detectionLength / 2))
    //            {
    //                //! Chase normally
    //                State = FonctionUtulitaire.Chase;
    //            }
    //            break;
    //        case FonctionUtulitaire.Shooting:
    //            Shoot();
    //            break;
    //        case FonctionUtulitaire.Searching:
    //            if (playerInVision)
    //                State = FonctionUtulitaire.Chase;
    //            break;
    //        default: 
    //            break;
    //    }
    //}

    float DistanceFromLastRegisteredPosition(bool patrol = false)
    {
        Vector3 position = new Vector3(transform.position.x, 0, transform.position.z);
        if (patrol)
        {
            Vector3 _destination = new Vector3(enemyAgent.destination.x, 0, enemyAgent.destination.z);
            return Vector3.Distance(_destination, position);
        }
        Vector3 destination = new Vector3(lastSeenPosition.x, 0, lastSeenPosition.z);
        return Vector3.Distance(destination,position);
    }

    bool CheckIfNeededToIdle()
    {
        float stamina = CalculatePercentage(this.stamina, enemyData.maxStamina);
        float ammo = CalculatePercentage(this.ammo, enemyData.maxAmmo);
        if (ammo <= enemyData.requiredPrecentageToReload)
        {
            this.ammo = enemyData.maxAmmo;
            return true;
        }
        else
        {
            if (stamina <= enemyData.requiredPrecentageToRestFromRunning)
            {
                this.stamina = enemyData.maxStamina;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    Vector3 GenerateRandomPosition()
    {
        float randomX = Random.Range(-enemyData.patrolDistance, enemyData.patrolDistance);
        float randomZ = Random.Range(-enemyData.patrolDistance, enemyData.patrolDistance);
        return new Vector3(randomX + transform.position.x, 0, randomZ + transform.position.z);
    }

    FonctionUtulitaire UtilityFunctionDecision()
    {
        float distance = DistanceFromLastRegisteredPosition();
        float sprintValue = enemyData.Sprinting.Evaluate(CalculatePercentage(distance, enemyData.detectionLength));
        float shootingValue = enemyData.shooting.Evaluate(CalculatePercentage(distance, enemyData.detectionLength));

        if (sprintValue > shootingValue)
        {
            //! Sprint
            return FonctionUtulitaire.Sprint;
        }
        else
        {
            //! Shoot
            return FonctionUtulitaire.Shooting;
        }
    }

    float CalculatePercentage(float value, float maxValue)
    {
        var percentage = (value * 100 / maxValue)/100;
        return percentage;
    }
    void Shoot()
    {
        ammo--;
        GameObject prefab = Instantiate(enemyData.bulletPrefab, shootPoint.position, Quaternion.identity);
        Vector3 direction = (lastSeenPosition - shootPoint.position).normalized;
        prefab.GetComponent<Rigidbody>().AddForce(direction * enemyData.bulletForce + (Vector3.up * enemyData.bulletUpwardForce), ForceMode.Impulse);
        Debug.Log("Shoot");
    }
    bool CanSeePlayer()
    {
        int i = 0;

        rays[i] = new Ray(raySpawnPosition.position, transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.red);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, (enemyData.detectionAngle * 1 / 2), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, (enemyData.detectionAngle * 3 / 8), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, (enemyData.detectionAngle * 1 / 4), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, (enemyData.detectionAngle * 1 / 8), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, (enemyData.detectionAngle * 3 / 4), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, (enemyData.detectionAngle * 5 / 8), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, (enemyData.detectionAngle * 7 / 8), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, -(enemyData.detectionAngle * 1 / 2), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, -(enemyData.detectionAngle * 3 / 8), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, (-enemyData.detectionAngle * 1 / 4), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, (-enemyData.detectionAngle * 1 / 8), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, (-enemyData.detectionAngle * 3 / 4), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.black);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, -(enemyData.detectionAngle * 5 / 8), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, (-enemyData.detectionAngle * 7 / 8), 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.blue);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, enemyData.detectionAngle, 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.red);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);
        i++;

        rays[i] = new Ray(raySpawnPosition.position, Quaternion.Euler(0, -enemyData.detectionAngle, 0) * transform.forward);
        Debug.DrawRay(rays[i].origin, rays[i].direction * enemyData.detectionLength, Color.red);
        rayStatus[i] = Physics.Raycast(rays[i], out hits[i], enemyData.detectionLength, enemyData.detectionLayerMask);

        i = 0;
        while (i < rays.Length)
        {
            if (rayStatus[i])
            {
                lastSeenPosition = hits[i].point;
                return true;
            }
            i++;
        }
        return false;
    }
    void ResetValues()
    {
        enemyAgent.speed = enemyData.speed;
    }
    public enum FonctionUtulitaire
    {
        Idle,
        Patrol,
        Chase,
        Searching,
        Sprint,
        Shooting
    }

    #region IEnumerators

    IEnumerator IdleForXAmountOfSeconds(float time = 0.1f)
    {
        enemyAgent.SetDestination(transform.position);
        yield return new WaitForSeconds(time);
        State = FonctionUtulitaire.Patrol;
        StartCoroutine(Patrol());
    }
    IEnumerator Shooting()
    {
        shootingCoroutineIsRunning = true;
        while (playerInVision && State == FonctionUtulitaire.Shooting)
        {
            //! Shoot 
            Shoot();
            //! Wait for Time
            yield return new WaitForSeconds(enemyData.shootingDelay);
        }
        shootingCoroutineIsRunning = false;
    }
    IEnumerator Sprint()
    {
        sprintingCoroutineIsRunning = true;
        enemyAgent.speed = enemyData.speed + enemyData.sprintAddedAmount;
        while (playerInVision && State == FonctionUtulitaire.Sprint)
        {
            //! sprint
            stamina -= enemyData.staminaTickRate;
            yield return null;
        }
        ResetValues();
        sprintingCoroutineIsRunning = false;
    }

    IEnumerator Patrol()
    {
        patrolCoroutineIsRunning = true;
        float timer = enemyData.patrolTime;
        while (State == FonctionUtulitaire.Patrol)
        {
            if (timer >= enemyData.patrolTime || DistanceFromLastRegisteredPosition(true) <= enemyData.stoppingDistance)
            {
                timer = 0f;
                Debug.Log("set new dest");
                enemyAgent.SetDestination(GenerateRandomPosition());
            }
            timer += Time.deltaTime;

            yield return null;

            if (playerInVision)
            {
                State = FonctionUtulitaire.Chase;
                StartCoroutine(Chase());
                break;
            }
        }
        patrolCoroutineIsRunning = false;
    }

    IEnumerator Chase()
    {
        chaseCoroutineIsRunning = true;
        while (State == FonctionUtulitaire.Chase || State == FonctionUtulitaire.Shooting || State == FonctionUtulitaire.Sprint)
        {
            enemyAgent.SetDestination(lastSeenPosition);

            //! Do Action
            FonctionUtulitaire decidedState = UtilityFunctionDecision();
            //! Only change the state when there is a different one 
            switch (decidedState)
            {
                case FonctionUtulitaire.Sprint:
                    //! If he is sprinting then he won't need to sprint
                    if (State == FonctionUtulitaire.Sprint)
                        break;
                    State = FonctionUtulitaire.Sprint;
                    StartCoroutine(Sprint());
                    break;
                case FonctionUtulitaire.Shooting:
                    //! If he is already shooting there is no need to shoot 
                    if (State == FonctionUtulitaire.Shooting)
                        break;

                    State = FonctionUtulitaire.Shooting;
                    StartCoroutine(Shooting());
                    break;
                default:
                    Debug.LogError("What ??");
                    break;
            }

            //! Check if needed to rest or reload
            bool idle = CheckIfNeededToIdle();
            if (idle)
            {
                State = FonctionUtulitaire.Idle;
                StartCoroutine(IdleForXAmountOfSeconds(enemyData.actionTime));
            }

            yield return null;
            
            if (!CanSeePlayer())
            {
                State = FonctionUtulitaire.Searching;
                StartCoroutine(Search());
            }
        }
        chaseCoroutineIsRunning = false;
    }
    IEnumerator Search()
    {
        searchCoroutineIsRunning = true;
        enemyAgent.SetDestination(lastSeenPosition);
        yield return null;
        IEnumerator timer = SearchTimer();
        StartCoroutine(timer);
        while (State == FonctionUtulitaire.Searching)
        {
            yield return null;
            if (DistanceFromLastRegisteredPosition() <= enemyData.stoppingDistance || timerDoneSearch)
            {
                State = FonctionUtulitaire.Patrol;
                StartCoroutine(Patrol());
                break;
            }
            else if (playerInVision)
            {
                State = FonctionUtulitaire.Chase;
                StartCoroutine(Chase());
                break;
            }
        }
        StopCoroutine(timer);
        searchCoroutineIsRunning = false;
    }

    IEnumerator SearchTimer()
    {
        yield return new WaitForSeconds(enemyData.patrolTime);
        if (State == FonctionUtulitaire.Searching)
        {
            timerDoneSearch = true;
        }
        else
        {
            Debug.Log("Reached Dest");
        }
    }

    #endregion
}
