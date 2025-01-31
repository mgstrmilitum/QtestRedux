using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage, IOpen
{
    enum EnemyType
    {
        Standard,
        Grenade,
        Shotgun,
        Melee
    }

    [SerializeField] int hp;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] float meleeRate;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int fov;
    [SerializeField] int roamPauseTime;
    [SerializeField] Transform headPos;
    [SerializeField] Animator animatorController;
    [SerializeField] int animSpeedTrans;
    [SerializeField] int roamDistance;

    float angleToPlayer;
    float stoppingDistanceOrig;
    bool isShooting;
    public bool isMelee;
    bool playerInRange;
    bool isRoaming;
    Color originalColor;
    Vector3 playerDirection;
    Vector3 startingPos;

    Coroutine co;

    [SerializeField]
    EnemyType enemyType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch(enemyType)
        {
            case EnemyType.Standard:
                break;
            case EnemyType.Grenade:
                model.material.color = Color.blue;
                break;
            case EnemyType.Shotgun:
                model.material.color = Color.black;
                break;
        }
        //COMMENT THIS LINE OUT ONCE SPAWNER IS ADDED
        originalColor = model.material.color;
        stoppingDistanceOrig = agent.stoppingDistance;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = animatorController.GetFloat("Speed");

        animatorController.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * animSpeedTrans));
        
        //Roam behavior
        if ((playerInRange && !CanSeePlayer()))
        {
            if(!isRoaming && agent.remainingDistance < 0.01f)
            {
                co = StartCoroutine(Roam());
            }

        }
        else if(!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                co = StartCoroutine(Roam());
            }
        }

    }

    IEnumerator Roam()
    {
        Debug.Log("Now roaming!");
        isRoaming = true;

        yield return new WaitForSeconds(roamPauseTime);

        agent.stoppingDistance = 0;

        Vector3 randomPos = Random.insideUnitSphere * roamDistance;
        randomPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDistance, 1);
        agent.SetDestination(hit.position);
        isRoaming = false;
    }

    bool CanSeePlayer()
    {
        playerDirection = GameManager.Instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);
        Debug.DrawRay(headPos.position, playerDirection);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {
                agent.SetDestination(GameManager.Instance.player.transform.position);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }

                if (!isShooting && enemyType != EnemyType.Melee)
                {
                    StartCoroutine(Shoot());

                }
                else if(enemyType == EnemyType.Melee)
                {
                    Debug.Log(agent.remainingDistance);
                    //agent.SetDestination(transform.position);
                    StartCoroutine(Melee());
                }

                agent.stoppingDistance = stoppingDistanceOrig;
                return true;
            }
        }
        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }
    public void TakeDamage(int amount)
    {
        hp -= amount;
        if(enemyType == EnemyType.Melee)
        {
            animatorController.SetTrigger("Damage");
        }
        agent.SetDestination(GameManager.Instance.player.transform.position);
        if(co != null)
        {
            StopCoroutine(co);
            isRoaming = false;
        }

        StartCoroutine(FlashRed());
        if (hp <= 0)
        {
            GameManager.Instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }

    }
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = originalColor;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        GameObject obj = Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator Melee()
    {
        isMelee = true;
        animatorController.SetTrigger("Attack");
        yield return new WaitForSeconds(meleeRate);
        isMelee = false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }
}
