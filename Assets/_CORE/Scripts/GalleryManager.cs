using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GalleryManager : MonoBehaviour
{
    public static GalleryManager Instance;

    [Space()]
    public Image loadingBar;

    [Space()]
    [Space()]
    public Animator pencil;

    [Space()]
    [Space()]
    bool isInternet = false;
    public GameObject InterNetPopUp;

    // Define the colors
    private Color[] colors = new Color[]
    {
        new Color(0.231f, 0.702f, 1.0f),  // #3bb3ff
        new Color(1.0f, 0.231f, 0.898f),  // #ff3be5
        new Color(0.514f, 0.161f, 0.910f) // #8329e8
    };

    [Space()]
    [Header("----------------- Gallery LEVEL -----------------")]
    [Space()]
    public LevelManagerGallery[] GalleryLevels;

    [Header("----------------- Text_Sprites -----------------")]
    public Animator[] TextSprites;

    [Space()]
    [Header("----------------- Complete Panel -----------------")]
    [Space()]
    public GameObject Complete_Panel;
    public Image Complete_Panel_Image;

    [Space()]
    [Header("----------------- UI -----------------")]
    [Space()]
    public GameObject UI_Canvas;

    public AudioClip SoundClipParticle;
    private AudioSource AudioSourceParticle;

    [Space()]
    [Header("------------------------ PENS COLOR --------------------------")]
    [Space()]
    public SpriteRenderer[] penBg;
    public SpriteRenderer[] penBody;
    public SpriteRenderer[] penColorShadow;

    [Space()]
    [Header("------------------------ PENS LIST --------------------------")]
    public Sprite[] _PenBody;
    public Sprite[] _PenBg;
    public Sprite[] _PenShadow;

    public int index;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ChangePenFunction();

        for (int i = 0; i < GalleryLevels.Length; i++)
        {
            GalleryLevels[i].gameObject.SetActive(false);
        }

        index = GameManager.selectedGalleryIndex;

        if (index >= 0 && index < GalleryLevels.Length)
        {
            GalleryLevels[index].gameObject.SetActive(true);

            try
            {
                Statics.GA_LevelStartEvent("GalleryLevel" + (index + 1));
            }
            catch
            {
            }

            //AppmetricaAnalytics.ReportCustomEvent(AnalyticsType.GameData, "GalleryLevel", $"Level_{index + 1}", "Start");
        }

        else
        {
            Debug.Log("___Index out of range: " + index);
        }
    }

    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && !isInternet)
        {
            InterNetPopUp.SetActive(true);

            isInternet = true;
        }
    }

    public void CheckInterNet()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            InterNetPopUp.SetActive(false);
            AudioManager.instance.PlayBgSound_1();
        }
        else
        {
            InterNetPopUp.SetActive(true);
        }
    }

    public void LoadingBar()
    {
        //loadingBar.DOFillAmount(1, 3.1f).OnComplete(delegate
        //{
        //    loadingBar.DOFillAmount(0, 0f);
        //});

        pencil.Play("PencilFilling");
    }

    public void Complete()
    {
        try
        {
            Statics.GA_LevelCompleteEvent("GalleryLevel" + (GameManager.selectedGalleryIndex + 1));
        }
        catch
        {
        }
        
        //AppmetricaAnalytics.ReportCustomEvent(AnalyticsType.GameData, "GalleryLevel" , $"Level_{GameManager.selectedGalleryIndex + 1}", "Complete");


        if (GameManager.selectedGalleryIndex == 0)
        {
            PlayerPrefs.SetInt("Color1", 1);

        }
        if (GameManager.selectedGalleryIndex == 1)
        {
            PlayerPrefs.SetInt("Color2", 2);

        }
        if (GameManager.selectedGalleryIndex == 2)
        {
            PlayerPrefs.SetInt("Color3", 3);

        }
        if (GameManager.selectedGalleryIndex == 3)
        {
            PlayerPrefs.SetInt("Color4", 4);

        }
        if (GameManager.selectedGalleryIndex == 4)
        {
            PlayerPrefs.SetInt("Color5", 5);

        }


        AdCaller._inst.callads();

        //     gamePaused = true;

        UI_Canvas.SetActive(false);

        Complete_Panel.SetActive(true);

        Complete_Panel_Image.color = colors[Random.Range(0, colors.Length)];

        if (PlayerPrefs.GetInt("CanPlayMusic", 1) == 1)
        {
            if (AudioSourceParticle == null)
            {
                AudioSourceParticle = gameObject.AddComponent<AudioSource>();
            }
            AudioSourceParticle.PlayOneShot(SoundClipParticle);
        }

    }

    public void ShowTextSprite(Vector3 pos)
    {
        pos.z = 0;

        ResetTextSprites();

        int randomNumber = Random.Range(0, TextSprites.Length);

        TextSprites[randomNumber].transform.parent.transform.localPosition = pos;

        TextSprites[randomNumber].gameObject.SetActive(true);

        TextSprites[randomNumber].Play("TextPop");

        Invoke(nameof(ResetTextSprites), 3f);

    }

    void ResetTextSprites()
    {
        for (int i = 0; i < TextSprites.Length; i++)
        {
            TextSprites[i].gameObject.SetActive(false);

            TextSprites[i].transform.localPosition = Vector3.zero;
        }
    }

    public void ChangePenFunction()
    {

        int pen = PlayerPrefs.GetInt("PenImage");

        for (int i = 0; i < penBg.Length; i++)
        {
            if (pen < _PenBg.Length)
            {
                penBg[i].sprite = _PenBg[pen];
            }
        }

        for (int i = 0; i < penBody.Length; i++)
        {
            if (pen < _PenBody.Length)
            {
                penBody[i].sprite = _PenBody[pen];
            }
        }

        for (int i = 0; i < penColorShadow.Length; i++)
        {
            if (pen < _PenShadow.Length)
            {
                penColorShadow[i].sprite = _PenShadow[pen];
            }
        }
    }

    public void BackPressed()
    {
        SceneManager.LoadScene("Main");
    }

    public void NextPressed()
    {
        PlayerPrefs.SetInt("ShowGalleryPanel", 1);

        AdCaller._inst.callads();

        SceneManager.LoadScene("Main");
    }
}
