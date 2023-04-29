using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Powers;

namespace Characters
{
    public class Player : MonoBehaviour
    {
        [Header("Powers")] [SerializeField] private Shield shieldPrefab1;
        [SerializeField] private Shield shieldPrefab2;
        [SerializeField] private AudioClip shieldAudioSourceStart;
        [SerializeField] private AudioClip shieldAudioSourceEnd;

        [Header("Movement")] [SerializeField] private PlayerInput playerInput;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotationModifier;
        [SerializeField] private float rotationSpeed;

        [Header("Padding")] [SerializeField] private float paddingLeft;
        [SerializeField] private float paddingRight;
        [SerializeField] private float paddingTop;
        [SerializeField] private float paddingBottom;

        private PlayerAnimator _playerAnimator;
        private Animator _shieldAnimator1;
        private Animator _shieldAnimator2;

        private AudioSource _shieldAudioSource1;
        private AudioSource _shieldAudioSource2;

        private Shield _shieldToHide;
        private Energy _playerEnergy;
        private Shooter _shooter;

        private Vector2 _rawInput;
        private Vector2 _minBounds;
        private Vector2 _maxBounds;

        private Coroutine _restoreEnergyCoroutine;
        private Coroutine _consumeEnergyCoroutine;

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

            _shieldAudioSource1 = gameObject.AddComponent<AudioSource>();
            _shieldAudioSource2 = GetComponent<AudioSource>();
            _playerEnergy = GetComponent<Energy>();

            //Set Input Action Callbacks
            var shieldAction = playerInput.actions["Shield"];
            shieldAction.performed += StartShield;
            shieldAction.canceled += StopShieldCallback;
        }

        private void Update()
        {
            Move();

            if (!_isShielded && _shieldAudioSource2.isPlaying)
            {
                StartCoroutine(WaitAndStopShieldSoundEffect());
            }

            if (_playerEnergy.ShouldRestoreEnergy() && _restoreEnergyCoroutine == null)
            {
                _restoreEnergyCoroutine = StartCoroutine(AddEnergyOverTime());
            }
            else if (!_playerEnergy.ShouldRestoreEnergy() && _restoreEnergyCoroutine != null)
            {
                StopCoroutine(_restoreEnergyCoroutine);
                _restoreEnergyCoroutine = null;
            }

            if (_isShielded && _consumeEnergyCoroutine == null)
            {
                _consumeEnergyCoroutine = StartCoroutine(RemoveEnergyOverTimeShield());
            }
            else if (!_isShielded && _consumeEnergyCoroutine != null)
            {
                StopCoroutine(_consumeEnergyCoroutine);
                _consumeEnergyCoroutine = null;
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

        private IEnumerator AddEnergyOverTime()
        {
            while (gameObject)
            {
                yield return new WaitForSeconds(_playerEnergy.GetPassiveRestorationDelay());
                _playerEnergy.AddEnergy(_playerEnergy.GetPassiveEnergyRestoration());
            }
        }

        void StartShield(InputAction.CallbackContext context)
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

        private void StopShieldSoundEffect()
        {
            _shieldAudioSource1.Stop();
            _shieldAudioSource2.Stop();
            StopCoroutine(WaitAndSoundShield());
        }

        private IEnumerator WaitAndStopShieldSoundEffect()
        {
            yield return new WaitForSeconds(_shieldAudioSource1.clip.length);
            StopShieldSoundEffect();
        }

        private IEnumerator WaitAndSoundShield()
        {
            _shieldAudioSource1.clip = shieldAudioSourceStart;
            _shieldAudioSource1.Play();
            yield return new WaitForSeconds(_shieldAudioSource1.clip.length);
            _shieldAudioSource2.Play();
        }

        private void StopShieldCallback(InputAction.CallbackContext context)
        {
            StopShield();
        }

        public void StopShield()
        {
            if (!_shieldSwitch && _isShielded)
            {
                StopShieldSoundEffect();
                _shieldAudioSource1.clip = shieldAudioSourceEnd;
                _shieldAudioSource1.Play();
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
                _shieldAudioSource1.clip = shieldAudioSourceEnd;
                _shieldAudioSource1.Play();
                _shieldAnimator2.SetTrigger("ShieldOff");
                _shieldAnimator2.SetBool("Shield", false);
                StartCoroutine(WaitAndHideShield());
                _shieldSwitch = false;
                _isShielded = false;
                StopCoroutine(RemoveEnergyOverTimeShield());
            }
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