using System;
using System.Collections;
using TMPro;
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
    
    
    [SerializeField] private int _health = 10;
    [SerializeField] private int _coins = 0;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _coinText;

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
        _healthText.text = $"Health: {_health}";
        _coinText.text = $"Coins: {_coins}";
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


    IEnumerator GameOverCoroutine()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
