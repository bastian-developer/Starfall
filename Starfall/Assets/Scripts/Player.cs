using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 12f;

    private Vector2 _rawInput;

    private Vector2 _minBounds;
    private Vector2 _maxBounds;
    
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
        Debug.Log(_rawInput);
    }

    void Move()
    {
        Vector2 delta = _rawInput * moveSpeed * Time.deltaTime;
        
        Vector2 newPosition = new Vector2();

        newPosition.x = Mathf.Clamp(transform.position.x + delta.x, _minBounds.x, _maxBounds.x);
        newPosition.y = Mathf.Clamp(transform.position.y + delta.y, _minBounds.y, _maxBounds.y);
        
        transform.position = newPosition;
    }
}
