using Characters;
using UnityEngine;
using Powers;
using GameManagement;

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


        private void Awake()
        {
            var player = FindObjectOfType<Player>();
            // Imperative to get specifically the player health component
            _health = player.GetComponent<Health>();
            _audioPlayer = FindObjectOfType<AudioPlayer>();
            _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        }

        private void Update()
        {
            // Rotate the life battery over time
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            // Check if the colliding object is the player
            if (!col.gameObject.CompareTag("Player")) return;
            // Play a sound effect for the life battery pickup
            _audioPlayer.PlayLifeBatteryClip();
            // Add health to the player
            _health.AddHealth(lifeRestorationAmount);
            // Destroy the life battery object
            Destroy(gameObject);
            // Add to the player's score
            _scoreKeeper.ModifyScore(score);
        }
    }
}
