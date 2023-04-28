using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shield : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] public GameObject laserPrefab;

    [SerializeField] public float rotationSpeed = 30f;
    [SerializeField] public float followSpeed;

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

            //Play animation effect
            //Turn Back Bullet?
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
