using System.Collections;
using Characters;
using UnityEngine;

namespace Powers
{
    // This class is meant to turn on and of two different Shield instances by switching them
    // It is also responsible for the sound effect that is divided in 3 sections
    // The start and end sections are added as clips
    // The middle section was added as an AudioSource to set it looped when player is shielded
    public class ShieldManager : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private Shield shieldPrefab1;
        [SerializeField] private Shield shieldPrefab2;
        [SerializeField] private AudioClip shieldAudioSourceStart;
        [SerializeField] private AudioClip shieldAudioSourceEnd;

        private Animator _shieldAnimator1;
        private Animator _shieldAnimator2;
        private Animator _currentShieldAnimator;
        private AudioSource _shieldAudioSourceSegment;
        private AudioSource _shieldAudioSourceLooped;
        private Shield _currentShield;
        private Energy _playerEnergy;
        private Coroutine _shieldingSoundCoroutine;
        private Player _player;
        
        private bool _shieldSwitch;
        private bool _isShielded;
        private static readonly int ShieldOn = Animator.StringToHash("ShieldOn");
        private static readonly int Shield = Animator.StringToHash("Shield");
        private static readonly int ShieldOff = Animator.StringToHash("ShieldOff");

        private void Awake()
        {
            _player = FindObjectOfType<Player>();
            _shieldAnimator1 = shieldPrefab1.GetComponent<Animator>();
            _shieldAnimator2 = shieldPrefab2.GetComponent<Animator>();
            _shieldAudioSourceSegment = gameObject.AddComponent<AudioSource>();
            
            //Attached as Audio Source Component
            _shieldAudioSourceLooped = GetComponent<AudioSource>();
            _playerEnergy = GetComponent<Energy>();
            _currentShield = shieldPrefab1;
            _currentShieldAnimator = _shieldAnimator1;
        }

        private void Update()
        {
            _currentShield.ManageEnergyConsumption(_isShielded);
        }
        
        // This method starts the shield activation process
        public void StartShield()
        {
            // Check if shield is already active or if player can't pay the energy cost
            if (!_player ||_isShielded || !_playerEnergy.PayEnergyCost(_currentShield.EnergyActivationCost, "Shielding")) return;
            
            // Stop shield animation playback and activate the shield game object
            _currentShieldAnimator.StopPlayback();
            _currentShield.gameObject.SetActive(true);
            
            // Start coroutine to play the shielding sound effect and set the shield animation triggers
            _shieldingSoundCoroutine = StartCoroutine(WaitAndSoundShield());
            _currentShieldAnimator.SetTrigger(ShieldOn);
            _currentShieldAnimator.SetBool(Shield, true);
            
            // Set _isShielded to true to indicate that the shield is active
            _isShielded = true;
        }
        
        // This method stops the shield deactivation process
        public void StopShield()
        {
            // Check if the shield is not active
            if (!_isShielded) return;
            
            // Stop the shield sound effect and play the shield deactivation sound effect
            StopShieldSoundEffect();
            _shieldAudioSourceSegment.clip = shieldAudioSourceEnd;
            _shieldAudioSourceSegment.Play();
            
            // Set the shield animation triggers and hide the shield game object
            _currentShieldAnimator.SetTrigger(ShieldOff);
            _currentShieldAnimator.SetBool(Shield, false);
            StartCoroutine(WaitAndHideShield(_currentShield));
            
            // Set _isShielded to false to indicate that the shield is not active anymore and switch shields
            _isShielded = false;
            SwitchShields();
        }

        // This method switches the current shield
        private void SwitchShields()
        {
            // Check the current shield and set the currentShield and currentShieldAnimator to the other shield prefab accordingly
            if (_currentShield.name == shieldPrefab1.name)
            {
                _currentShield = shieldPrefab2;
                _currentShieldAnimator = _shieldAnimator2;
            } else if (_currentShield.name == shieldPrefab2.name)
            {
                _currentShield = shieldPrefab1;
                _currentShieldAnimator = _shieldAnimator1;
            }
        }
        
        // This method stops the shield sound effect
        private void StopShieldSoundEffect()
        {
            // Stop the shielding sound effect and looping sound effect and stop the coroutine that plays the shielding sound effect
            _shieldAudioSourceSegment.Stop();
            _shieldAudioSourceLooped.Stop();
            StopCoroutine(_shieldingSoundCoroutine);
            _shieldingSoundCoroutine = null;
        }

        // This coroutine plays the shielding sound effect
        private IEnumerator WaitAndSoundShield()
        {
            // Play the shielding sound effect and wait for its length
            _shieldAudioSourceSegment.clip = shieldAudioSourceStart;
            _shieldAudioSourceSegment.Play();
            yield return new WaitForSeconds(_shieldAudioSourceSegment.clip.length);
            
            // Play the looping sound effect
            _shieldAudioSourceLooped.Play();
        }
        
        // This coroutine waits for the shield off animation to finish and hides the shield game object
        private IEnumerator WaitAndHideShield(Component shield)
        {
            // Wait for the shield off animation to finish
            var shieldOffAnimationTime = _currentShieldAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(shieldOffAnimationTime);
            
            // Hide the shield game object
            shield.gameObject.SetActive(false);
        }
    }
}