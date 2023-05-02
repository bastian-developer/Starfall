using Characters;
using UnityEngine;
using Powers;
using GameManagement;

namespace Items
{
    public class Coin : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private int coinAmount;
        [SerializeField] private int score;
        [SerializeField] private float rotationSpeed;
        
        private AudioPlayer _audioPlayer;
        private ScoreKeeper _scoreKeeper;
        private CoinManager _coinManager;


        private void Awake()
        {
            _audioPlayer = FindObjectOfType<AudioPlayer>();
            _scoreKeeper = FindObjectOfType<ScoreKeeper>();
            _coinManager = FindObjectOfType<CoinManager>();
        }

        private void Update()
        {
            // Rotate coin over time
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            // Check if the colliding object is the player
            if (!col.gameObject.CompareTag("Player")) return;
            // Play a sound effect for the coin pickup
            _audioPlayer.PlayCoinClip();
            // Add coins to the player
            _coinManager.AddCoins(coinAmount);
            // Destroy the life battery object
            Destroy(gameObject);
            // Add to the player's score
            _scoreKeeper.ModifyScore(score);
        }
    }
}