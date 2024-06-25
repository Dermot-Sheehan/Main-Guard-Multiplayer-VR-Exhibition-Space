using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayButton : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip audioClip;  

    void Start()
    {
       
        audioSource = GetComponent<AudioSource>();

        
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from this GameObject. Please add an AudioSource component.");
        }
    }

    public void PlayAudioClip()
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;  
            audioSource.Play(); 
        }
        else
        {
            if (audioSource == null)
            {
                Debug.LogError("AudioSource is not initialized.");
            }
            if (audioClip == null)
            {
                Debug.LogError("AudioClip is not assigned.");
            }
        }
    }
}
