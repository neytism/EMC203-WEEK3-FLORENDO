using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Instance

    private static GameManager _instance;
    public static GameManager Instance
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

    public event Action<int, int> UpdateUITrigger;
    
    [SerializeField] private int _health = 10;
    [SerializeField] private int _coins = 0;

    public int GetCoinCount => _coins;

    private void Start()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        EnemyTest.DecreasePlayerHealth += DecreaseHealth;
        EnemyTest.IncreasePlayerCoins += IncreaseCoinCount;
        TurretTest.DecreaseCoins += DecreaseCoinCount;
    }

    private void OnDisable()
    {
        EnemyTest.DecreasePlayerHealth -= DecreaseHealth;
        EnemyTest.IncreasePlayerCoins -= IncreaseCoinCount;
        TurretTest.DecreaseCoins -= DecreaseCoinCount;
    }

    private void UpdateUI()
    {
       UpdateUITrigger?.Invoke(_health, _coins);
    }

    private void DecreaseHealth()
    {
        _health--;

        if (_health <= 0)
        {
            _health = 0;
            
            //Gameover

            StartCoroutine(GameOverCoroutine());
        }

        UpdateUI();
    }

    private void IncreaseCoinCount()
    {
        _coins++;
        
        UpdateUI();
    }

    private void DecreaseCoinCount(int value)
    {
        _coins -= value;
        UpdateUI();
    }

    public void ChangeTimeScale()
    {
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 2f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void Restart()
    {
        ObjectPool.Instance.DisposeAll();
        EnemySpawner.Instance.ResetSpawner();
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        _health = 10;
        _coins = 0;
        SceneManager.LoadScene(currentScene.name);
    }

    IEnumerator GameOverCoroutine()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f);
        Restart();
        

    }
    
}
