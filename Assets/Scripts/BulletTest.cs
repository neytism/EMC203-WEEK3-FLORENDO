using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest : MonoBehaviour
{
     [SerializeField] private float _bulletLife = 3f;
     public bool enableDebug = false;
     [SerializeField] private float _bulletRange = 1f;
     [SerializeField] private float _bulletSpeed = 1f;
     
     [SerializeField] private TrailRenderer _trailRenderer;
     [SerializeField] private GameObject _particle;

     private Vector3 _direction;
     private EnemyTest _enemyTarget;
     private GameObject[] enemies;
     

    private void OnEnable()
    {
        StartCoroutine(BulletLife());
    }

    private void Update()
    {
        transform.position += _direction * (_bulletSpeed * Time.deltaTime);
        
        UpdateTarget();
        
        if (_enemyTarget == null) return;

        if (Distance(transform.position, _enemyTarget.transform.position) < _bulletRange)
        {
            _enemyTarget.HealthTrigger();
            DestroySelf();
        }
    }
    
    private void UpdateTarget()
    {
        float shortestDistance = _bulletRange;
        enemies = GameObject.FindGameObjectsWithTag("Enemy"); //will get all enemies with tag
        
        foreach (var enemy in enemies)
        {
            float distanceToEnemy = Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                _enemyTarget = enemy.GetComponent<EnemyTest>(); //will set nearest as target
            }
        }

        if(_enemyTarget == null) return;
        
        if (Distance(_enemyTarget.transform.position, transform.position) > _bulletRange)
        {
            _enemyTarget = null;
        }
        
    }

    public void InitializeBullet(Vector3 dir)
    {
        _direction = dir;
    }
    

    IEnumerator BulletLife()
    {
        yield return new WaitForSeconds(_bulletLife);
        DestroySelf();
    }
    
    private void DestroySelf()
    {
        _trailRenderer.Clear();
        GameObject particle = ObjectPool.Instance.PoolObject(_particle, transform.position);
        particle.SetActive(true);
        
        gameObject.SetActive(false);
        StopCoroutine(BulletLife());
    }
    
    private float Distance(Vector3 firstPos, Vector3 secondPos)
    {
        float xDifference = firstPos.x - secondPos.x;
        float yDifference = firstPos.y - secondPos.y;
        float zDifference = firstPos.z - secondPos.z;
        
        return Mathf.Sqrt(Mathf.Pow((xDifference), 2) + Mathf.Pow((yDifference), 2) + Mathf.Pow((zDifference), 2));
    }

    private void OnDrawGizmos()
    {
        if (!enableDebug) return;
        
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, _bulletRange);
    }

    
}
