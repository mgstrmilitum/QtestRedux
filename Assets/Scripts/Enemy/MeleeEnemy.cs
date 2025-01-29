using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour, IDamage
{
    [SerializeField] int hp;

    [Header("Combat")]
    [SerializeField] float meleeRate;
    [SerializeField] float meleeRange;
    [SerializeField] float aggroRange;

    [SerializeField] float roamCooldown;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] Transform playerCenter;
    //[SerializeField] int faceTargetSpeed;
    //[SerializeField] int fov;
    //[SerializeField] int roamPauseTime;
    //[SerializeField] Transform headPos;
    [SerializeField] Animator animatorController;
    //[SerializeField] int animSpeedTrans;
    //[SerializeField] int roamDistance;

    //float angleToPlayer;
    //float stoppingDistanceOrig;
    //bool isMelee;
    //bool playerInRange;
    //bool isRoaming;

    float timeElapsed;
    float origRoamCooldown;


    Vector3 playerDirection;
    Vector3 startingPos;

    Coroutine co;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origRoamCooldown = roamCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Vector3.Distance(GameManager.Instance.player.transform.position, transform.position));
        animatorController.SetFloat("Speed", agent.velocity.magnitude / agent.speed);

        if(timeElapsed >= meleeRate)
        {
            if(Vector3.Distance(GameManager.Instance.player.transform.position, transform.position) <= meleeRange)
            {
                animatorController.SetTrigger("Attack");
                timeElapsed = 0f;
            }
        }

        timeElapsed += Time.deltaTime;

        if(roamCooldown <= 0 && Vector3.Distance(GameManager.Instance.player.transform.position, transform.position) <= aggroRange)
        {
            roamCooldown = origRoamCooldown;
            agent.SetDestination(GameManager.Instance.player.transform.position);
        }

        roamCooldown -= Time.deltaTime;
        transform.LookAt(playerCenter);
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        if(hp <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
