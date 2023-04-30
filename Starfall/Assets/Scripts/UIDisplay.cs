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

    [Header("Bombs")]
    [SerializeField] TextMeshProUGUI bombText;
    private BombManager _bombManager;
    
    private void Awake()
    {
        _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        _playerHealth = FindObjectOfType<Health>();
        _playerEnergy = FindObjectOfType<Energy>();
        _bombManager = FindObjectOfType<BombManager>();

    }

    private void Start()
    {
        healthSlider.maxValue = _playerHealth.CurrentHealth;
        energySlider.maxValue = _playerEnergy.CurrentEnergy;
    }

    private void Update()
    {
        healthSlider.value = _playerHealth.CurrentHealth;
        energySlider.value = _playerEnergy.CurrentEnergy;
        scoreText.text = _scoreKeeper.GetScore().ToString("000000000");
        bombText.text = "* " + _bombManager.CurrentBombs.ToString();
    }

}
