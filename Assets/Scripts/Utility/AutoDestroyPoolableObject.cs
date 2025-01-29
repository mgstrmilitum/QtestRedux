using UnityEngine;

public class AutoDestroyPoolableObject : PoolableObject
{
    public float autoDestroyTime = 5f;
    private const string DisableMethodName = "Disable";

    public virtual void OnEnable()
    {
        CancelInvoke(DisableMethodName);
        Invoke(DisableMethodName, autoDestroyTime);
    }
    public virtual void Disable()
    {
        gameObject.SetActive(false);
    }
}
