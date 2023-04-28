using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    [SerializeField] private int score;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int passiveHealthRestoration = 1;
    [SerializeField] private float passiveHealthRestorationDelay = 1;
    
    [SerializeField] private ParticleSystem hitEffect;

    [SerializeField] private bool applyCameraShake;

    private int _currentHealth;

    private CameraShake _cameraShake;
    
    private AudioPlayer _audioPlayer;

    private ScoreKeeper _scoreKeeper;

    private LevelManager _levelManager;
    
    private Animator _animator;
    
    private Coroutine _restoreHealthCoroutine;


    public int GetCurrentHealth()
    {
        return _currentHealth;
    }
    
    private void Awake()
    {
        _cameraShake = Camera.main.GetComponent<CameraShake>();
        _audioPlayer = FindObjectOfType<AudioPlayer>();
        _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        _levelManager = FindObjectOfType<LevelManager>();
        _animator = FindObjectOfType<Animator>();
        _currentHealth = maxHealth;
    }

    public bool _shouldRestoreHealth()
    {
        if (_currentHealth < maxHealth)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Update()
    {
        if (isPlayer && _shouldRestoreHealth() && _restoreHealthCoroutine == null)
        {
            _restoreHealthCoroutine = StartCoroutine(AddHealthOverTime());
            
        }else if (isPlayer && !_shouldRestoreHealth() && _restoreHealthCoroutine != null)
        {
            StopCoroutine(_restoreHealthCoroutine);
            _restoreHealthCoroutine = null;
        }
    }

    IEnumerator AddHealthOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(passiveHealthRestorationDelay);
            _currentHealth += passiveHealthRestoration;
            
            Debug.Log("Passive Health " + passiveHealthRestoration);
            Debug.Log("Current Health " + _currentHealth);


        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();

        if (damageDealer != null)
        {
            TakeDamage(damageDealer.GetDamage());
            PlayHitEffect();
            ShakeCamera();

            if (isPlayer)
            {
                _animator.SetTrigger("Hit");
            }
            
            damageDealer.Hit();
        }
    }

    void ShakeCamera()
    {
        if (_cameraShake && applyCameraShake)
        {
            _cameraShake.Play();
        }
    }
    void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (!isPlayer)
        {
            _scoreKeeper.ModifyScore(score);
        }
        else
        {
            _levelManager.LoadGameOver();
        }
        Destroy(gameObject);
    }

    void PlayHitEffect()
    {
        if (hitEffect != null)
        {
            ParticleSystem instance = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);

            if (isPlayer)
            {
                _audioPlayer.PlayPlayerDamageClip();
            }
            else
            {
                _audioPlayer.PlayAlienDamageClip();
            }
            
        }
    }
}
