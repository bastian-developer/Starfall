using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using Powers;

namespace Items
{
    public class LifeBattery : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private int lifeRestorationAmount;
        [SerializeField] private int score;
        [SerializeField] private float rotationSpeed;
        
        private Health _health;
        private AudioPlayer _audioPlayer;
        private ScoreKeeper _scoreKeeper;
        private Player _player;

        
        private void Awake()
        {
            _player = FindObjectOfType<Player>();
            _health = _player.GetComponent<Health>();
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
            _audioPlayer.PlayLifeBatteryClip();
            _health.AddHealth(lifeRestorationAmount);
            Destroy(gameObject);
            _scoreKeeper.ModifyScore(score);
            Debug.Log("health eaten");
        }
    }
}
