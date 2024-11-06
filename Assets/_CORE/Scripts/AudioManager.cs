using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] AudioSource musicSource_1;

    public static AudioManager instance;
    private float savedPlaybackTime;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        PlayBgSound_1();
    }

    public void PlayBgSound_1()
    {
        if (musicSource_1 != null)
        {
            musicSource_1.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource_1 != null)
        {
            musicSource_1.Stop();
        }
    }

    public void SavePlaybackTime()
    {
        if (musicSource_1 != null)
        {
            savedPlaybackTime = musicSource_1.time;
        }
    }

}
