using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAudio : MonoBehaviour
{
    public AudioClip audioClip1;
    public AudioClip audioClip2;

    public float detectionRadius = 100f;

    public float detectionRadiusProche = 50f;
    private AudioSource audioSourceLoin;
    private AudioSource audioSourceProche;
    private Transform playerTransform;

    void Start()
    {
        audioSourceLoin = GetComponent<AudioSource>();
        audioSourceLoin.clip = audioClip1;
        audioSourceLoin.loop = true;
        audioSourceLoin.spatialBlend = 1.0f; // 3D sound
        audioSourceLoin.maxDistance = detectionRadius;
        audioSourceLoin.Play(); // Start playing the sound in loop

        audioSourceProche = gameObject.AddComponent<AudioSource>();
        audioSourceProche.clip = audioClip2;
        audioSourceProche.loop = true;
        audioSourceProche.spatialBlend = 1.0f; // 3D sound
        audioSourceProche.maxDistance = detectionRadiusProche;
        audioSourceProche.volume = 0f;
        audioSourceProche.Play(); // Start playing the sound in loop
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
            if (distance <= detectionRadiusProche)
            {
                audioSourceLoin.volume = 0f; // Mute the audio if out of range
            
                audioSourceProche.volume = 1.0f - (distance / detectionRadius); // Adjust volume based on distance
            }
            else if (distance <= detectionRadius)
            {
                audioSourceLoin.volume = 1.0f - (distance / detectionRadius); // Adjust volume based on distance
                audioSourceProche.volume = 0f; // Mute the audio if out of range
            }
            else
            {
                audioSourceLoin.volume = 0f; // Mute the audio if out of range
                audioSourceProche.volume = 0f; // Mute the audio if out of range
            }
        }
    }
}