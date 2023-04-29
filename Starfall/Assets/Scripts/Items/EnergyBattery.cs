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
        
        private Energy _energy;
        private AudioPlayer _audioPlayer;

        private void Awake()
        {
            _energy = FindObjectOfType<Energy>();
            _audioPlayer = FindObjectOfType<AudioPlayer>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.gameObject.CompareTag("Player")) return;
            _audioPlayer.PlayEnergyBatteryClip();
            _energy.AddEnergy(energyRestorationAmount);
            Destroy(gameObject);
        }
    }

}