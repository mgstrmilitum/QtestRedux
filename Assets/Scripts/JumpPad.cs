using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    struct JumpPadTarget
    {
        public float contactTime;
    }

    [SerializeField] float launchDelay;
    [SerializeField] float launchForce;
    [SerializeField] ForceMode LaunchMode = ForceMode.Impulse;

    Dictionary<Rigidbody, JumpPadTarget> targets = new Dictionary<Rigidbody, JumpPadTarget>();
    List<Rigidbody> targetsToClear = new List<Rigidbody>();

    private void FixedUpdate()
    {
        Debug.DrawRay(this.transform.position, this.transform.up * 3f, Color.red);
        //why fixed update versus update?
        float thresholdTime = Time.timeSinceLevelLoad - launchDelay;

        foreach(var kvp in targets)
        {
            if(kvp.Value.contactTime >= thresholdTime)
            {
                
                Launch(kvp.Key);
                targetsToClear.Add(kvp.Key);
            }
        }
        //ADD RUNPADS IN ADDITION TO JUMP/VERT PAD
        foreach(var target in targetsToClear)
        {
            targets.Remove(target);
        }

        targetsToClear.Clear();
    }

    private void Launch(Rigidbody key)
    {

        key.constraints = RigidbodyConstraints.None;
        key.AddForce(this.transform.up * launchForce, LaunchMode);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        Rigidbody rb;
        if (collision.gameObject.TryGetComponent<Rigidbody>(out rb))
        {
            targets[rb] = new JumpPadTarget() { contactTime = Time.timeSinceLevelLoad };
        }
        Rigidbody trigRb = collision.rigidbody;
        trigRb.constraints = RigidbodyConstraints.FreezeAll;
        collision.transform.SetParent(this.transform);
        //StartCoroutine(PrepLaunch(collision.gameObject));
    }

    private void OnCollisionExit(Collision collision)
    {
        collision.transform.SetParent(null);
    }

    IEnumerator PrepLaunch(GameObject obj)
    {
        bool isPlayer = false;
        bool isEnemy = false;
        if(obj.CompareTag("Player"))
        {
            isPlayer = true;
            //GameManager.Instance.playerScript.controller.enabled = false;
        }
        else if(obj.CompareTag("Enemy"))
        {
            isEnemy = true;
            obj.GetComponent<EnemyAI>().agent.enabled = false;
        }
        yield return new WaitForSeconds(1.5f);

        if (isPlayer)
        {
           // GameManager.Instance.playerScript.controller.enabled = true;
        }
        else if(isEnemy)
        {
            obj.GetComponent<EnemyAI>().agent.enabled = true;
        }

    }
}
