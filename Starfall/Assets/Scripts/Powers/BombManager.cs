using System;
using UnityEngine;
using System.Collections;

namespace Powers
{
    public class BombManager : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private int maxQuantity;

        private int _currentQuantity;
        private CameraShake _cameraShake;

        private void Awake()
        {
            _currentQuantity = maxQuantity;
            _cameraShake = FindObjectOfType<CameraShake>();
        }

        private bool CanBomb()
        {
            return _currentQuantity > 0;
        }

        public void AddBomb()
        {
            if (_currentQuantity < maxQuantity )
            {
                _currentQuantity += 1;
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
            //Animate
            //Sound effect
            //bomb item
            _cameraShake.Play();
            _currentQuantity -= 1;
            Debug.Log("Bombed " + _currentQuantity);
            
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (var VARIABLE in objectsWithTag)
            {
                VARIABLE.GetComponent<Health>().Die();
            }

        }
        
        
    }
}