using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Damage : MonoBehaviour
{
    enum DamageType
    {
        Moving,
        Stationary,
        Melee
    }

    [SerializeField] DamageType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] EnemyAI meleeAI;
    [SerializeField] bool isLava;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] float damageDelay;

    float localDamageDelay;

    public bool isAttacking;

    void Start()
    {
        isAttacking = false;
        rb.linearVelocity = transform.forward * speed;
        if (type == DamageType.Moving)
        {
            Destroy(gameObject, destroyTime);
        }
        localDamageDelay = damageDelay;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.isTrigger))
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            if (type == DamageType.Melee)
            {
                if (meleeAI != null)
                {
                    if (meleeAI.isMelee)
                        dmg.TakeDamage(damageAmount);
                }
            }
            else
            {
                dmg.TakeDamage(damageAmount);
            }
        }

        if (type == DamageType.Moving)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && isLava == true)
        {
            if (localDamageDelay <= 0)
            {
                dmg.TakeDamage(damageAmount);
                localDamageDelay = damageDelay;
            }
            else
            {
                localDamageDelay -= Time.deltaTime;
            }
        }
    }
}
