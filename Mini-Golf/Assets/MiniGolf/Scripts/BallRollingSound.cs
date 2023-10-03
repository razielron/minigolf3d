using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRollingSound : MonoBehaviour
{
   // public AudioClip rollingSound;
    public float volumeMultiplier = 1.0f;

    private Rigidbody rb;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = gameObject.GetComponent<AudioSource>();
       // audioSource.clip = rollingSound;
        audioSource.loop = true;
        audioSource.volume = 0.0f;  // Start with zero volume
        audioSource.Play();
    }

    void Update()
    {
        // Check if the ball is moving
        if (rb.velocity.magnitude > 0.1f)
        {
            // Adjust the volume based on the ball's speed
            float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / 10.0f);  // Adjust 10.0f as needed
            audioSource.volume = speedFactor * volumeMultiplier;
        }
        else
        {
            // If the ball is not moving, stop the sound
            audioSource.volume = 0.0f;
        }
    }
}
