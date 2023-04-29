using System;
using System.Collections;
using UnityEngine;

namespace Powers
{
    public class ShieldManager : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private Shield shieldPrefab1;
        [SerializeField] private Shield shieldPrefab2;
        [SerializeField] private AudioClip shieldAudioSourceStart;
        [SerializeField] private AudioClip shieldAudioSourceEnd;
        //Add Audio Source as Component
        //Add ShieldManager to Player
        
        private Animator _shieldAnimator1;
        private Animator _shieldAnimator2;
        
        private AudioSource _shieldAudioSourceSegment;
        private AudioSource _shieldAudioSourceLooped;
        
        private Powers.Shield _shieldToHide;
        
        private Energy _playerEnergy;

        private Coroutine _shieldingCoroutine;
        private Coroutine _shieldingSoundCoroutine;
        
        private bool _shieldSwitch;
        private bool _isShielded;
        private float _shieldOffAnimationTime;
        
        
        private void Awake()
        {
            
            _shieldAnimator1 = shieldPrefab1.GetComponent<Animator>();
            _shieldAnimator2 = shieldPrefab2.GetComponent<Animator>();

            _shieldAudioSourceSegment = gameObject.AddComponent<AudioSource>();
            //attached as Audio Source Component
            _shieldAudioSourceLooped = GetComponent<AudioSource>();
            
            _playerEnergy = GetComponent<Energy>();
            
        }

        private void Update()
        {
            if (!_shieldSwitch)
            {
                shieldPrefab1.ManageEnergyConsumption(_isShielded);
            }
            else if (_shieldSwitch)
            {
                shieldPrefab2.ManageEnergyConsumption(_isShielded);
            }

            //Hard Stop to Shield Sound Effect if playing and not shielded
            if (!_isShielded && _shieldAudioSourceLooped.isPlaying)
            {
                StartCoroutine(WaitAndStopShieldSoundEffect());
            }
        }
        
        
        private IEnumerator RemoveEnergyOverTimeShield()
        {
            while (_isShielded)
            {
                yield return new WaitForSeconds(shieldPrefab1.EnergyConsumptionDelay);
                _playerEnergy.PayEnergyCost(shieldPrefab1.EnergyCostOverTime, "Shielding");
            }
        }
        
        public void StartShield()
        {
            if (!_shieldSwitch && !_isShielded &&
                _playerEnergy.PayEnergyCost(shieldPrefab1.EnergyActivationCost, "Shielding"))
            {
                if (shieldPrefab1.isActiveAndEnabled)
                {
                    shieldPrefab1.gameObject.SetActive(false);
                }

                _shieldAnimator1.StopPlayback();
                StopShieldSoundEffect();
                shieldPrefab1.gameObject.SetActive(true);
                StartCoroutine(WaitAndSoundShield());
                _shieldAnimator1.SetTrigger("ShieldOn");
                _shieldAnimator1.SetBool("Shield", true);
                _isShielded = true;
            }
            else if (_shieldSwitch && !_isShielded &&
                     _playerEnergy.PayEnergyCost(shieldPrefab1.EnergyActivationCost, "Shielding"))
            {
                if (shieldPrefab2.isActiveAndEnabled)
                {
                    shieldPrefab2.gameObject.SetActive(false);
                }

                _shieldAnimator2.StopPlayback();
                StopShieldSoundEffect();
                shieldPrefab2.gameObject.SetActive(true);
                StartCoroutine(WaitAndSoundShield());
                _shieldAnimator2.SetTrigger("ShieldOn");
                _shieldAnimator2.SetBool("Shield", true);
                _isShielded = true;
            }
        }
        
        public void StopShield()
        {
            if (!_shieldSwitch && _isShielded)
            {
                StopShieldSoundEffect();
                _shieldAudioSourceSegment.clip = shieldAudioSourceEnd;
                _shieldAudioSourceSegment.Play();
                _shieldAnimator1.SetTrigger("ShieldOff");
                _shieldAnimator1.SetBool("Shield", false);
                StartCoroutine(WaitAndHideShield());
                _shieldSwitch = true;
                _isShielded = false;
                StopCoroutine(RemoveEnergyOverTimeShield());
            }
            else if (_shieldSwitch && _isShielded)
            {
                StopShieldSoundEffect();
                _shieldAudioSourceSegment.clip = shieldAudioSourceEnd;
                _shieldAudioSourceSegment.Play();
                _shieldAnimator2.SetTrigger("ShieldOff");
                _shieldAnimator2.SetBool("Shield", false);
                StartCoroutine(WaitAndHideShield());
                _shieldSwitch = false;
                _isShielded = false;
                StopCoroutine(RemoveEnergyOverTimeShield());
            }
        }
        
        private void StopShieldSoundEffect()
        {
            _shieldAudioSourceSegment.Stop();
            _shieldAudioSourceLooped.Stop();
            StopCoroutine(WaitAndSoundShield());
        }

        private IEnumerator WaitAndStopShieldSoundEffect()
        {
            yield return new WaitForSeconds(_shieldAudioSourceSegment.clip.length);
            StopShieldSoundEffect();
        }

        private IEnumerator WaitAndSoundShield()
        {
            _shieldAudioSourceSegment.clip = shieldAudioSourceStart;
            _shieldAudioSourceSegment.Play();
            yield return new WaitForSeconds(_shieldAudioSourceSegment.clip.length);
            _shieldAudioSourceLooped.Play();
        }
        
        private IEnumerator WaitAndHideShield()
        {
            if (!_shieldSwitch)
            {
                _shieldOffAnimationTime = _shieldAnimator1.GetCurrentAnimatorStateInfo(0).length;
                _shieldToHide = shieldPrefab1;
            }
            else
            {
                _shieldOffAnimationTime = _shieldAnimator2.GetCurrentAnimatorStateInfo(0).length;
                _shieldToHide = shieldPrefab2;
            }

            yield return new WaitForSeconds(_shieldOffAnimationTime);
            _shieldToHide.gameObject.SetActive(false);
        }
    }
}