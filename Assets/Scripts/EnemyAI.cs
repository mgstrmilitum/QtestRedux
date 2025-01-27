using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Photon.Pun.UtilityScripts.PunTeams;

/*GENERAL COMMENTS*/
//-------------------
// -Hellhounds Should Probably be another script.
// -Should be Up to date with Lecture 2 so far -DY
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
    
    // Position of player.
    Vector3 playerPosition;

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
            TargetPlayer();
        }
    }
    
    // Detects Player when he enters the Sphere Collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Sphere Collider Exit
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    //Targerts the Player and shoots.
    private void TargetPlayer()
    {
        playerPosition = GameManager.Instance.player.transform.position;
        agent.SetDestination(playerPosition - transform.position);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            FaceTarget();
        }

        if (!isShooting)
        {
            StartCoroutine(Shoot());
        }
    }

    // Faces the Target
    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerPosition);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    // Takes Damage
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

    // Flashes Red
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        model.material.color = origColor;
    }

    // Shoots
    IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shotPosition.position, transform.rotation);
        yield return new WaitForSeconds(rateOfFire);
        isShooting = false;
    }
}
