using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTest : MonoBehaviour
{
    public static event Action DecreasePlayerHealth;
    public static event Action IncreasePlayerCoins;
    public event Action DecreaseEnemyHealth;

    [SerializeField] private Image _healthBar;
    [SerializeField] private int _maxHealth = 3;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _colorNormal = Color.white;
    [SerializeField] private Color _colorHurt = Color.red;

    private int _health = 3;
    private float _duration;
    private List<Transform> _waypoints;
    private float _startTime;
    private float _pathTotalDistance;
    
    
    private void OnEnable()
    {
        ResetEnemy();

        DecreaseEnemyHealth += DecreaseHealth;
    }

    private void OnDisable()
    {
        DecreaseEnemyHealth -= DecreaseHealth;
    }

    private void Start()
    {
        _startTime = Time.time;
        
        _waypoints = EnemySpawner.Instance.Waypoints;
        _pathTotalDistance = EnemySpawner.Instance.GetTotalLength();

        transform.position = _waypoints[0].position;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        //movement will be calculated from the duration and distance
        
        float travelTime = Time.time - _startTime; //gets how long this has been moving

        _duration = EnemySpawner.Instance.enemyMoveDuration; // gets the duration from spawner
        
        if (travelTime <= _duration)
        {
            //will calculate based on the time taken
            //this will make the enemy move at a constant speed
            //the speed is determined by _pathTotalDistance / _duration
            
            float distanceTraveled = travelTime / _duration * _pathTotalDistance;  

            float currentTotalSegmentLength = 0f;
            
            for (int i = 0; i < _waypoints.Count - 1; i++)
            {
                float currentSegmentLength = Vector3.Distance(_waypoints[i].position, _waypoints[i + 1].position);
                
                
                if (distanceTraveled < (currentTotalSegmentLength + currentSegmentLength))
                {
                    float segmentInterpolation = (distanceTraveled - currentTotalSegmentLength) / currentSegmentLength;
                    transform.position = Vector2.Lerp(_waypoints[i].position, _waypoints[i + 1].position, segmentInterpolation);
                    
                    // Vector2 basePosition = transform.position = Vector2.Lerp(_waypoints[i].position, _waypoints[i + 1].position, segmentInterpolation);
                    //
                    // // Add a sine wave to the movement
                    // _sineAngle += Time.deltaTime * _sineSpeed;
                    // Vector2 randomMovement = new Vector2(Mathf.Sin(_sineAngle) * _sineLength, Mathf.Cos(_sineAngle) * _sineLength);
                    // transform.position = basePosition + randomMovement;
                    
                    break;
                }
                
                currentTotalSegmentLength += currentSegmentLength;
            }
        }
        else
        {
            //if enemy reaches the last waypoint
            DecreasePlayerHealth?.Invoke();
            gameObject.SetActive(false);
        }
        
    }

    
    private void ResetEnemy()
    {
        _startTime = Time.time;
        
        if (EnemySpawner.Instance.Waypoints != null) _waypoints = EnemySpawner.Instance.Waypoints;
        _pathTotalDistance = EnemySpawner.Instance.GetTotalLength();

        if(_waypoints != null) transform.position = _waypoints[0].position;

        _health = _maxHealth;
        _spriteRenderer.color = _colorNormal;
        UpdateHealthBar(_health, _maxHealth);
    }

    private void DecreaseHealth()
    {
        _health--;
        StartCoroutine(ColorTick());
        UpdateHealthBar( (float)_health, (float)_maxHealth);

        if (_health <= 0)
        {
            gameObject.SetActive(false);
            ResetEnemy();
            IncreasePlayerCoins?.Invoke();
        }
        
    }

    private void UpdateHealthBar(float currentVal, float maxVal)
    {
        _healthBar.fillAmount = currentVal / maxVal;
        _healthBar.color = Color.Lerp(Color.red, Color.green, currentVal / maxVal);
    }

    public void HealthTrigger()
    {
        //this will be called on Turret
        DecreaseEnemyHealth?.Invoke();
    }

    IEnumerator ColorTick()
    {
        _spriteRenderer.color = _colorHurt;
        yield return new WaitForSeconds(.075f);
        _spriteRenderer.color = _colorNormal;
    }
    
}
