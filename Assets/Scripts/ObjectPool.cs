using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Instance

    private static ObjectPool _instance;
    public static ObjectPool Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null) {
            
            _instance = this;
            
        } else if (_instance != this) {
            
            Destroy (gameObject);
            
        }
 
        DontDestroyOnLoad (gameObject);
    }

    #endregion

    private GameObject _objectToPool;
    private bool _notEnoughObjectsInPool = true;

    public List<GameObject> _objectsPool;
    public List<GameObject> _objectsPoolUI;
    public Transform spawnedObjectsHolder;

    private void OnEnable()
    {
        _objectsPool = new List<GameObject>();
        _objectsPoolUI = new List<GameObject>();
    }

    public GameObject PoolObject(GameObject objectToPool, Vector3 pos)
    {
        _objectToPool = objectToPool;

        if (_objectsPool.Count > 0)
        {
            for (int i = 0; i < _objectsPool.Count; i++)
            {
                //Names of objects MustNotHaveSpaces
                if (!_objectsPool[i].activeInHierarchy && _objectsPool[i].name.StartsWith(_objectToPool.name))
                {
                    _objectsPool[i].transform.position = pos;
                    return _objectsPool[i];
                }
            }
        }


        if (_notEnoughObjectsInPool)
        {
            CreateObjectParentIfNeeded();
            
            GameObject obj = Instantiate(_objectToPool, pos, Quaternion.identity);
            obj.name = _objectToPool.name + "_" + _objectsPool.Count + "_Pooled";
            obj.transform.SetParent(spawnedObjectsHolder);
            obj.SetActive(false);
            _objectsPool.Add(obj);
            
            CreateSubParents(); //might be too complicated to run, but using this makes the scene clean
            
            return obj;
        }

        return null;
    }
    
    public GameObject PoolObject(GameObject objectToPool, Vector3 pos, Quaternion rot)
    {
        _objectToPool = objectToPool;

        if (_objectsPool.Count > 0)
        {
            for (int i = 0; i < _objectsPool.Count; i++)
            {
                //Names of objects MustNotHaveSpaces
                if (!_objectsPool[i].activeInHierarchy && _objectsPool[i].name.StartsWith(_objectToPool.name))
                {
                    _objectsPool[i].transform.position = pos;
                    _objectsPool[i].transform.rotation = rot;
                    return _objectsPool[i];
                }
            }
        }


        if (_notEnoughObjectsInPool)
        {
            CreateObjectParentIfNeeded();
            
            GameObject obj = Instantiate(_objectToPool, pos, rot);
            obj.name = _objectToPool.name + "_" + _objectsPool.Count + "_Pooled";
            obj.transform.SetParent(spawnedObjectsHolder);
            obj.SetActive(false);
            _objectsPool.Add(obj);
            
            CreateSubParents(); //might be too complicated to run, but using this makes the scene clean
            
            return obj;
        }

        return null;
    }

    public GameObject PoolObjectUI(GameObject objectToPool, Transform pos)
    {
        //this function is for UI loading
        _objectToPool = objectToPool;

        if (_objectsPoolUI.Count > 0)
        {
            for (int i = 0; i < _objectsPoolUI.Count; i++)
            {
                //MUST SET PROPER TAGS PER PREFAB IN UNITY EDITOR
                if (!_objectsPoolUI[i].activeInHierarchy && _objectsPoolUI[i].CompareTag(_objectToPool.tag))
                {
                    _objectsPoolUI[i].transform.position = transform.position;
                    return _objectsPoolUI[i];
                }
            }
        }


        if (_notEnoughObjectsInPool)
        {
            CreateObjectParentIfNeeded();

            GameObject obj = Instantiate(_objectToPool, pos);
            _objectsPoolUI.Add(obj);
            return obj;
        }

        return null;
    }


    private void CreateObjectParentIfNeeded()
    {
        //creates object to parent pooled objects to avoid messy scene...

        if (spawnedObjectsHolder == null)
        {
            string name = "ObjectPoolHolder";
            var parentObject = GameObject.Find(name);
            if (parentObject != null)
            {
                spawnedObjectsHolder = parentObject.transform;
            }
            else
            {
                spawnedObjectsHolder = new GameObject(name).transform;
            }

        }
    }
    
    public void CreateSubParents()
    {
        Dictionary<string, Transform> subParents = new Dictionary<string, Transform>();

        foreach (GameObject obj in _objectsPool)
        {
            if (!subParents.ContainsKey(obj.tag))
            {
                string name = obj.tag + "_Parent";
                var parentObject = GameObject.Find(name);
                if (parentObject == null)
                {
                    parentObject = new GameObject(name);
                }
                parentObject.transform.SetParent(spawnedObjectsHolder);
                subParents[obj.tag] = parentObject.transform;
            }

            obj.transform.SetParent(subParents[obj.tag]);
        }
    }


    public void Dispose(List<GameObject> pool)
    {
        foreach (var t in pool)
        {
            Destroy(t);
        }
    }

    public void DisposeAll()
    {
        foreach (var t in _objectsPool)
        {
            Destroy(t);
        }
        
        _objectsPool.Clear();
    }
}