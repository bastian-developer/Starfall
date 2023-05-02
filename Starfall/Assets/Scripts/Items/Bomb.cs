using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManagement;
using Powers;

namespace Items
{
    public class Bomb : MonoBehaviour
    {

        [Header("Setup")] 
        [SerializeField] private int refillAmount;
        [SerializeField] private int score;
        [SerializeField] private float rotationSpeed;

        private AudioPlayer _audioPlayer;
        private ScoreKeeper _scoreKeeper;
        private BombManager _bombManager;

        private void Awake()
        {
            _bombManager = FindObjectOfType<BombManager>();
            _audioPlayer = FindObjectOfType<AudioPlayer>();
            _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        }
        
        private void Update()
        {
            // Rotate the bomb over time
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            // Check if the colliding object is the player
            if (!col.gameObject.CompareTag("Player")) return;
            // Play a sound effect for the energy battery pickup
            _audioPlayer.PlayBombClip();
            // Add energy to the player
            _bombManager.AddBombs(refillAmount);
            // Destroy the energy battery object
            Destroy(gameObject);
            // Add to the player's score
            _scoreKeeper.ModifyScore(score);
        }
    }

}