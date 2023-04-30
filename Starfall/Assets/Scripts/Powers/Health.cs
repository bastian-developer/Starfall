using System.Collections;
using Characters;
using UnityEngine;
using Items;
using Enemies;

namespace Powers
{
    public class Health : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private bool isPlayer;
        [SerializeField] private int score;
        [SerializeField] private int maxHealth;
        [SerializeField] private int passiveHealthRestoration ;
        [SerializeField] private float passiveHealthRestorationDelay;
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private bool applyCameraShake;

        private int _currentHealth;
        private CameraShake _cameraShake;
        private AudioPlayer _audioPlayer;
        private ScoreKeeper _scoreKeeper;
        private LevelManager _levelManager;
        private Coroutine _restoreHealthCoroutine;

        public int CurrentHealth => _currentHealth;

        private void Awake()
        {
            _audioPlayer = FindObjectOfType<AudioPlayer>();
            _scoreKeeper = FindObjectOfType<ScoreKeeper>();
            _levelManager = FindObjectOfType<LevelManager>();
            _currentHealth = maxHealth;
            var mainCamera = Camera.main;
            if (mainCamera != null) _cameraShake = mainCamera.GetComponent<CameraShake>();
        }

        // A private method that checks whether health needs to be restored
        private bool _shouldRestoreHealth()
        {
            return _currentHealth < maxHealth;
        }

        //Public method that when called adds the amount of health passed
        public void AddHealth(int healthAmount)
        {
            if (_currentHealth + healthAmount > maxHealth && isPlayer)
            {
                _currentHealth = maxHealth;
            }
            else if (isPlayer)
            {
                _currentHealth += healthAmount;
            }
        }
        
        private void Update()
        {
            switch (isPlayer)
            {
                // If this is the player and health needs to be restored, start the restoration coroutine
                case true when _shouldRestoreHealth() && _restoreHealthCoroutine == null:
                    _restoreHealthCoroutine = StartCoroutine(AddHealthOverTime());
                    break;
                // If this is the player and health does not need to be restored, stop the restoration coroutine
                case true when !_shouldRestoreHealth() && _restoreHealthCoroutine != null:
                    StopCoroutine(_restoreHealthCoroutine);
                    _restoreHealthCoroutine = null;
                    break;
            }
        }

        // A coroutine that restores health over time
        private IEnumerator AddHealthOverTime()
        {
            while (_shouldRestoreHealth())
            {
                // Wait for a delay before restoring health
                yield return new WaitForSeconds(passiveHealthRestorationDelay);
                _currentHealth += passiveHealthRestoration;
            }
        }

        // A method that is called when the game object enters a trigger collider
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Get the DamageDealer component from the collider
            var damageDealer = other.GetComponent<DamageDealer>();
            // If there is no DamageDealer component, do nothing
            if (damageDealer == null) return;
            // Take damage from the DamageDealer
            TakeDamage(damageDealer.GetDamage());
            // Play a hit effect and shake the camera
            PlayHitEffect();
            ShakeCamera();
            
            
            // Notify the DamageDealer that it has hit something
            if (isPlayer)
            {
                damageDealer.GetDamage();
            }
            else
            {
                damageDealer.Hit();
            }
        }


        // A method that shakes the camera if camera shake is enabled
        private void ShakeCamera()
        {
            if (_cameraShake && applyCameraShake)
            {
                _cameraShake.Play();
            }
        }

        // A method that takes damage and handles death
        private void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        // A method that handles death
        private void Die()
        {
            if (!isPlayer)
            {
                // Modify the score if this is not the player
                _scoreKeeper.ModifyScore(score);
                GameObject o;
                var dropper = (o = gameObject).GetComponent<EnemyDropper>();
                dropper.DropItems(o);
            }
            else
            {
                // Load the game over scene if this is the player
                _levelManager.LoadGameOver();
            }
            
            // Destroy the game object
            Destroy(gameObject);
        }

        // A method that plays the hit effect and damage sound
        private void PlayHitEffect()
        {
            if (hitEffect == null) return;
            var instance = Instantiate(hitEffect, transform.position, Quaternion.identity);
            var main = instance.main;
            Destroy(instance.gameObject, main.duration + main.startLifetime.constantMax);
            if (isPlayer)
            {
                _audioPlayer.PlayPlayerDamageClip();
            }
            else
            {
                _audioPlayer.PlayAlienDamageClip();
            }
        }
    }
}