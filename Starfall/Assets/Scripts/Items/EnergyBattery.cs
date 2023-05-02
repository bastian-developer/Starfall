using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Powers;
using GameManagement;

namespace Items {

    public class EnergyBattery : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private int energyRestorationAmount;
        [SerializeField] private int score;
        [SerializeField] private float rotationSpeed;
        
        private Energy _energy;
        private AudioPlayer _audioPlayer;
        private ScoreKeeper _scoreKeeper;
        
        private void Awake()
        {
            _energy = FindObjectOfType<Energy>();
            _audioPlayer = FindObjectOfType<AudioPlayer>();
            _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        }

        private void Update()
        {
            // Rotate the energy battery over time
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            // Check if the colliding object is the player
            if (!col.gameObject.CompareTag("Player")) return;
            // Play a sound effect for the energy battery pickup
            _audioPlayer.PlayEnergyBatteryClip();
            // Add energy to the player
            _energy.AddEnergy(energyRestorationAmount);
            // Destroy the energy battery object
            Destroy(gameObject);
            // Add to the player's score
            _scoreKeeper.ModifyScore(score);
        }
    }

}