using System;
using UnityEngine;
using System.Collections;

namespace Powers
{
    public class BombManager : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private int maxQuantity;
        [SerializeField] private int startingQuantity;

        private int _currentQuantity;
        private CameraShake _cameraShake;
        private AudioPlayer _audioPlayer;
        [SerializeField] private Animator _animator;

        public int CurrentBombs => _currentQuantity;

        private void Awake()
        {
            _currentQuantity = startingQuantity;
            _cameraShake = FindObjectOfType<CameraShake>();
            _audioPlayer = FindObjectOfType<AudioPlayer>();
        }

        private bool CanBomb()
        {
            return _currentQuantity > 0;
        }

        public void AddBombs(int quantity)
        {
            if (_currentQuantity < maxQuantity )
            {
                _currentQuantity += quantity;
                Debug.Log("Bombs added " + _currentQuantity);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Enemy"))
            {
                Destroy(col.gameObject);
            }
        }

        public void Explode()
        {
            if (!CanBomb()) return;
            _audioPlayer.PlayExplosionClip();
            _animator.SetTrigger("Explode");
            _cameraShake.PlayBomb();
            _currentQuantity -= 1;
            Debug.Log("Bombed " + _currentQuantity);
            
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");

            
            foreach (var variable in enemies)
            {
                variable.GetComponent<Health>().Die();
            }
            
            var lasers = GameObject.FindGameObjectsWithTag("Laser");

            foreach (var variable in lasers)
            {
                Destroy(variable.gameObject);
            }
        }
        
        
    }
}