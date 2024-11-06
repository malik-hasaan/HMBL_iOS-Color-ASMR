using UnityEngine;

public class ToolAudioPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ToolAudio"))
        {
            if (audioSource != null)
                audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ToolAudio"))
        {
            if (audioSource != null)
                audioSource.Pause();
        }
    }
}
