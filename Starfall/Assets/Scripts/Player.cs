using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 12f;
    
    [SerializeField] private float paddingLeft;
    [SerializeField] private float paddingRight;
    [SerializeField] private float paddingTop;
    [SerializeField] private float paddingBottom;
    
    [SerializeField] private float rotationModifier;
    [SerializeField] private float rotationSpeed;

    private Animator _animator;

    private Vector2 _rawInput;

    private Vector2 _minBounds;
    private Vector2 _maxBounds;

    private Shooter _shooter;

    void Awake()
    {
        _shooter = GetComponent<Shooter>();
        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        Move();
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

        //Disabled because we're using just iddle animation for now
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
            _animator.SetBool("Left", true);
            _animator.SetBool("Right", false);
            _animator.SetBool("Up", false);
            _animator.SetBool("Down", false);
        }
        if (_rawInput.x == 1)
        {
            _animator.SetBool("Right", true);
            _animator.SetBool("Left", false);
            _animator.SetBool("Up", false);
            _animator.SetBool("Down", false);
        }
        if (_rawInput.y == -1)
        {
            _animator.SetBool("Up", false);
            _animator.SetBool("Down", true);
            _animator.SetBool("Right", false);
            _animator.SetBool("Left", false);
        }
        if (_rawInput.y == 1)
        {
            _animator.SetBool("Up", true);
            _animator.SetBool("Down", false);
            _animator.SetBool("Right", false);
            _animator.SetBool("Left", false);
        }
        if (_rawInput.x == 0)
        {
            _animator.SetBool("Right", false);
            _animator.SetBool("Left", false);
  
            
        }
        if (_rawInput.y == 0)
        {

            _animator.SetBool("Up", false);
            _animator.SetBool("Down", false);
            
        }
    }
}
