using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SplashScript : MonoBehaviour
{
    [SerializeField]
    private float displayTime = 2.5f;

    [SerializeField]
    private Image loadingBar;

    [Space()]
    [Space()]
    public Animator  pencil;

    [Space()]
    [SerializeField]
    private GameObject InterNetPopUp;

    private bool isInternet = false;

    void Awake()
    {
        try
        {
            if (SystemInfo.systemMemorySize <= 1024)
            {
                PlayerPrefs.SetInt("NoAds", 1);
                Application.Quit();
            }
            else if (SystemInfo.systemMemorySize <= 2048)
            {
                PlayerPrefs.SetInt("MaxAdStop", 1);
            }
        }
        catch { }
    }

    void Start()
    {
        PlayerPrefs.SetInt("CanPlayMusic", 1);
        PlayerPrefs.SetInt("CanPlaySounds", 1);
        PlayerPrefs.SetInt("Haptic", 1);
        PlayerPrefs.SetInt("ShowGalleryPanel", 0);

        CheckInternetStatus();
        InvokeRepeating(nameof(CheckInternetStatus), 0f, 5f);
    }

    void CheckInternetStatus()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            InterNetPopUp.SetActive(true);
            if (AudioManager.instance != null)
            {
                AudioManager.instance.StopMusic();
            }
        }
        else
        {
            InterNetPopUp.SetActive(false);
            AudioManager.instance.PlayBgSound_1();
            SceneLoad();
        }

    }

    public void CheckInterNet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            InterNetPopUp.SetActive(true);
            if (AudioManager.instance != null)
            {
                AudioManager.instance.StopMusic();
            }
        }
        else
        {
            InterNetPopUp.SetActive(false);
            AudioManager.instance.PlayBgSound_1();
            SceneLoad();
        }
    }

    void SceneLoad()
    {
        //loadingBar.DOFillAmount(1, displayTime).OnComplete(() =>
        //{
        //    SceneManager.LoadScene(1);
        //});

        pencil.Play("PencilFilling");

        Invoke(nameof(loadit), displayTime);
    }

    void loadit()
    {
        SceneManager.LoadScene(1);
    }
}
