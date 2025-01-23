using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Photon.Pun.UtilityScripts.PunTeams;

/*GENERAL COMMENTS*/
//-------------------
// -Hellhounds Should Probably be another script.
//-------------------



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
    

    Vector3 playerDirection;
    

    // Roaming
    public Vector3 walkPoint;

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
            playerDirection = GameManager.Instance.player.transform.position;
            TargetPlayer();
        }
        
    }
    
    // Detects Player when he enters the
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
        }
    }

    private void TargetPlayer()
    {
        agent.SetDestination(playerDirection - transform.position);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            FaceTarget();
        }

        if (!isShooting)
        {
            StartCoroutine(Shoot());
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
}
