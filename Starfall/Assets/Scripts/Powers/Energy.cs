using System.Collections;
using UnityEngine;
using Characters;

namespace Powers
{
    public class Energy : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private int maxEnergy = 100;
        [SerializeField] private int passiveEnergyRestoration = 1;
        [SerializeField] private float passiveEnergyRestorationDelay = 1;
        
        private Player _player;
        private ShieldManager _shieldManager;
        private Coroutine _restoreEnergyCoroutine;
        private int _currentEnergy;
        public int CurrentEnergy => _currentEnergy;
    
        private void Awake()
        {
            _player = FindObjectOfType<Player>();
            _shieldManager = FindObjectOfType<ShieldManager>();
            _currentEnergy = maxEnergy;
        }
    
        private void Update()
        {
            ManageEnergyRestoration();
        }
        
        public bool PayEnergyCost(int energyCost, string source)
        {
            if (_currentEnergy <= energyCost)
            {
                if (source == "Shielding")
                {
                    _shieldManager.StopShield();
                }
                return false;
            }
            else
            {
                _currentEnergy -= energyCost;
                return true;
            }
        }
        
        private void AddEnergy(int energyAmount)
        {
            _currentEnergy += energyAmount;
        }
        
        private bool ShouldRestoreEnergy()
        {
            return _currentEnergy < maxEnergy;
        }
        
        private void ManageEnergyRestoration()
        {
            if (ShouldRestoreEnergy() && _restoreEnergyCoroutine == null)
            {
                _restoreEnergyCoroutine = StartCoroutine(AddEnergyOverTime());
            }
            else if (!ShouldRestoreEnergy() && _restoreEnergyCoroutine != null)
            {
                StopCoroutine(_restoreEnergyCoroutine);
                _restoreEnergyCoroutine = null;
            }
        }
        
        private IEnumerator AddEnergyOverTime()
        {
            while (_player)
            {
                yield return new WaitForSeconds(passiveEnergyRestorationDelay);
                AddEnergy(passiveEnergyRestoration);
            }
        }
    }
}