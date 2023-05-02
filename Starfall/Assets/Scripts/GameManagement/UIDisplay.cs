using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Powers;
using Items;

namespace GameManagement
{
    public class UIDisplay : MonoBehaviour
    {
    
        [Header("Health")]
        [SerializeField] private Slider healthSlider;
        private Health _playerHealth;
    
        [Header("Energy")]
        [SerializeField] private Slider energySlider;
        private Energy _playerEnergy;

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI scoreText;
        private ScoreKeeper _scoreKeeper;

        [Header("Coins")]
        [SerializeField] private TextMeshProUGUI coinText;
        private CoinManager _coinManager;
        
        [Header("Bombs")]
        [SerializeField] private TextMeshProUGUI bombText;
        private BombManager _bombManager;
    
        private void Awake()
        {
            _scoreKeeper = FindObjectOfType<ScoreKeeper>();
            _playerHealth = FindObjectOfType<Health>();
            _playerEnergy = FindObjectOfType<Energy>();
            _bombManager = FindObjectOfType<BombManager>();
            _coinManager = FindObjectOfType<CoinManager>();
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
            coinText.text = "* " + _coinManager.CurrentCoins.ToString();
        }

    }
}