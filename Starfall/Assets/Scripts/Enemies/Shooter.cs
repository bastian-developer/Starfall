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
        [SerializeField] private int energyCostSecondary;


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
                    _firingCoroutine = StartCoroutine(FireContinuoslyPrimary());
                    break;
                case false when _firingCoroutine != null:
                    StopCoroutine(_firingCoroutine);
                    _firingCoroutine = null;
                    break;
            }

            switch (isFiringSecondary)
            {
                case true when _firingCoroutineSecondary == null:
                    _firingCoroutineSecondary = StartCoroutine(FireContinuoslySecondary());
                    break;
                case false when _firingCoroutineSecondary != null:
                    StopCoroutine(_firingCoroutineSecondary);
                    _firingCoroutineSecondary = null;
                    break;
            }
        }


        IEnumerator FireContinuoslyPrimary()
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
                        instance = Instantiate(projectilePrefab, transform.position, playerRotation);
                        _audioPlayer.PlayRedLaserClip();

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
                        _audioPlayer.PlayGreenLaserClip();
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

        IEnumerator FireContinuoslySecondary()
        {
            while (_player)
            {
                if (_player)
                {
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
                    }
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
