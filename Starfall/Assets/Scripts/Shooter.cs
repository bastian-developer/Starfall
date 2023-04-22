using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 5f;
    [SerializeField] private float baseFiringRate = 0.2f;
    
    [Header("AI")]
    [SerializeField] private bool useAI;
    [SerializeField] private float firingRateVariance = 0f;
    [SerializeField] private float minimumFiringRate = 0.1f;


    [HideInInspector] public bool isFiring;

    private Coroutine firingCoroutine;
    
    private AudioPlayer _audioPlayer;

    private Player _player;

    void Awake()
    {
        _audioPlayer = FindObjectOfType<AudioPlayer>();
        _player = FindObjectOfType<Player>();
    }

    
    void Start()
    {
        if (useAI)
        {
            isFiring = true;
        }
    }

    void Update()
    {
        // Call Fire() only when isFiring state changes
        if (isFiring && firingCoroutine == null)
        {
            Fire();
        }
        else if (!isFiring && firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    void Fire()
    {
        if (isFiring && firingCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireContinuosly());
        }
        else if(!isFiring && firingCoroutine != null)
        {
            //StopAllCoroutines();
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    IEnumerator FireContinuosly()
    {
        while (true)
        {
            GameObject instance;
            
            if (useAI && _player)
            {
                //Get Player position to calculate bullet direction
                Vector3 playerPosition = _player.transform.position;
                Vector3 vectorToTarget = playerPosition - transform.position;
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                
                
                instance = Instantiate(projectilePrefab, transform.position, q);
                _audioPlayer.PlayGreenLaserClip();;

            }
            else
            {
                //Get Mouse position to calculate bullet direction
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector3 vectorToTarget = mousePosition - transform.position;
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                
                
                instance = Instantiate(projectilePrefab, transform.position, q);
                _audioPlayer.PlayRedLaserClip();
            }

            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
            
            if (rb != null)
            {
                rb.velocity = transform.up * projectileSpeed;
            }
            
            float timeToNextProjectile = Random.Range(baseFiringRate - firingRateVariance,
                baseFiringRate + firingRateVariance);
            
            timeToNextProjectile = Mathf.Clamp(timeToNextProjectile, minimumFiringRate, float.MaxValue);
            
            Destroy(instance, projectileLifetime);

            //Get the private static instance through public getter via SINGLETON
            //_audioPlayer.GetInstance().PlayShootingClip();
            
            yield return new WaitForSeconds(timeToNextProjectile);

        }
    }
    

}
