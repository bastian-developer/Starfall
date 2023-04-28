using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Powers")]
    [SerializeField] private Shield shieldPrefab1;
    [SerializeField] private Shield shieldPrefab2;

    [SerializeField] private InputAction shieldAction;

    [Header("Movement")] 
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float rotationModifier;
    [SerializeField] private float rotationSpeed;
    
    [Header("Padding")]
    [SerializeField] private float paddingLeft;
    [SerializeField] private float paddingRight;
    [SerializeField] private float paddingTop;
    [SerializeField] private float paddingBottom;

    
    private Animator _playerAnimator;
    private Animator _shieldAnimator1;
    private Animator _shieldAnimator2;
    
    private AudioPlayer _audioPlayer;
    private AudioSource _shieldAudioSource1;
    private AudioSource _shieldAudioSource2;

    
    [SerializeField] AudioClip shieldAudioSourceStart;
    [SerializeField] AudioClip shieldAudioSourceEnd;

    private Energy playerEnergy;

    private bool _shieldSwitch;
    private bool _isShielded;
    private float _shieldOffAnimationTime;
    private Shield _shieldToHide;

    private Vector2 _rawInput;
    private Vector2 _minBounds;
    private Vector2 _maxBounds;
    private Shooter _shooter;
    
    private Coroutine _restoreEnergyCoroutine;
    private Coroutine _consumeEnergyCoroutine;

    
    public float GetRotationSpeed()
    {
        return rotationSpeed;
    }

    void Awake()
    {
        _shooter = GetComponent<Shooter>();
        _playerAnimator = GetComponent<Animator>();
        _shieldAnimator1 = shieldPrefab1.GetComponent<Animator>();
        _shieldAnimator2 = shieldPrefab2.GetComponent<Animator>();
        
        _audioPlayer = FindObjectOfType<AudioPlayer>();
        _shieldAudioSource1 = gameObject.AddComponent<AudioSource>();
        _shieldAudioSource2 = GetComponent<AudioSource>();
        
        shieldAction = playerInput.actions["Shield"];

        playerEnergy = GetComponent<Energy>();
        
        //Callbacks
        shieldAction.performed += StartShield;
        shieldAction.canceled += StopShieldCallback;
    }
    
    void Update()
    {
        Move();
        
        if (!_isShielded && _shieldAudioSource2.isPlaying)
        {
            StartCoroutine(WaitAndStopShieldSoundEffect());
        }
        
        if (playerEnergy.ShouldRestoreEnergy() && _restoreEnergyCoroutine == null)
        {
            _restoreEnergyCoroutine = StartCoroutine(AddEnergyPerTime());
        }
        else if (!playerEnergy.ShouldRestoreEnergy() && _restoreEnergyCoroutine != null)
        {
            StopCoroutine(_restoreEnergyCoroutine);
            _restoreEnergyCoroutine = null;
        }
        
        if (_isShielded && _consumeEnergyCoroutine == null)
        {
            _consumeEnergyCoroutine = StartCoroutine(RemoveEnergyPerTimeShield());
        }
        else if (!_isShielded && _consumeEnergyCoroutine != null)
        {
            StopCoroutine(_consumeEnergyCoroutine);
            _consumeEnergyCoroutine = null;
        }
    }
    
    IEnumerator RemoveEnergyPerTimeShield()
    {
        while (true)
        {
            yield return new WaitForSeconds(shieldPrefab1.GetEnergyConsumptionDelay());
            playerEnergy.PayEnergyCost(shieldPrefab1.GetEnergyPerTime());
        }
    }
    
    IEnumerator AddEnergyPerTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(playerEnergy.GetPassiveRestorationDelay());
            playerEnergy.AddEnergy(playerEnergy.GetPassiveEnergyRestoration());
        }
    }

    void StartShield(InputAction.CallbackContext context)
    {
        if (_shieldSwitch == false && _isShielded == false && playerEnergy.PayEnergyCost(shieldPrefab1.GetActivationCost()))
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
            _shieldAnimator1.SetBool("Shield",true);
            _isShielded = true;
        }
        else if (_shieldSwitch == true && _isShielded == false && playerEnergy.PayEnergyCost(shieldPrefab1.GetActivationCost()))
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
            _shieldAnimator2.SetBool("Shield",true);
            _isShielded = true;
        }
    }

    void StopShieldSoundEffect()
    {
        _shieldAudioSource1.Stop();
        _shieldAudioSource2.Stop();
        StopCoroutine(WaitAndSoundShield());
    }
    IEnumerator WaitAndStopShieldSoundEffect()
    {
        yield return new WaitForSeconds(_shieldAudioSource1.clip.length);
        StopShieldSoundEffect();
    }
    
    IEnumerator WaitAndSoundShield()
    {
        _shieldAudioSource1.clip = shieldAudioSourceStart;
        _shieldAudioSource1.Play();
        yield return new WaitForSeconds(_shieldAudioSource1.clip.length);
        _shieldAudioSource2.Play();
    }
    

    public void StopShieldCallback(InputAction.CallbackContext context)
    {
        StopShield();
    }

    public void StopShield()
    {
        if (_shieldSwitch  == false && _isShielded)
        {
            StopShieldSoundEffect();
            _shieldAudioSource1.clip = shieldAudioSourceEnd;
            _shieldAudioSource1.Play();
            _shieldAnimator1.SetTrigger("ShieldOff");
            _shieldAnimator1.SetBool("Shield",false);
            StartCoroutine(WaitAndHideShield());
            _shieldSwitch = true;
            _isShielded = false;
            StopCoroutine(RemoveEnergyPerTimeShield());
        }
        else if (_shieldSwitch && _isShielded)
        {
            StopShieldSoundEffect();
            _shieldAudioSource1.clip = shieldAudioSourceEnd;
            _shieldAudioSource1.Play();
            _shieldAnimator2.SetTrigger("ShieldOff");
            _shieldAnimator2.SetBool("Shield",false);
            StartCoroutine(WaitAndHideShield());
            _shieldSwitch = false;
            _isShielded = false;
            StopCoroutine(RemoveEnergyPerTimeShield());
        }   
    }
    IEnumerator WaitAndHideShield()
    {
        if (_shieldSwitch  == false)
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

    void Move()
    {
        Vector2 delta = _rawInput * moveSpeed * Time.deltaTime;
        AnimatePlayer();
        Vector2 newPosition = new Vector2();
        newPosition.x = Mathf.Clamp(transform.position.x + delta.x, _minBounds.x + paddingLeft, _maxBounds.x - paddingRight);
        newPosition.y = Mathf.Clamp(transform.position.y + delta.y, _minBounds.y + paddingBottom, _maxBounds.y - paddingTop);
        transform.position = newPosition;
        RotateTowardsMousePosition();
    }

    void RotateTowardsMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        transform.rotation = Quaternion.Slerp(transform.rotation, _shooter.Vector3ToQuaternion(mousePosition), Time.deltaTime * rotationSpeed);
    }
    
    void AnimatePlayer()
    {
        if (_rawInput.x == -1)
        {
            _playerAnimator.SetBool("Left", true);
            _playerAnimator.SetBool("Right", false);
            _playerAnimator.SetBool("Up", false);
            _playerAnimator.SetBool("Down", false);
        }
        if (_rawInput.x == 1)
        {
            _playerAnimator.SetBool("Right", true);
            _playerAnimator.SetBool("Left", false);
            _playerAnimator.SetBool("Up", false);
            _playerAnimator.SetBool("Down", false);
        }
        if (_rawInput.y == -1)
        {
            _playerAnimator.SetBool("Up", false);
            _playerAnimator.SetBool("Down", true);
            _playerAnimator.SetBool("Right", false);
            _playerAnimator.SetBool("Left", false);
        }
        if (_rawInput.y == 1)
        {
            _playerAnimator.SetBool("Up", true);
            _playerAnimator.SetBool("Down", false);
            _playerAnimator.SetBool("Right", false);
            _playerAnimator.SetBool("Left", false);
        }
        if (_rawInput.x == 0)
        {
            _playerAnimator.SetBool("Right", false);
            _playerAnimator.SetBool("Left", false);
        }
        if (_rawInput.y == 0)
        {
            _playerAnimator.SetBool("Up", false);
            _playerAnimator.SetBool("Down", false);
        }
    }
}
