using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class UIDisplay : MonoBehaviour
{
    
    [Header("Health")]
    [SerializeField] Slider healthSlider;

    private Health _playerHealth;

    [Header("Score")]
    [SerializeField] TextMeshProUGUI scoreText;
    private ScoreKeeper _scoreKeeper;

    void Awake()
    {
        _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        _playerHealth = FindObjectOfType<Health>();
    }

    void Start()
    {
        healthSlider.maxValue = _playerHealth.GetHealth();
    }

    void Update()
    {
        healthSlider.value = _playerHealth.GetHealth();
        scoreText.text = _scoreKeeper.GetScore().ToString("000000000");
    }

}
