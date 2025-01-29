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

    Color origColor;
    bool isShooting;
    bool playerInRange;
    bool enemyInRange;

    Vector3 playerDirection;
    Vector3 enemyDirection;

    // Start is called before the first frame update
    void Start()
    {
        origColor = model.material.color;
      
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
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
