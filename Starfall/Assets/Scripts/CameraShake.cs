using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    [Header("Small Shake")] 
    [SerializeField] private float shakeDuration;

    [SerializeField] private float shakeMagnitude;

    [Header("Bomb Shake")]
    [SerializeField] private  float bombShakeDuration;
    [SerializeField] private float bombShakeMagnitude;

    // The initial position of the object
    private Vector3 _initialPosition;


    // Sets the initial position of the object
    private void Start()
    {
        _initialPosition = transform.position;
    }

    // Plays the shake animation for the main camera
    public void Play()
    {
        StartCoroutine(Shake(shakeDuration, shakeMagnitude));
    }

    // Plays the shake animation for the bomb explosion
    public void PlayBomb()
    {
        StartCoroutine(Shake(bombShakeDuration, bombShakeMagnitude));
    }

    // Coroutine for the shake animation
    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsedTime = 0;

        // Keep shaking until the duration has elapsed
        while (elapsedTime < duration)
        {
            // Calculate the position to shake to
            Vector3 shakePosition = _initialPosition + (Vector3)Random.insideUnitCircle * magnitude;

            // Apply the shake position to the object's transform
            transform.position = shakePosition;

            // Increment the elapsed time and wait for the end of the frame
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Reset the object's position to its initial position
        transform.position = _initialPosition;
    }
}