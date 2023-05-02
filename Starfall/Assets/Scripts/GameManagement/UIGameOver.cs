using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameManagement
{
    public class UIGameOver : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI scoreText;
        private ScoreKeeper _scoreKeeper;

        private void Awake()
        {
            _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        }

        private void Start()
        {
            scoreText.text = "Score:\n" + _scoreKeeper.GetScore();
        }
    }
}