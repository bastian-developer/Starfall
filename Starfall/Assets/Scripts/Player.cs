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


    private bool _shieldSwitch;
    private float _shieldOffAnimationTime;
    private Shield _shieldToHide;

    private Vector2 _rawInput;
    private Vector2 _minBounds;
    private Vector2 _maxBounds;
    private Shooter _shooter;
    
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

        
        shieldAction = playerInput.actions["Shield"];
        
        //Callbacks
        shieldAction.performed += StartShield;
        shieldAction.canceled += StopShield;
    }
    
    void Update()
    {
        Move();
    }

    void StartShield(InputAction.CallbackContext context)
    {
        if (_shieldSwitch == false)
        {
            if (shieldPrefab1.isActiveAndEnabled)
            {
                shieldPrefab1.gameObject.SetActive(false);
            }
            _shieldAnimator1.StopPlayback();
            shieldPrefab1.gameObject.SetActive(true);

            _audioPlayer.PlayShieldClip();
            
            _shieldAnimator1.SetTrigger("ShieldOn");
            _shieldAnimator1.SetBool("Shield",true);
        }
        else
        {
            if (shieldPrefab2.isActiveAndEnabled)
            {
                shieldPrefab2.gameObject.SetActive(false);
            }
            _shieldAnimator2.StopPlayback();
            shieldPrefab2.gameObject.SetActive(true);

            _audioPlayer.PlayShieldClip();
            
            
            _shieldAnimator2.SetTrigger("ShieldOn");
            _shieldAnimator2.SetBool("Shield",true);
        }
    }
    
    void StopShield(InputAction.CallbackContext context)
    {
        if (_shieldSwitch  == false)
        {
            _shieldAnimator1.SetTrigger("ShieldOff");
            _shieldAnimator1.SetBool("Shield",false);
            StartCoroutine(WaitAndHideShield());
            _shieldSwitch = true;

        }
        else
        {
            _shieldAnimator2.SetTrigger("ShieldOff");
            _shieldAnimator2.SetBool("Shield",false);
            StartCoroutine(WaitAndHideShield());
            _shieldSwitch = false;
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
