using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] int health;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] Transform shotPosition;
    [SerializeField] GameObject bullet;
    [SerializeField] float rateOfFire;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDist;
    [SerializeField] Transform headPos;

    Color origColor;
    bool isShooting;
    bool playerInRange;
    bool enemyInRange;
    bool isRoaming;
    float stoppingDistanceOrig;
    
    Vector3 startingPos;
    Vector3 playerDirection;
    Vector3 enemyDirection;

    Coroutine co;

    // Start is called before the first frame update
    void Start()
    {
        origColor = model.material.color;
        GameManager.Instance.UpdateGameGoal(1);
        stoppingDistanceOrig = agent.stoppingDistance;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            enemyDirection = GameManager.Instance.player.transform.position - transform.position;
            agent.SetDestination(GameManager.Instance.player.transform.position);

            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }

            if (!isShooting)
            {
                StartCoroutine(Shoot());
            }

            if (playerInRange && !canSeePlayer())
             {
                if(!isRoaming && agent.remainingDistance < 0.01f)
                {
                    co = StartCoroutine(EnemyRoam());
                }

             }
             else if (!playerInRange)
             {
                if(!isRoaming && agent.remainingDistance < 0.01f)
                {
                    co = StartCoroutine(EnemyRoam());
                }
             }
        }


        if (enemyInRange)
        {
            //Probs gotta tune gameManager to be able to recognize an enemy.
            playerDirection = GameManager.Instance.player.transform.position - transform.position;
            agent.SetDestination(GameManager.Instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }

            if (!isShooting)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    IEnumerator EnemyRoam()
    {
        isRoaming = true;

        yield return new WaitForSeconds(roamPauseTime);

        agent.stoppingDistance = 0;

        Vector3 randomPos = Random.insideUnitSphere * roamDist;
        randomPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);

        isRoaming = false;
    }

    bool canSeePlayer()
    {
        playerDirection = GameManager.Instance.player.transform.position - headPos.position;
        agent.SetDestination(GameManager.Instance.player.transform.position);

        RaycastHit hit;
        if(Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            if(hit.collider.CompareTag("Player"))
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
                if (!isShooting)
                {
                    StartCoroutine(Shoot());
                }

                return true;
            }
        }       
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
            agent.stoppingDistance = 0;
        }

        if (other.CompareTag("Enemy"))
        {
            enemyInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }

        if (other.CompareTag("Enemy"))
        {
            enemyInRange = false;
        }
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        StartCoroutine(FlashRed());

        agent.SetDestination(GameManager.Instance.player.transform.position);

        if (co != null)
        {
            StopCoroutine(co);
            isRoaming = false;
        }

        if (health <= 0)
        {
            GameManager.Instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        model.material.color = origColor;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shotPosition.position, transform.rotation);
        yield return new WaitForSeconds(rateOfFire);
        isShooting = false;
    }
}
