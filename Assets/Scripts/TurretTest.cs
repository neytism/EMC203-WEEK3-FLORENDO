using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class TurretTest : MonoBehaviour
{
    public static event Action<int> DecreaseCoins;

    public bool enableDebug = true;
    [SerializeField] private float _range = 1f;
    [SerializeField] private float _fireRate = 1f;
    
    [SerializeField] private float _maxRange = 8f;
    [SerializeField] private float _maxFireRate = 4f;
    
    [SerializeField] private int _rangeUpgradeCost = 10;
    [SerializeField] private int _fireRateUpgradeCost = 15;
    
    [SerializeField] private float _turnSpeed = 5f;
    [SerializeField] private GameObject _target;
    
    [SerializeField] private float shotsInterval;
    [SerializeField] private Transform firePoint;
    
    [SerializeField] private LineRenderer _lineRenderer;
    
    private GameObject[] enemies;
    
    [SerializeField] private TextMeshProUGUI _fireRateText;
    [SerializeField] private TextMeshProUGUI _rangeText;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        UpdateTarget();
        RotateTowardsTarget();
        
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
        DisplayLaser();

        if (_target == null) return;
        
        if (Distance(transform.position, _target.transform.position) > _range) return;
        
        Fire();
        
        //from old code, need to change according to instructions
        
        Vector2 direction = _target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _turnSpeed * Time.deltaTime);
    }
    
    private void Fire()
    {
        
        if(shotsInterval <= 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.up, _range);

            if (hit.transform != null && hit.transform.CompareTag("Enemy"))
            {
                //incorporate dot product here
                hit.transform.gameObject.GetComponent<EnemyTest>().HealthTrigger();

            }
            
            shotsInterval = 1f / _fireRate;  // adds interval between shots,, calculated from fire rate
            
        } else
        {
            shotsInterval -= Time.deltaTime;
        }
        
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

    private void DisplayLaser()
    {
        if (_target != null)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.SetPositions(new []{firePoint.position, firePoint.position + firePoint.up * Distance(firePoint.position, _target.transform.position)});
        }
        else
        {
            _lineRenderer.enabled = false;
        }
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
