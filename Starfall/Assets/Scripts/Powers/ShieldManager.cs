using System.Collections;
using UnityEngine;

namespace Powers
{
    //This class is meant to turn on and of two different Shield instances by switching them
    //It is also responsible for the sound effect that is divided in 3 sections
    //The start and end sections are added as clips
    //The middle section was added as an AudioSource to set it looped when player is shielded
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
        
        private bool _shieldSwitch;
        private bool _isShielded;
        private static readonly int ShieldOn = Animator.StringToHash("ShieldOn");
        private static readonly int Shield = Animator.StringToHash("Shield");
        private static readonly int ShieldOff = Animator.StringToHash("ShieldOff");

        private void Awake()
        {
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
        
        public void StartShield()
        {
            if (_isShielded || !_playerEnergy.PayEnergyCost(_currentShield.EnergyActivationCost, "Shielding")) return;
            _currentShieldAnimator.StopPlayback();
            _currentShield.gameObject.SetActive(true);
            _shieldingSoundCoroutine = StartCoroutine(WaitAndSoundShield());
            _currentShieldAnimator.SetTrigger(ShieldOn);
            _currentShieldAnimator.SetBool(Shield, true);
            _isShielded = true;
        }
        
        public void StopShield()
        {
            if (!_isShielded) return;
            StopShieldSoundEffect();
            _shieldAudioSourceSegment.clip = shieldAudioSourceEnd;
            _shieldAudioSourceSegment.Play();
            _currentShieldAnimator.SetTrigger(ShieldOff);
            _currentShieldAnimator.SetBool(Shield, false);
            StartCoroutine(WaitAndHideShield(_currentShield));
            _isShielded = false;
            SwitchShields();
        }

        private void SwitchShields()
        {
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
        
        private void StopShieldSoundEffect()
        {
            _shieldAudioSourceSegment.Stop();
            _shieldAudioSourceLooped.Stop();
            StopCoroutine(_shieldingSoundCoroutine);
            _shieldingSoundCoroutine = null;
        }

        private IEnumerator WaitAndSoundShield()
        {
            _shieldAudioSourceSegment.clip = shieldAudioSourceStart;
            _shieldAudioSourceSegment.Play();
            yield return new WaitForSeconds(_shieldAudioSourceSegment.clip.length);
            _shieldAudioSourceLooped.Play();
        }
        
        private IEnumerator WaitAndHideShield(Component shield)
        {
            var shieldOffAnimationTime = _currentShieldAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(shieldOffAnimationTime);
            shield.gameObject.SetActive(false);
        }
    }
}