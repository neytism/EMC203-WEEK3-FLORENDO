using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Instance

    private static EnemySpawner _instance;
    public static EnemySpawner Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    #endregion

    public static event Action<int> UpdateUIWaveCount;
    public static event Action<bool> UpdateUIWaveButton;
    
    [SerializeField] private float _enemySpawnRate = 2f;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _maxNumberOfEnemiesSpawned = 10;

    private Transform waypointHolder;
    
    public float enemyMoveDuration = 10f;
    private float _timer;
    
    private List<Transform> waypoints;
    public List<Transform> Waypoints => waypoints;
    
    private int _numberOfEnemiesSpawned = 0;
   
    private int _waveNumber = 0;
    private bool _canSpawnEnemy = false;
    
    private List<GameObject> enemies;
    private void OnEnable()
    {
        UIManager.StartNextWaveButtonTrigger += StartWave;
    }

    private void OnDisable()
    {
        UIManager.StartNextWaveButtonTrigger -= StartWave;
    }

    private void Start()
    {
        InitializeSpawner();
    }
    
    private void Update()
    {
        CheckAliveEnemies();
            
        if (Time.time < _timer + 1f/_enemySpawnRate) return; //spawn rate
        
        SpawnEnemy();

        _timer = Time.time;
    }


    private void InitializeSpawner()
    {
        waypointHolder = GameObject.FindWithTag("WaypointsHolder").transform;
        
        waypoints = new List<Transform>();
        
        foreach (Transform child in waypointHolder)
        {
            waypoints.Add(child);
        }
        
        GetComponent<DrawLine>().Initialize(waypoints);
        
        StartWave();
        SpawnEnemy();
        UpdateUI();
    }

    public float GetTotalLength()
    {
        float journeyLength = 0f;
        
        if (waypointHolder != null)
        {
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                journeyLength += Vector2.Distance(waypoints[i].transform.position, waypoints[i + 1].transform.position);
            }
        }
        
        return journeyLength;
    }

    
    private void SpawnEnemy()
    {
        if (waypointHolder == null)
        {
            InitializeSpawner();
            return;
        }
        
        if (_enemyPrefab == null) return;
        
        if(!_canSpawnEnemy) return;

        GameObject enemy = ObjectPool.Instance.PoolObject(_enemyPrefab, waypoints[0].position); //object pooling 
        enemy.SetActive(true);

        _numberOfEnemiesSpawned++;
        
        if (_numberOfEnemiesSpawned >= _maxNumberOfEnemiesSpawned)
        {
            _canSpawnEnemy = false;
        }
    }

    private void UpdateUI()
    {
       UpdateUIWaveCount?.Invoke(_waveNumber);
    }

    private void WaveModifier()
    {
        _waveNumber++;
        _enemySpawnRate *= 1.3f;
        enemyMoveDuration *= 0.85f;
        _maxNumberOfEnemiesSpawned = Mathf.RoundToInt(_maxNumberOfEnemiesSpawned * 1.3f);
    }
    
    private void CheckAliveEnemies()
    {
        if(_numberOfEnemiesSpawned >= _maxNumberOfEnemiesSpawned)
        {
            enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy")) ;
            bool allEnemiesDead = true; //assuming all enemies r dead

            foreach (GameObject enemy in enemies)
            {
                if (enemy.activeSelf)
                {
                    allEnemiesDead = false; //if theres an active enemy, of course, all enemies r not dead xd
                    break;
                }
            }

            if (allEnemiesDead)
            {
                UpdateUIWaveButton?.Invoke(true);  //if the code above didnt break, all enemies are dead.
            }
        }
        
    }

    public void StartWave()
    {
        //this is referenced to a button
        
        WaveModifier(); //will modify spawn rate, speed(based on duration), and number of enemies needed
        
        _numberOfEnemiesSpawned = 0; //resets the enemy counter
        
        _canSpawnEnemy = true; // allows apawning
        
        UpdateUI(); // upd.. yes
        
        UpdateUIWaveButton?.Invoke(false);  // hides button

    }

    public void ResetSpawner()
    {
        _enemySpawnRate = 0.5f;
        _maxNumberOfEnemiesSpawned = 15;
        enemyMoveDuration = 40;
        _waveNumber = 0;

        UpdateUIWaveButton?.Invoke(true);
        UpdateUI();

    }
    
}
