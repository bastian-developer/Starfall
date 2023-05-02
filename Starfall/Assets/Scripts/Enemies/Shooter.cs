using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;
using Characters;
using Powers;
using GameManagement;

namespace Enemies
{
    public class Shooter : MonoBehaviour
    {
        [Header("Primary Weapon")] [SerializeField]
        private GameObject projectilePrefab;

        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float projectileLifetime = 5f;
        [SerializeField] private float baseFiringRate = 0.2f;
        [SerializeField] private int energyCost;


        [Header("Secondary Weapon")] [SerializeField]
        private GameObject projectilePrefabSecondary;

        [SerializeField] private float projectileSpeedSecondary = 10f;
        [SerializeField] private float projectileLifetimeSecondary = 5f;
        [SerializeField] private float baseFiringRateSecondary = 0.2f;
        [SerializeField] private int lifeCostSecondary;


        [Header("AI")] [SerializeField] private bool useAI;
        [SerializeField] private float firingRateVariance = 0f;
        [SerializeField] private float minimumFiringRate = 0.1f;

        [HideInInspector] public bool isFiring;
        [HideInInspector] public bool isFiringSecondary;

        private Coroutine _firingCoroutine;
        private Coroutine _firingCoroutineSecondary;


        private AudioPlayer _audioPlayer;

        private Player _player;

        private Energy _energy;
        private Health _health;




        public float GetProjectileSpeed()
        {
            return projectileSpeed;
        }

        public float GetProjectileLifetime()
        {
            return projectileSpeed;
        }

        private void Awake()
        {
            _audioPlayer = FindObjectOfType<AudioPlayer>();
            _player = FindObjectOfType<Player>();
            _energy = FindObjectOfType<Energy>();
            _health = GetComponent<Health>();
        }


        private void Start()
        {
            if (useAI)
            {
                isFiring = true;
            }
        }

        private float _lastFireTimePrimary = -0.5f;
        private float _lastFireTimeSecondary = -0.5f;

        private void Update()
        {
            if (!_player) return;

            switch (isFiring)
            {
                case true when _firingCoroutine == null:
                {
                    if (_lastFireTimePrimary < 0f || Time.time - _lastFireTimePrimary >= baseFiringRate)
                    {
                        _lastFireTimePrimary = Time.time;
                        Fire();
                    }

                    break;
                }
                case false when _firingCoroutine != null:
                    StopCoroutine(_firingCoroutine);
                    _firingCoroutine = null;
                    break;
            }

            switch (isFiringSecondary)
            {
                case true when _firingCoroutineSecondary == null:
                {
                    if (_lastFireTimeSecondary < 0f || Time.time - _lastFireTimeSecondary >= baseFiringRateSecondary)
                    {
                        _lastFireTimeSecondary = Time.time;
                        Fire();
                    }

                    break;
                }
                case false when _firingCoroutineSecondary != null:
                    StopCoroutine(_firingCoroutineSecondary);
                    _firingCoroutineSecondary = null;
                    break;
            }
        }

        private void Fire()
        {
            switch (isFiring)
            {
                case true when _firingCoroutine == null:
                    _firingCoroutine = StartCoroutine(FireContinuouslyPrimary());
                    break;
                case false when _firingCoroutine != null:
                    StopCoroutine(_firingCoroutine);
                    _firingCoroutine = null;
                    break;
            }

            switch (isFiringSecondary)
            {
                case true when _firingCoroutineSecondary == null:
                    _firingCoroutineSecondary = StartCoroutine(FireContinuouslySecondary());
                    break;
                case false when _firingCoroutineSecondary != null:
                    StopCoroutine(_firingCoroutineSecondary);
                    _firingCoroutineSecondary = null;
                    break;
            }
        }


        private IEnumerator FireContinuouslyPrimary()
        {
            while (_player)
            {
                if (_player)
                {
                    var transform1 = _player.transform;
                    var playerPosition = transform1.position;
                    var playerRotation = transform1.rotation;

                    GameObject instance;
                    if (!useAI && _energy.PayEnergyCost(energyCost, "Shooting"))
                    {
                        //Get Mouse position to calculate bullet direction
                        instance = Instantiate(projectilePrefab, transform.position , playerRotation);
                        _audioPlayer.PlayMainWeaponClip();

                        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();

                        if (rb != null)
                        {
                            rb.velocity = transform.up * projectileSpeed;
                        }

                        Destroy(instance, projectileLifetime);
                    }
                    else if (useAI)
                    {
                        instance = Instantiate(projectilePrefab, transform.position,
                            Vector3ToQuaternion(playerPosition));
                        _audioPlayer.PlayEnemyLaserClip();
                        ;

                        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();

                        if (rb != null)
                        {
                            rb.velocity = transform.up * projectileSpeed;
                        }

                        Destroy(instance, projectileLifetime);
                    }

                    var timeToNextProjectile = Random.Range(baseFiringRate - firingRateVariance,
                        baseFiringRate + firingRateVariance);

                    timeToNextProjectile = Mathf.Clamp(timeToNextProjectile, minimumFiringRate, float.MaxValue);

                    yield return new WaitForSeconds(timeToNextProjectile);
                }


            }
        }

        private IEnumerator FireContinuouslySecondary()
        {
            while (_player)
            {
                if (_player)
                {
                    
                    var transform1 = _player.transform;
                    var playerPosition = transform1.position;
                    var playerRotation = transform1.rotation;

                    GameObject instanceLeft, instanceRight;
                    if (!useAI && _health.PayLifeCost(lifeCostSecondary, "Shooting"))
                    {
                        // Get the position of the left and right sides of the player
                        var leftPosition = playerPosition + (-transform1.right * 0.05f);
                        var rightPosition = playerPosition + (transform1.right * 0.05f);

                        // Instantiate bullets at the left and right positions with the same rotation as the player
                        instanceLeft = Instantiate(projectilePrefabSecondary, leftPosition, playerRotation);
                        instanceRight = Instantiate(projectilePrefabSecondary, rightPosition, playerRotation);

                        // Play the sound effect for shooting
                        _audioPlayer.PlaySecondaryWeaponClip();

                        // Set the velocity of the bullets to move upwards
                        var rbLeft = instanceLeft.GetComponent<Rigidbody2D>();
                        var rbRight = instanceRight.GetComponent<Rigidbody2D>();

                        if (rbLeft != null && rbRight != null)
                        {
                            rbLeft.velocity = transform.up * projectileSpeedSecondary;
                            rbRight.velocity = transform.up * projectileSpeedSecondary;
                        }

                        // Destroy the bullets after a set amount of time
                        Destroy(instanceLeft, projectileLifetimeSecondary);
                        Destroy(instanceRight, projectileLifetimeSecondary);
                    }
                    
                    
                    /*
                    var transform1 = _player.transform;
                    var playerRotation = transform1.rotation;

                    GameObject instance;
                    if (!useAI && _energy.PayEnergyCost(energyCostSecondary, "Shooting"))
                    {
                        //Get Mouse position to calculate bullet direction
                        instance = Instantiate(projectilePrefabSecondary, transform.position, playerRotation);

                        //Secondary sound effect
                        _audioPlayer.PlayRedLaserClip();

                        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();

                        if (rb != null)
                        {
                            rb.velocity = transform.up * projectileSpeedSecondary;
                        }

                        Destroy(instance, projectileLifetimeSecondary);
                        
                    }*/
                }


                var timeToNextProjectile = baseFiringRateSecondary;
                yield return new WaitForSeconds(timeToNextProjectile);

            }
        }


        public Quaternion Vector3ToQuaternion(Vector3 vector)
        {
            Vector3 vectorToTarget = vector - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

            return q;
        }
    }
}
