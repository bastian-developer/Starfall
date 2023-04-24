using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Header("Shooting")] 
    [SerializeField] AudioClip greenLaserClip;
    [SerializeField] [Range(0f, 1f)] float greenLaserVolume = 1f;
    
    [SerializeField] AudioClip redLaserClip;
    [SerializeField] [Range(0f, 1f)] float redLaserVolume = 1f;

    [Header("Damage")] 
    [SerializeField] AudioClip playerDamageClip;
    [SerializeField] [Range(0f, 1f)] float playerDamageVolume = 1f;
    
    [SerializeField] AudioClip alienDamageClip;
    [SerializeField] [Range(0f, 1f)] float alienDamageVolume = 1f;

    [Header("Menu")] 
    [SerializeField] AudioClip buttonPressedClip;
    [SerializeField] [Range(0f, 1f)] float buttonPressedVolume = 1f;
    [SerializeField] AudioClip buttonHoverClip;
    [SerializeField] [Range(0f, 1f)] float buttonHoverVolume = 1f;
    
    
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

    public void PlayGreenLaserClip()
    {
        PlayClip(greenLaserClip, greenLaserVolume);
    }
    
    public void PlayRedLaserClip()
    {
        PlayClip(redLaserClip, redLaserVolume);
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