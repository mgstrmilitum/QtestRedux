using NUnit.Framework;
using System;
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
        
        float thresholdTime = Time.timeSinceLevelLoad - launchDelay;

        foreach(var kvp in targets)
        {
            if(kvp.Value.contactTime >= thresholdTime)
            {
                Launch(kvp.Key);
                targetsToClear.Add(kvp.Key);
            }
        }

        foreach(var target in targetsToClear)
        {
            targets.Remove(target);
        }

        targetsToClear.Clear();
    }

    private void Launch(Rigidbody key)
    {
       key.AddForce(transform.up * launchForce, LaunchMode);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb;
        if (collision.gameObject.TryGetComponent<Rigidbody>(out rb))
        {
            targets[rb] = new JumpPadTarget() { contactTime = Time.timeSinceLevelLoad };
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }
}
