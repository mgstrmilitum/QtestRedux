using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] int health;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int fieldOfView;
    [SerializeField] Transform shotPosition, headPosition;
    [SerializeField] GameObject bullet;
    [SerializeField] float rateOfFire;
    [SerializeField] int animTransSpeed;

    float angleToPlayer;
    Color origColor;
    bool isShooting;
    bool playerInRange;

    Vector3 playerDirection;

    // Start is called before the first frame update
    void Start()
    {
        origColor = model.material.color;
        GameManager.Instance.UpdateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, animTransSpeed * Time.deltaTime));
        if (playerInRange && CanSeePlayer())
        {
            

        }
    }

    bool CanSeePlayer()
    {
        playerDirection = GameManager.Instance.player.transform.position - headPosition.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);
        Debug.DrawRay(headPosition.position, playerDirection);

        RaycastHit hit;
        if (Physics.Raycast(headPosition.position, playerDirection, out hit))
        {
            if(hit.collider.CompareTag("Player") && angleToPlayer <= fieldOfView)
            {
                agent.SetDestination(GameManager.Instance.player.transform.position);

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, 0f, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        StartCoroutine(FlashRed());

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
