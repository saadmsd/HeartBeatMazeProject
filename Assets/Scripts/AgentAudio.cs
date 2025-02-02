using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAudio : MonoBehaviour
{
    public AudioClip audioClip;
    public float detectionRadius = 10f;
    private AudioSource audioSource;
    private Transform playerTransform;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.spatialBlend = 1.0f; // 3D sound
        audioSource.maxDistance = detectionRadius;
        audioSource.Play(); // Start playing the sound in loop

        // Assuming the player has a tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= detectionRadius)
            {
                audioSource.volume = 1.0f - (distance / detectionRadius); // Adjust volume based on distance
            }
            else
            {
                audioSource.volume = 0f; // Mute the audio if out of range
            }
        }
    }
}