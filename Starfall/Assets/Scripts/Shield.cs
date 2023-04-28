using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shield : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject laserPrefab;
    [Header("Movement")]
    [SerializeField] public float rotationSpeed = 30f;
    [SerializeField] public float followSpeed;
    [Header("Energy")]
    [SerializeField] public int activationCost = 15;
    [SerializeField] public int energyPerSecond = 5;

    public int GetActivationCost()
    {
        return activationCost;
    }
    
    public int GetEnergyPerSecond()
    {
        return energyPerSecond;
    }
    
    private AudioPlayer _audioPlayer;
    Quaternion playerRotation;

    GameObject instance;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            _audioPlayer.PlayShieldClip();
            Destroy(other.gameObject);
            playerRotation = player.transform.rotation;
            instance = Instantiate(laserPrefab, transform.position, playerRotation);
            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
            
            if (rb != null)
            {
                rb.velocity = player.transform.up * player.GetComponent<Shooter>().GetProjectileSpeed();
            }
            Destroy(instance, player.GetComponent<Shooter>().GetProjectileLifetime());

        }
    }

    private void Awake()
    {
        transform.position = player.gameObject.transform.position;
        
        _audioPlayer = FindObjectOfType<AudioPlayer>();

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
