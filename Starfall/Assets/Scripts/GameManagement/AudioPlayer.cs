using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace GameManagement
{
    public class AudioPlayer : MonoBehaviour
{
    [Header("Shooting")] 
    [SerializeField] private AudioClip enemyLaserClip;
    [SerializeField] [Range(0f, 1f)] private float enemyLaserVolume = 1f;
    [SerializeField] private AudioClip mainWeaponClip;
    [SerializeField] [Range(0f, 1f)] private float mainWeaponVolume = 1f;
    [SerializeField] private AudioClip secondaryWeaponClip;
    [SerializeField] [Range(0f, 1f)] private float secondaryWeaponVolume = 1f;
    
    [Header("Powers")] 
    [SerializeField] private AudioClip shieldClip;
    [SerializeField] [Range(0f, 1f)] private float shieldClipVolume = 1f;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] [Range(0f, 1f)] private float explosionClipVolume = 1f;

    [Header("Items")] 
    [SerializeField] private AudioClip energyBatteryClip;
    [SerializeField] [Range(0f, 1f)] private float energyBatteryVolume = 1f;
    [SerializeField] private AudioClip lifeBatteryClip;
    [SerializeField] [Range(0f, 1f)] private float lifeBatteryVolume = 1f;
    [SerializeField] private AudioClip bombClip;
    [SerializeField] [Range(0f, 1f)] private float bombClipVolume = 1f;
    
    [Header("Damage")] 
    [SerializeField] private AudioClip playerDamageClip;
    [SerializeField] [Range(0f, 1f)] private float playerDamageVolume = 1f;
    [SerializeField] private AudioClip alienDamageClip;
    [SerializeField] [Range(0f, 1f)] private float alienDamageVolume = 1f;

    [Header("Menu")] 
    [SerializeField] private AudioClip buttonPressedClip;
    [SerializeField] [Range(0f, 1f)] private float buttonPressedVolume = 1f;
    [SerializeField] private AudioClip buttonHoverClip;
    [SerializeField] [Range(0f, 1f)] private float buttonHoverVolume = 1f;
    
    //Singleton
    private static AudioPlayer _audioInstance;
    
    public AudioPlayer GetInstance()
    {
        return _audioInstance;
    }

    private void Awake()
    {
        ManageSingleton();
    }

    void ManageSingleton()
    {
        if(_audioInstance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            _audioInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayBombClip()
    {
        PlayClip(bombClip, bombClipVolume);
    }
    
    public void PlayExplosionClip()
    {
        PlayClip(explosionClip, explosionClipVolume);
    }

    public void PlayEnergyBatteryClip()
    {
        PlayClip(energyBatteryClip, energyBatteryVolume);
    }

    public void PlayLifeBatteryClip()
    {
        PlayClip(lifeBatteryClip, lifeBatteryVolume);
    }
    
    public void PlayShieldClip()
    {
        PlayClip(shieldClip, shieldClipVolume);
    }
    
    public void PlayEnemyLaserClip()
    {
        PlayClip(enemyLaserClip, enemyLaserVolume);
    }
    
    public void PlayMainWeaponClip()
    {
        PlayClip(mainWeaponClip, mainWeaponVolume);
    }
    
    public void PlaySecondaryWeaponClip()
    {
        PlayClip(secondaryWeaponClip, secondaryWeaponVolume);
    }

    public void PlayAlienDamageClip()
    {
        PlayClip(alienDamageClip, alienDamageVolume);
    }
    
    public void PlayPlayerDamageClip()
    {
        PlayClip(playerDamageClip, playerDamageVolume);
    }
    
    public void PlayButtonPressedClip()
    {
        PlayClip(buttonPressedClip, buttonPressedVolume);
    }
    
    public void PlayButtonHoverClip()
    {
        PlayClip(buttonHoverClip, buttonHoverVolume);
    }
    
    void PlayClip(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            AudioSource.PlayClipAtPoint(clip, cameraPos, volume);
        }
    }
}
}