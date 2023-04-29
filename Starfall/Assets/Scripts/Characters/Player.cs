using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    public class Player : MonoBehaviour
    {
        [Header("Powers")] 
        [SerializeField] private Powers.Shield shieldPrefab1;
        [SerializeField] private Powers.Shield shieldPrefab2;
        [SerializeField] private AudioClip shieldAudioSourceStart;
        [SerializeField] private AudioClip shieldAudioSourceEnd;

        [Header("Movement")] 
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotationModifier;
        [SerializeField] private float rotationSpeed;

        [Header("Padding")] 
        [SerializeField] private float paddingLeft;
        [SerializeField] private float paddingRight;
        [SerializeField] private float paddingTop;
        [SerializeField] private float paddingBottom;

        private PlayerAnimator _playerAnimator;
        private Animator _shieldAnimator1;
        private Animator _shieldAnimator2;

        private AudioSource _shieldAudioSourceSegment;
        private AudioSource _shieldAudioSourceLooped;

        private Powers.Shield _shieldToHide;
        private Energy _playerEnergy;
        private Shooter _shooter;

        private Vector2 _rawInput;
        private Vector2 _minBounds;
        private Vector2 _maxBounds;

        private Coroutine _consumeEnergyCoroutine;
        
        //
        private Coroutine _shieldingCoroutine;
        private Coroutine _shieldingSoundCoroutine;
        
        private bool _shieldSwitch;
        private bool _isShielded;
        private float _shieldOffAnimationTime;

        public float GetRotationSpeed()
        {
            return rotationSpeed;
        }

        private void Awake()
        {
            _shooter = GetComponent<Shooter>();
            var animator = GetComponent<Animator>();
            _playerAnimator = new PlayerAnimator(animator);
            
            _shieldAnimator1 = shieldPrefab1.GetComponent<Animator>();
            _shieldAnimator2 = shieldPrefab2.GetComponent<Animator>();

            _shieldAudioSourceSegment = gameObject.AddComponent<AudioSource>();
            //attached as Audio Source Component
            _shieldAudioSourceLooped = GetComponent<AudioSource>();
            
            _playerEnergy = GetComponent<Energy>();

            //Set Input Action Callbacks
            var shieldAction = playerInput.actions["Shield"];
            shieldAction.performed += StartShieldCallback;
            shieldAction.canceled += StopShieldCallback;
        }

        private void Update()
        {
            Move();

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



        private void StartShieldCallback(InputAction.CallbackContext context)
        {
            StartShield();
        }
        
        private void StopShieldCallback(InputAction.CallbackContext context)
        {
            StopShield();
        }
        
        private void StartShield()
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

        private void Start()
        {
            InitBounds();
        }

        void InitBounds()
        {
            Camera mainCamera = Camera.main;
            _minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
            _maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        }

        void OnMove(InputValue value)
        {
            _rawInput = value.Get<Vector2>();
        }

        void OnFire(InputValue value)
        {
            if (_shooter != null)
            {
                _shooter.isFiring = value.isPressed;
            }
        }

        private void Move()
        {
            var delta = _rawInput * moveSpeed * Time.deltaTime;
            _playerAnimator.AnimatePlayer(_rawInput);
            var newPosition = new Vector2
            {
                x = Mathf.Clamp(transform.position.x + delta.x, _minBounds.x + paddingLeft,
                    _maxBounds.x - paddingRight),
                y = Mathf.Clamp(transform.position.y + delta.y, _minBounds.y + paddingBottom,
                    _maxBounds.y - paddingTop)
            };
            transform.position = newPosition;
            RotateTowardsMousePosition();
        }

        private void RotateTowardsMousePosition()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            transform.rotation = Quaternion.Slerp(transform.rotation, _shooter.Vector3ToQuaternion(mousePosition),
                Time.deltaTime * rotationSpeed);
        }
    }
}