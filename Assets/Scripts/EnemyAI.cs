using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Photon.Pun.UtilityScripts.PunTeams;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] int health;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] Transform shotPosition;
    [SerializeField] GameObject bullet;
    [SerializeField] float rateOfFire;
    [SerializeField] int team;

    Color origColor;
    bool isShooting;
    bool playerInRange;
    bool enemyInRange;

    Vector3 playerDirection;
    Vector3 enemyDirection;

    // Roaming
    public Vector3 walkPoint;
    bool walkPointSet;
    public float rangeToWalkPoint;
    LayerMask ground;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        origColor = model.material.color;
        GameManager.Instance.UpdateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            enemyDirection = GameManager.Instance.player.transform.position;

            //if (agent.remainingDistance <= agent.stoppingDistance)
            //{
            //    FaceTarget();
            //}

            //if (!isShooting)
            //{
            //    StartCoroutine(Shoot());
            //}

            TargetPlayerOrEnemy();
        }
        else if (enemyInRange)
        {
            enemyDirection = GameManager.Instance.player.transform.position;

            //Probs gotta tune gameManager to be able to recognize an enemy.
            //playerDirection = GameManager.Instance.player.transform.position - transform.position;

            //agent.SetDestination(GameManager.Instance.player.transform.position);

            //if (agent.remainingDistance <= agent.stoppingDistance)
            //{
            //    FaceTarget();
            //}

            //if (!isShooting)
            //{
            //    StartCoroutine(Shoot());
            //}

            TargetPlayerOrEnemy();
        }
        //else
        //{
        //    Roam();
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
        else if (other.CompareTag("Enemy"))
        {
            EnemyAI otherEnemy = other.GetComponent<EnemyAI>();
            if (otherEnemy != null && otherEnemy.team != team) { enemyInRange = true; }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
        else if (other.CompareTag("Enemy"))
        {
            EnemyAI otherEnemy = other.GetComponent<EnemyAI>();
            if (otherEnemy != null && otherEnemy.team != team) { enemyInRange = false; }
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

        yield return new WaitForSeconds(0.15f);

        model.material.color = origColor;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shotPosition.position, transform.rotation);
        yield return new WaitForSeconds(rateOfFire);
        isShooting = false;
    }

    public void Roam()
    {
        if (!walkPointSet) SearchForWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude <= 1f)
            walkPointSet = false;

    }

    private void SearchForWalkPoint()
    {
        float randX = UnityEngine.Random.Range(-rangeToWalkPoint, rangeToWalkPoint);
        float randZ = UnityEngine.Random.Range(-rangeToWalkPoint, rangeToWalkPoint);

        walkPoint = new Vector3(transform.position.x + randX, transform.position.y, transform.position.z + randZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, ground))
            walkPointSet = true;
    }

    private void TargetPlayerOrEnemy()
    {
        agent.SetDestination(enemyDirection - transform.position);

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
