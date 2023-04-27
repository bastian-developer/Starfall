using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shield : MonoBehaviour
{

    [SerializeField] private GameObject player;
    
    public float rotationSpeed = 30f;
    public float followSpeed;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            //Play Sound effect
        }
    }

    private void Awake()
    {
        transform.position = player.gameObject.transform.position;
    }

    private void Update()
    {
        if (player)
        {
           transform.position = Vector3.MoveTowards(transform.position, player.transform.position, followSpeed);
           transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }
    
}
