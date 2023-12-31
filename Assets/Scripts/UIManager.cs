using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static event Action StartNextWaveButtonTrigger;

    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _waveNumberText;
    
    [SerializeField] private GameObject _startWaveButton;

    private void OnEnable()
    {
        GameManager.Instance.UpdateUITrigger += UpdateUICoinHealth;
        EnemySpawner.UpdateUIWaveCount += UpdateUIWaveCount;
        EnemySpawner.UpdateUIWaveButton += UpdateUIWaveButton;
    }

    private void OnDisable()
    {
        GameManager.Instance.UpdateUITrigger -= UpdateUICoinHealth;
        EnemySpawner.UpdateUIWaveCount -= UpdateUIWaveCount;
        EnemySpawner.UpdateUIWaveButton -= UpdateUIWaveButton;
    }
    
    private void UpdateUICoinHealth(int health, int coins)
    {
        _healthText.text = $"Health: {health}";
        _coinText.text = $"Coins: {coins}";
    }

    private void UpdateUIWaveCount(int waveNumber)
    {
        _waveNumberText.text = $"Wave {waveNumber}";
    }

    private void UpdateUIWaveButton(bool b)
    {
        _startWaveButton.SetActive(b);
    }

    public void StartNextWaveButtonClicked()
    {
        StartNextWaveButtonTrigger?.Invoke();
    }


}
