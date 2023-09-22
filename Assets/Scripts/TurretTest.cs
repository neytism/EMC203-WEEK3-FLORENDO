using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

public class TurretTest : MonoBehaviour
{
    public static event Action<int> DecreaseCoins;

    public bool enableDebug = true;
    [SerializeField] private float _range = 1f;
    [SerializeField] private float _fireRate = 1f;
    
    [SerializeField] private float _maxRange = 8f;
    [SerializeField] private float _maxFireRate = 4f;
    
    [Range(0f, 360f)]
    [SerializeField] private float _viewAngleRange = 10f;
    
    [SerializeField] private int _rangeUpgradeCost = 10;
    [SerializeField] private int _fireRateUpgradeCost = 15;
    
    [SerializeField] private float _turnSpeed = 5f;
    [SerializeField] private GameObject _target;
    
    [SerializeField] private float shotsInterval;
    [SerializeField] private Transform firePoint;

    [SerializeField] private float randomTargetChangeInterval;
    [SerializeField] private Vector2 randomAngle;
    
    private GameObject[] enemies;
    
    [SerializeField] private TextMeshProUGUI _fireRateText;
    [SerializeField] private TextMeshProUGUI _rangeText;
    
    [SerializeField] private GameObject _bulletPrefab;

    private void Start()
    {
        UpdateUI();
        randomTargetChangeInterval = 0f;  
    }

    private void Update()
    {
        UpdateTarget();
        
        if (_target == null)
        {
            RotateTowardsRandom();
        }
        else
        {
            RotateTowardsTarget();
        }
        
    }

    private void UpdateTarget()
    {
        float shortestDistance = _range;
        enemies = GameObject.FindGameObjectsWithTag("Enemy"); //will get all enemies with tag
        
        foreach (var enemy in enemies)
        {
            float distanceToEnemy = Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                _target = enemy; //will set nearest as target
            }
        }

        if(_target == null) return;
        
        if (Distance(_target.transform.position, transform.position) > _range)
        {
            _target = null;
        }
        
    }

    private void RotateTowardsTarget()
    {

        if (Distance(transform.position, _target.transform.position) > _range) return;
        
        Fire();
        
        Vector2 directionToPlayer = _target.transform.position - transform.position;
        
        //rotate using Vector3.Slerp
        //transform.up = Vector3.Slerp(transform.up, NormalizeVector(directionToPlayer), _turnSpeed * Time.deltaTime);
        
        //rotate using Quaternion.Slerp
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90;
        Quaternion zRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, zRotation, _turnSpeed * Time.deltaTime);
        randomAngle = transform.up;
    }

    private void RotateTowardsRandom()
    {
        
        if(randomTargetChangeInterval <= 0)
        {
            //rotate using Quaternion.Slerp
            randomAngle = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            randomTargetChangeInterval = Random.Range(2f, 10f);  // random interval seconds
            
        } else
        {
            randomTargetChangeInterval -= Time.deltaTime;
        }
        
        float angle = Mathf.Atan2(randomAngle.y, randomAngle.x) * Mathf.Rad2Deg - 90;
        Quaternion zRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, zRotation, (_turnSpeed / 10) * Time.deltaTime);
    }
    
    private void Fire()
    {
        if(IsTargetLocked()) return;
        
        if(shotsInterval <= 0)
        {
            GameObject bullet = ObjectPool.Instance.PoolObject(_bulletPrefab, firePoint.position);
            bullet.GetComponent<BulletTest>().InitializeBullet(firePoint.up);
            bullet.SetActive(true);
            
            shotsInterval = 1f / _fireRate;  // adds interval between shots,, calculated from fire rate
            
        } else
        {
            shotsInterval -= Time.deltaTime;
        }
        
    }

    private bool IsTargetLocked()
    {
        Vector2 directionToPlayer = _target.transform.position - transform.position;
        float dotProduct = DotProduct(NormalizeVector(directionToPlayer), transform.up);

        return dotProduct < ConvertViewAngle(_viewAngleRange);
    }
    
    private float Distance(Vector3 firstPos, Vector3 secondPos)
    {
        float xDifference = firstPos.x - secondPos.x;
        float yDifference = firstPos.y - secondPos.y;
        float zDifference = firstPos.z - secondPos.z;
        
        return Mathf.Sqrt(Mathf.Pow((xDifference), 2) + Mathf.Pow((yDifference), 2) + Mathf.Pow((zDifference), 2));
    }

    private float DotProduct(Vector3 firstPos, Vector3 secondPos)
    {
        float xProduct = firstPos.x * secondPos.x;
        float yProduct = firstPos.y * secondPos.y;
        float zProduct = firstPos.z * secondPos.z;
        
        return xProduct + yProduct + zProduct;
    }

    private float Magnitude(Vector3 v)
    {
        return Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.y, 2) + Mathf.Pow(v.z, 2));
    }

    private Vector3 NormalizeVector(Vector3 v)
    {
        float mag = Magnitude(v);

        v.x /= mag;
        v.y /= mag;
        v.z /= mag;
        
        return v;
    }
    
    private void OnDrawGizmos()
    {
        if (!enableDebug) return;
        
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, _range);
        
        if (_target == null ) return;
        
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.9f);
        Gizmos.DrawLine(transform.position, _target.transform.position);
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.9f);
        Gizmos.DrawLine(transform.position, transform.position + ( transform.up * _range));
        Gizmos.DrawLine(transform.position, transform.position - ( transform.up * _range));
        
        Gizmos.color = new Color(0.0f, 1.0f, 1.0f, 0.9f);
        
        Vector3 trianglePoint = _target.transform.position + Vector3.Project((transform.position + transform.forward) - _target.transform.position, transform.right);
        
        Gizmos.DrawLine(_target.transform.position, trianglePoint);

    }

    private float ConvertViewAngle(float angle)
    {
        return Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad);
    }

    private void UpdateUI()
    {
        _rangeText.text = _range >= _maxRange ? $"Range: MAX" : $"Range: {_range}";
        _fireRateText.text = _fireRate >= _maxFireRate ? $"Fire Rate: MAX" : $"Fire Rate: {_fireRate}";
    }

    
    public void UpgradeRange()
    {
        if (GameManager.Instance.GetCoinCount < _rangeUpgradeCost) return;
        
        if (_range >= _maxRange) return;
        
        _range++;
        DecreaseCoins?.Invoke(_rangeUpgradeCost);
        if (_range >= _maxRange)
        {
            _range = _maxRange;
        }
        UpdateUI();
    }

    public void UpgradeFireRate()
    {
        if (GameManager.Instance.GetCoinCount < _fireRateUpgradeCost) return;
        if (_fireRate >= _maxFireRate) return;
        
        _fireRate++;
        DecreaseCoins?.Invoke(_fireRateUpgradeCost);
        if (_fireRate >= _maxFireRate)
        {
            _fireRate = _maxFireRate;
        }
        UpdateUI();
    }
}
