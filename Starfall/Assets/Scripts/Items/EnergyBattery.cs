using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Powers;

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
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.gameObject.CompareTag("Player")) return;
            _audioPlayer.PlayEnergyBatteryClip();
            _energy.AddEnergy(energyRestorationAmount);
            Destroy(gameObject);
            _scoreKeeper.ModifyScore(score);
        }
    }

}