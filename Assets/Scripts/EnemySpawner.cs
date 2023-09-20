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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    #endregion
    
    [SerializeField] private float _enemySpawnRate = 2f;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform waypointHolder;
    [SerializeField] private int _maxNumberOfEnemiesSpawned = 10;
    [SerializeField] private TextMeshProUGUI _waveNumberText;
    [SerializeField] private GameObject _startWaveButton;
    
    public float enemyMoveDuration = 10f;
    private float _timer;
    
    private List<Transform> waypoints;
    public List<Transform> Waypoints => waypoints;
    
    private int _numberOfEnemiesSpawned = 0;
   
    private int _waveNumber = 0;
    private bool _canSpawnEnemy = false;
    
    private GameObject[] enemies;
   

    private void Start()
    {
        waypointHolder = GameObject.FindWithTag("WaypointsHolder").transform;
        InitializeWaypoints();
        StartWave();
        SpawnEnemy();
        UpdateUI();
    }

    private void InitializeWaypoints()
    {
        //initialize waypoints on awake
        waypoints = new List<Transform>();
        
        foreach (Transform child in waypointHolder)
        {
            waypoints.Add(child);
        }
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

    // Update is called once per frame
    void Update()
    {
        CheckAliveEnemies();
            
        if (Time.time < _timer + 1f/_enemySpawnRate) return; //spawn rate
        
        SpawnEnemy();

        _timer = Time.time;
    }

    private void SpawnEnemy()
    {
        if (_enemyPrefab == null) return;
        
        if(!_canSpawnEnemy) return;

        GameObject enemy = ObjectPool.Instance.PoolObject(_enemyPrefab, waypoints[0].position); //object pooling 
        enemy.SetActive(true);

        _numberOfEnemiesSpawned++;
        
        if (_numberOfEnemiesSpawned >= _maxNumberOfEnemiesSpawned)
        {
            _canSpawnEnemy = false;
            //will have a max number of enemies to be spawned per wave
            //if it meets max, stop spawning
        }
    }

    private void UpdateUI()
    {
        _waveNumberText.text = $"Wave {_waveNumber}";
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
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
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
                _startWaveButton.SetActive(true); //if the code above didnt break, all enemies are dead.
            }
        }
        
    }

    public void StartWave()
    {
        //this is referenced to a button
        
        WaveModifier(); //will modify spawnrate, speed(based on duration), and number of enemies needed
        
        _numberOfEnemiesSpawned = 0; //resets the enemy counter
        
        _canSpawnEnemy = true; // allows apawning
        
        UpdateUI(); // upd.. yes
        
        _startWaveButton.SetActive(false); // hides button

    }
    
}
