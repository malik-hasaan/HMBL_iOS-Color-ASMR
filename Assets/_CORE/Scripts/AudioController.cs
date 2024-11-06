using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public AudioMixer MainMixer;

    void Start()
    {
        SetSFX();

        SetMusic();
    }

    public void SetSFX()
    {
        MainMixer.SetFloat("sfxVolume", 0);

    }

    public void SetMusic()
    {
        MainMixer.SetFloat("musicVolume", 0);
    }
}
