using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Damage : MonoBehaviour
{
    enum DamageType
    {
        Moving,
        Stationary
    }

    [SerializeField] DamageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    public bool isAttacking;

    void Start()
    {
        isAttacking = false;
        rb.linearVelocity = transform.forward * speed;
        if (type == DamageType.Moving)
        {
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.isTrigger))
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if(dmg != null)
        {
            dmg.TakeDamage(damageAmount);
        }

        if(type == DamageType.Moving)
        {
            Destroy(gameObject);
        }
    }
}
