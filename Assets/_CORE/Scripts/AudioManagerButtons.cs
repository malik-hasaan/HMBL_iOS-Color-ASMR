using UnityEngine;

public class AudioManagerButtons : MonoBehaviour
{
    public AudioClip musicTapSound;
    private AudioSource audioSource; 
    
    public AudioClip musicTapSoundColor;

    public static AudioManagerButtons _inst;

    private void Awake()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();

        // Check if AudioSource component exists, if not add one
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Start()
    {
        _inst = this;
    }

    // Function to play music tap sound
    public void PlayMusicTap()
    {

        if (PlayerPrefs.GetInt("CanPlayMusic", 1) == 1)
        {     // Check if the musicTapSound is assigned
            if (musicTapSound != null && audioSource != null)
            {
                // Play the music tap sound
                audioSource.PlayOneShot(musicTapSound);
            }
            else
            {
                Debug.LogWarning("Music tap sound or AudioSource component is not assigned.");
            }

        }
        else
        {
            Debug.Log("Music tap sound or AudioSource component is not assigned.");
        }
    }  
    
    public void PlayMusicTapColor()
    {

        if (PlayerPrefs.GetInt("CanPlayMusic", 1) == 1)
        {     // Check if the musicTapSound is assigned
            if (musicTapSoundColor != null && audioSource != null)
            {
                // Play the music tap sound
                audioSource.PlayOneShot(musicTapSoundColor);
            }
            else
            {
                Debug.LogWarning("Music tap sound or AudioSource component is not assigned.");
            }

        }
        else
        {
            Debug.Log("Music tap sound or AudioSource component is not assigned.");
        }
    }
}
