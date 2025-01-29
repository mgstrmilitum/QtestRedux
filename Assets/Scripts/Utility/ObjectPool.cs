using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private PoolableObject prefab;
    private int size;
    private List<PoolableObject> availableObjs;

    private ObjectPool(PoolableObject _prefab, int _size)
    {
        prefab = _prefab;
        size = _size;
        availableObjs = new List<PoolableObject>(size);
    }

    public static ObjectPool CreateInstance(PoolableObject _prefab, int _size)
    {
        ObjectPool pool = new ObjectPool(_prefab, _size);

        GameObject poolObject = new GameObject(_prefab.name + " pool");
        pool.CreateObjects(poolObject.transform, _size);

        return pool;
    }

    private void CreateObjects(Transform parent, int size)
    {
        for(int i = 0; i < size; ++i)
        {
            PoolableObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent.transform);
            obj.Parent = this;
            obj.gameObject.SetActive(false);
        }
    }

    public void ReturnObjectToPool(PoolableObject poolableObject)
    {
        availableObjs.Add(poolableObject);
    }

    public PoolableObject GetObject()
    {
        PoolableObject instance = null;
        if (availableObjs.Count > 0)
        {
            instance = availableObjs[0];
            availableObjs.RemoveAt(0);

            instance.gameObject.SetActive(true);
        }

        return instance;
    }
}
