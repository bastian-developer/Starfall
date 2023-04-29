using System.Collections;
using UnityEngine;
using Characters;

namespace Powers
{
    // This class is in charge of managing the energy of the player
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
        
        // This function is called when some source needs energy to be activated
        // It returns true if the energy cost can be paid, if not, will return false
        public bool PayEnergyCost(int energyCost, string source)
        {
            if (_currentEnergy <= energyCost)
            {
                // Check if the source of energy consumption is the shield
                // If the shield asks for energy to be active and the player
                // Runs out of of this resource, the shields will be deactivated
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
        
        // Function that add the amount of energy passed to
        private void AddEnergy(int energyAmount)
        {
            _currentEnergy += energyAmount;
        }
        
        // This function validates if the energy bar of the player is at his maximum
        // Preventing to stacking an infinite amount of energy.
        private bool ShouldRestoreEnergy()
        {
            return _currentEnergy < maxEnergy;
        }
        
        // This function is responsible for activating or deactivating energy regeneration
        // Based on the amount and rate of energy regeneration sent as parameters
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
        
        // This coroutine adds energy to the player over time, in a loop that runs while the player object is still active.
        // It waits for a certain time specified by passiveEnergyRestorationDelay before adding the energy.
        // The amount of energy added is specified by passiveEnergyRestoration.
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