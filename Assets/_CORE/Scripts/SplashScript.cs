using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScript : MonoBehaviour
{
    public static SplashScript instance;
 
    [SerializeField]
    float displayTime = 2.5f;

    [SerializeField]
    Image loadingBar;

    [Space()]
    [Space()]
    public Animator pencil;

    [Space()]
    [SerializeField]
    GameObject InterNetPopUp;

    bool isInternet = false;

    [Space()]
    public GameObject UmpManager;

    void Awake()
    {
        instance = this;

    //    try
    //    {
    //        if (SystemInfo.systemMemorySize <= 1024)
    //        {
    //            PlayerPrefs.SetInt("NoAds", 1);
    //            Application.Quit();
    //        }
    //        else if (SystemInfo.systemMemorySize <= 2048)
    //        {
    //            PlayerPrefs.SetInt("MaxAdStop", 1);
    //        }
    //    }
    //    catch { }

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
        pencil.Play("PencilFilling");

        Invoke(nameof(loadit), displayTime);
    }

    void loadit()
    {
        SceneManager.LoadScene(1);
    }
}
