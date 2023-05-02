using UnityEngine;
using GameManagement;

namespace Items
{
    public class CoinManager : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private int maxQuantity;
        [SerializeField] private int startingQuantity;

        private int _currentQuantity;

        public int CurrentCoins => _currentQuantity;

        private void Awake()
        {
            _currentQuantity = startingQuantity; 
        }

        public bool PayCoins(int quantity)
        {
            if (_currentQuantity <= quantity)
            {
                return false;
            }
            else
            {
                _currentQuantity -= quantity;
                return true;
            }
        }

        public void AddCoins(int quantity)
        {
            if (_currentQuantity < maxQuantity )
            {
                _currentQuantity += quantity;

                Debug.Log("added coins " + quantity);
                Debug.Log("total coins " +_currentQuantity);
            }
        }
    }
}