using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class MainScript : MonoBehaviour
{
    public static MainScript instance;

    bool isInternet = false;
    public GameObject InterNetPopUp;

    [Space()]
    public Image loadingBar;

    [Space()]
    public Animator pencil;

    [Space()]
    [Space()]
    [Header("----------------- Pens Shop Panel: -----------------")]
    public GameObject Pens_Panel;
    public GameObject PensPopUp;
    public Transform scrollAnim;
    public Transform scrollAnim2;

    private CanvasGroup scrollAnimCanvasGroup;

    [Space()]
    [Header("----------------- FOR SETTING ONLY -----------------")]

    public Image SoundImage;
    public GameObject SoundOff;

    public Image MusicImage;
    public GameObject MusicOff;

    public Image VibrationImage;
    public GameObject VibrationOff;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        InitializeSoundButton();
        InitializeMusicButton();
        InitializeVibrationButton();

        scrollAnimCanvasGroup = scrollAnim.GetComponent<CanvasGroup>();
        if (scrollAnimCanvasGroup == null)
        {
            scrollAnimCanvasGroup = scrollAnim.gameObject.AddComponent<CanvasGroup>();
        }

        scrollAnimCanvasGroup.alpha = 0;

        try
        {
            CheckInternetStatus();
        }
        catch { }
    }

    void Update()
    {
        try
        {
            CheckInternetStatus();
        }
        catch { }
    }


    PaintDragNew PDN;
    void CheckInternetStatus()
    {
        bool currentInternetStatus = Application.internetReachability != NetworkReachability.NotReachable;

        if (currentInternetStatus != isInternet)
        {
            isInternet = currentInternetStatus;

            if (isInternet)
            {
                InterNetPopUp.SetActive(false);
                GameManager.Instance.gamePaused = false;
                if (PlayerPrefs.GetInt("CanPlaySounds", 1) == 1)
                {
                    AudioManager.instance.SavePlaybackTime();
                }
                try
                {
                    if (ToolMoveOnSpline.instance.audioSource != null && ToolMoveOnSpline.instance.audioSource.isPlaying) ToolMoveOnSpline.instance.audioSource.Stop();
                    if (ToolMoveOnSpline.instance.particle != null) ToolMoveOnSpline.instance.particle.Stop();
                    ToolMoveOnSpline.instance.scratching = false;
                    ToolMoveOnSpline.instance.Tool.follow = false;
                    ToolMoveOnSpline.instance.isVibrating = false;

                    if (PaintDragNew._instance.audioSource != null) PaintDragNew._instance.audioSource.Stop();
                    PDN = LevelManager.instance.AllColors[LevelManager.instance.currentColor].Tool.GetComponent<PaintDragNew>();
                    PDN.is_dragable = true;
                    GameManager.Instance.canPaint = true;
                }
                catch { }
            }
            else
            {
                InterNetPopUp.SetActive(true);
                GameManager.Instance.gamePaused = true;

                SoundOff.SetActive(false);
                PlayerPrefs.SetInt("CanPlaySounds", 0);
                AudioManager.instance.StopMusic();

                try
                {
                    if (ToolMoveOnSpline.instance.audioSource != null && ToolMoveOnSpline.instance.audioSource.isPlaying) ToolMoveOnSpline.instance.audioSource.Stop();
                    if (ToolMoveOnSpline.instance.particle != null) ToolMoveOnSpline.instance.particle.Stop();
                    ToolMoveOnSpline.instance.scratching = false;
                    ToolMoveOnSpline.instance.Tool.follow = false;
                    ToolMoveOnSpline.instance.isVibrating = false;

                    if (PaintDragNew._instance.audioSource != null) PaintDragNew._instance.audioSource.Stop();
                    PDN = LevelManager.instance.AllColors[LevelManager.instance.currentColor].Tool.GetComponent<PaintDragNew>();
                    PDN.is_dragable = false;
                    GameManager.Instance.canPaint = false;
                }
                catch { }
            }
        }
    }

    public void CheckInterNet()
    {
        GameManager.Instance.VibrateIt();

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            InterNetPopUp.SetActive(false);
            GameManager.Instance.gamePaused = false;

            if (PlayerPrefs.GetInt("CanPlaySounds", 1) == 1)
                AudioManager.instance.SavePlaybackTime();
        }
        else
        {
            InterNetPopUp.SetActive(true);
        }
    }

    void InitializeSoundButton()
    {
        int canPlaySounds = PlayerPrefs.GetInt("CanPlaySounds", 1);

        SoundOff.SetActive(canPlaySounds == 1 ? false : true);
    }

    void InitializeMusicButton()
    {
        int canPlayMusic = PlayerPrefs.GetInt("CanPlayMusic", 1);

        MusicOff.SetActive(canPlayMusic == 1 ? false : true);
    }

    void InitializeVibrationButton()
    {
        int haptic = PlayerPrefs.GetInt("Haptic", 1);

        VibrationOff.SetActive(haptic == 1 ? false : true);
    }

    public void SoundButton()
    {
        if (PlayerPrefs.GetInt("CanPlaySounds", 0) == 1)
        {
            SoundOff.SetActive(false);

            PlayerPrefs.SetInt("CanPlaySounds", 0);

            AudioManager.instance.StopMusic();
        }

        else
        {
            SoundOff.SetActive(true);

            PlayerPrefs.SetInt("CanPlaySounds", 1);

            AudioManager.instance.PlayBgSound_1();
        }

        GameManager.Instance?.VibrateIt();
    }

    public void MusicButton()
    {
        if (PlayerPrefs.GetInt("CanPlayMusic", 0) == 1)
        {
            MusicOff.SetActive(false);

            PlayerPrefs.SetInt("CanPlayMusic", 0);
        }
        else
        {
            MusicOff.SetActive(true);

            PlayerPrefs.SetInt("CanPlayMusic", 1);
        }

        GameManager.Instance?.VibrateIt();
    }

    public void VibrationButton()
    {
        if (PlayerPrefs.GetInt("Haptic", 0) == 1)
        {
            VibrationOff.SetActive(false);

            PlayerPrefs.SetInt("Haptic", 0);
        }

        else
        {
            VibrationOff.SetActive(true);

            PlayerPrefs.SetInt("Haptic", 1);
        }

        GameManager.Instance?.VibrateIt();
    }

    public void SceneLoad()
    {
        //loadingBar.DOFillAmount(1, 3.1f).OnComplete(delegate
        //{
        //    loadingBar.DOFillAmount(0, 2f);
        //});

        pencil.Play("PencilFilling");
    }

    public void OpenPensPanel()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.gamePaused = true;

        try
        {
            if (ToolMoveOnSpline.instance.audioSource != null) ToolMoveOnSpline.instance.audioSource.Stop();
            if (ToolMoveOnSpline.instance.particle != null) ToolMoveOnSpline.instance.particle.Stop();
            ToolMoveOnSpline.instance.scratching = false;
            ToolMoveOnSpline.instance.Tool.follow = false;
            ToolMoveOnSpline.instance.isVibrating = false;

            if (PaintDragNew._instance.audioSource != null) PaintDragNew._instance.audioSource.Stop();
            PDN = LevelManager.instance.AllColors[LevelManager.instance.currentColor].Tool.GetComponent<PaintDragNew>();
            PDN.is_dragable = false;
            GameManager.Instance.canPaint = false;

        }
        catch { }
        Pens_Panel.SetActive(true);

        GameManager.Instance.gamePaused = true;

        GameManager.Instance.VibrateIt();

        AudioManagerButtons._inst.PlayMusicTap();

        scrollAnim.localPosition = new Vector3(scrollAnim.localPosition.x, scrollAnim.localPosition.y - 1000f, scrollAnim.localPosition.z);
        scrollAnimCanvasGroup.alpha = 0;

        Sequence openSequence = DOTween.Sequence();
        openSequence.Append(scrollAnimCanvasGroup.DOFade(1, 0.9f));
        openSequence.Join(scrollAnim.DOLocalMove(scrollAnim2.localPosition, 0.9f).SetEase(Ease.OutQuad));
    }

    public void ClosePensPanel()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.gamePaused = false;

        AdCaller._inst.callads();
        GameManager.Instance.VibrateIt();
        Pens_Panel.SetActive(false);
        GameManager.Instance.gamePaused = false;
        AudioManagerButtons._inst.PlayMusicTap();

        try
        {
            if (ToolMoveOnSpline.instance.audioSource != null) ToolMoveOnSpline.instance.audioSource.Stop();
            if (ToolMoveOnSpline.instance.particle != null) ToolMoveOnSpline.instance.particle.Stop();
            ToolMoveOnSpline.instance.scratching = false;
            ToolMoveOnSpline.instance.Tool.follow = false;
            ToolMoveOnSpline.instance.isVibrating = false;

            if (PaintDragNew._instance.audioSource != null) PaintDragNew._instance.audioSource.Stop();
            PDN = LevelManager.instance.AllColors[LevelManager.instance.currentColor].Tool.GetComponent<PaintDragNew>();
            PDN.is_dragable = true;
            GameManager.Instance.canPaint = true;

        }
        catch { }
    }

}
