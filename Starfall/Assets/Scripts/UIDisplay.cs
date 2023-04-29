using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Powers;

public class UIDisplay : MonoBehaviour
{
    
    [Header("Health")]
    [SerializeField] Slider healthSlider;
    private Health _playerHealth;
    
    [Header("Energy")]
    [SerializeField] Slider energySlider;
    private Energy _playerEnergy;

    [Header("Score")]
    [SerializeField] TextMeshProUGUI scoreText;
    private ScoreKeeper _scoreKeeper;

    void Awake()
    {
        _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        _playerHealth = FindObjectOfType<Health>();
        _playerEnergy = FindObjectOfType<Energy>();
    }

    void Start()
    {
        healthSlider.maxValue = _playerHealth.GetCurrentHealth();
        energySlider.maxValue = _playerEnergy.CurrentEnergy;

    }

    void Update()
    {
        healthSlider.value = _playerHealth.GetCurrentHealth();
        energySlider.value = _playerEnergy.CurrentEnergy;
        scoreText.text = _scoreKeeper.GetScore().ToString("000000000");
    }

}
