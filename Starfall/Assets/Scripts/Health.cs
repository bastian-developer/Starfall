using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    [SerializeField] private int score;
    [SerializeField] private int health = 100;
    [SerializeField] private ParticleSystem hitEffect;

    [SerializeField] private bool applyCameraShake;
    
    private CameraShake _cameraShake;
    
    private AudioPlayer _audioPlayer;

    private ScoreKeeper _scoreKeeper;

    private LevelManager _levelManager;
    
    private Animator _animator;

    public int GetHealth()
    {
        return health;
    }
    
    private void Awake()
    {
        _cameraShake = Camera.main.GetComponent<CameraShake>();
        _audioPlayer = FindObjectOfType<AudioPlayer>();
        _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        _levelManager = FindObjectOfType<LevelManager>();
        _animator = FindObjectOfType<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();

        if (damageDealer != null)
        {
            TakeDamage(damageDealer.GetDamage());
            PlayHitEffect();
            ShakeCamera();
            _animator.SetTrigger("Hit");
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
        health -= damage;
        if (health <= 0)
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
