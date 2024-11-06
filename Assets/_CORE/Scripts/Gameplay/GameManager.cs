using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.NiceVibrations;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Camera MainCam;

    [Space()]
    [Space()]
    public int currentlevel = 0;

    [Space()]
    [Header("----------------- LEVEL -----------------")]
    public LevelManager thisLevel;

    [Space()]
    public GameObject[] AllLevelTattoos;

    [Space()]
    public GameObject TattooFrame;
    public Transform TattooFrameTARGET;

    [Space()]
    public AudioSource Tool1AudioSource;
    public AudioSource Tool2AudioSource;

    [Space()]
    public bool gamePaused = false;
    public bool canPaint = false;

    [Space()]
    public Animator[] TextSprites;

    [Space()]
    public GameObject RemoveTxt;

    [Space()]
    [Header("----------------- SELECTION COLORs -----------------")]

    [Space()]
    public GameObject SelectionColorsParent;
    public Transform SelectionColorsParentTARGET;
    public ColorInfo[] SelectionColorPlaceholders;

    [Space()]
    [Header("----------------- UI -----------------")]
    [Space()]
    public GameObject HandTutorial;
    public Button PaintDoneBtn;

    [SerializeField]
    public Image Fade_Img, Black_Img;

    [Space()]
    public GameObject UI_Canvas;
    public GameObject LoadingCanvas;

    [Space()]
    public Text[] CashTexts;

    [Space()]
    [Header("Complete Panel:")]
    public GameObject Complete_Panel;
    public Image Complete_Panel_Image;
    public Image Accuracy_Fill_Image;
    public Text Accuracy_Text;

    [Space()]
    [Header("Complete Reward Panel:")]
    public GameObject CompleteReward_Panel;
    public Image CompleteReward_Icon;


    [Space()]
    [Header("Settings Panel:")]
    public GameObject Settings_Panel;
    public GameObject SettingsPopUp;

    [Space()]
    [Header("Gallery Color Panel:")]
    public GameObject Gallery_Panel;
    public GameObject GalleryPopUp;
    public Transform scrollAnim;
    public Transform scrollAnim2;
    private CanvasGroup scrollAnimCanvasGroup;
    public Image ColorImage1, ColorImage2, ColorImage3, ColorImage4, ColorImage5;
    public Sprite SpriteColorImage1, SpriteColorImage2, SpriteColorImage3, SpriteColorImage4, SpriteColorImage5;
    public Image BtnImg1, BtnImg2, BtnImg3, BtnImg4, BtnImg5;
    public Image BtnTickImg1, BtnTickImg2, BtnTickImg3, BtnTickImg4, BtnTickImg5;
    public Sprite SelectSprite, UnSelectSprite;

    [Space()]
    [Header("Level Numbering:")]
    public Text LevelNum;

    [Space()]
    [Space()]
    [Header("----------------- SFX -----------------")]

    public AudioClip SoundClipParticle;

    AudioSource AudioSourceParticle;

    [Space()]
    [Header("----------------- Particles -----------------")]
    public GameObject[] ShowTextSpriteParticles;

    [Space()]
    [Header("ADObjects")]
    public GameObject[] adsObjectsForNameChange;

    [Space()]
    [Header("------------------------ PENS LIST --------------------------")]
    public Sprite[] _PenBody;
    public Sprite[] _PenBg;

    public Sprite[] _PenPlain;
    public Sprite[] _PenShadow;

    public GameObject RatingPanel;

    int displayLevelNumber;


    [Space()]
    [Space()]
    [Header("------------------------ FIREBASE NOTIFICATION --------------------------")]
    public GameObject notificationPanel;
    public Text notificationTitleText;
    public Text notificationBodyText;

    public GameObject SkipButton;

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 120;

        ResetLevels();

        for (int i = 0; i < adsObjectsForNameChange.Length - 1; i++)
        {
            adsObjectsForNameChange[i].name = "Rew" + i;
        }
    }

    IEnumerator Start()
    {
        int CheckAndShowGalleryPanelValue = PlayerPrefs.GetInt("ShowGalleryPanel");

        if (CheckAndShowGalleryPanelValue == 1)
        {

            try
            {
                OpenGalleryPanel();
            }
            catch { }

            StartCoroutine(SetNewValue());

        }

        try
        {
            scrollAnimCanvasGroup = scrollAnim.GetComponent<CanvasGroup>();

            if (scrollAnimCanvasGroup == null)
            {
                scrollAnimCanvasGroup = scrollAnim.gameObject.AddComponent<CanvasGroup>();
            }
        }
        catch { }

        yield return new WaitForSeconds(0.05f);

        Accuracy_Fill_Image.DOFillAmount(0, 0.5f);

        StartLevel();

        yield return new WaitForSeconds(3f);

        try
        {
            GadsmeAdsManager.intance.ShowGadsMeAd();
        }

        catch { }

        if (PlayerPrefs.GetInt("ButtonEnabled") == 0)
        {
            StartCoroutine(enableButton());
        }
        else
        {
            SkipButton.SetActive(true);
        }
    }

    private IEnumerator enableButton()
    {
        PlayerPrefs.SetInt("ButtonEnabled", 1);
        yield return new WaitForSeconds(45f);

        SkipButton.SetActive(true);
    }
    IEnumerator SetNewValue()
    {
        yield return new WaitForSeconds(3.1f);

        PlayerPrefs.SetInt("ShowGalleryPanel", 0);
    }

    public void ResetLeveldsdsss()
    {
        SceneManager.LoadScene("Gallery");
    }

    public void ResetLevels()
    {
        for (int i = 0; i < AllLevelTattoos.Length; i++)
        {
            AllLevelTattoos[i].SetActive(false);
        }
    }

    public void StartLevel()
    {
        currentlevel = SaveSystem.Instance.DataFields.currentLevel; //PlayerPrefs.GetInt("LEVELNUMBER", 0);

        if (currentlevel == 1)
        {
            HandTutorial.SetActive(true);
        }

        if (currentlevel > SaveSystem.Instance.DataFields.totalLevels)
        {
            currentlevel = Random.Range(1, SaveSystem.Instance.DataFields.totalLevels + 1);
        }

        int lvlToPlay = GetLevelAccordingToFirebaseOrder(currentlevel);

        GameObject Temp = Resources.Load<GameObject>("Levels/" + lvlToPlay);

        Temp = Instantiate(Temp);
        Temp.SetActive(true);

        thisLevel = Temp.GetComponent<LevelManager>();

        try
        {
            AllLevelTattoos[lvlToPlay - 1].gameObject.SetActive(true);
        }
        catch { }

        displayLevelNumber = SaveSystem.Instance.DataFields.levelNumber; //PlayerPrefs.GetInt("DISPLAY_LEVEL_NUMBER", 1);

        if (displayLevelNumber >= 10)
            LevelNum.text = "LEVEL " + displayLevelNumber.ToString();

        else
            LevelNum.text = "LEVEL 0" + displayLevelNumber.ToString();

        TattooFrame.gameObject.SetActive(true);
        TattooFrame.transform.DOKill();
        TattooFrame.transform.DOLocalMoveY(1500f, .01f).OnComplete(() =>
        {
            TattooFrame.transform.DOLocalMove(TattooFrameTARGET.localPosition, 1f).SetDelay(3).SetEase(Ease.OutBack);

            thisLevel.FadeInSplinesForStart();
        });

        currentlevel = SaveSystem.Instance.DataFields.currentLevel;

        try
        {
            Statics.GA_LevelStartEvent("DrawingLevel" + currentlevel);
        }
        catch { }

        AppmetricaAnalytics.ReportCustomEvent(AnalyticsType.GameData, "DrawingLevel", $"Level_{currentlevel}", "Start");
    }

    int GetLevelAccordingToFirebaseOrder(int lvlNo)
    {
        try
        {
            return FirebaseManager.Instance.levelList.Levels[lvlNo - 1];
        }

        catch
        {
            return lvlNo;
        }
    }

    public void CurrentScratchDone()
    {
        thisLevel.CurrentScratchDone();
    }

    public void SetCurrentColor(int buttonIndex)
    {
        thisLevel.SetCurrentColor(buttonIndex);
    }

    public void ChangePenFunction()
    {
        try
        {
            LevelManager.instance.ChangePenFunction();
        }
        catch
        {
        }
    }

    #region Home, Complete

    int index = 1;

    public void Restart()
    {
        try
        {
            if (currentlevel > 3)
                AdCaller._inst.callads();
        }
        catch
        {
        }

        IEnumerator Execution()
        {
            VibrateIt();

            yield return new WaitForSeconds(0.05f);

            index = SceneManager.GetActiveScene().buildIndex;

            LoadScene();
        }

        StartCoroutine(Execution());
    }

    public void RetryLevel()
    {
        try
        {
            if (currentlevel > 3)
                AdCaller._inst.callads();
        }
        catch
        {
        }

        SaveSystem.Instance.DataFields.currentLevel--; //PlayerPrefs.SetInt("LEVELNUMBER", PlayerPrefs.GetInt("LEVELNUMBER") - 1);

        SaveSystem.Instance.DataFields.levelNumber--;// displayLevelNumber--;

        //PlayerPrefs.SetInt("DISPLAY_LEVEL_NUMBER", displayLevelNumber);

        IEnumerator Execution2()
        {
            yield return new WaitForSeconds(0.05f);

            VibrateIt();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        StartCoroutine(Execution2());
    }

    public void SkipLevelBtn()
    {
        try
        {
            AdCaller._inst.callads();
        }
        catch
        {
        }

        SaveSystem.Instance.DataFields.currentLevel++; //PlayerPrefs.SetInt("LEVELNUMBER", PlayerPrefs.GetInt("LEVELNUMBER") + 1);

        SaveSystem.Instance.DataFields.levelNumber++;//displayLevelNumber++;

        //PlayerPrefs.SetInt("DISPLAY_LEVEL_NUMBER", displayLevelNumber);

        IEnumerator Execution2()
        {
            yield return new WaitForSeconds(0.2f);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        StartCoroutine(Execution2());
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(index);
    }

    Color[] colors = new Color[]
    {
        new Color(0.231f, 0.702f, 1.0f),  // #3bb3ff
        new Color(1.0f, 0.231f, 0.898f),  // #ff3be5
        new Color(0.514f, 0.161f, 0.910f) // #8329e8
    };

    public void Complete()
    {
        currentlevel = SaveSystem.Instance.DataFields.currentLevel;//PlayerPrefs.GetInt("LEVELNUMBER", 0);

        if (currentlevel == 3)
        {
            RatingPanel.SetActive(true);
        }

        gamePaused = true;

        UI_Canvas.SetActive(false);

        Canvas canvas = UI_Canvas.GetComponent<Canvas>();
        canvas.sortingOrder = 0;

        Complete_Panel.SetActive(true);

        Complete_Panel_Image.color = colors[Random.Range(0, colors.Length)];

        SaveSystem.Instance.DataFields.currentLevel++;//PlayerPrefs.SetInt("LEVELNUMBER", PlayerPrefs.GetInt("LEVELNUMBER") + 1);

        SaveSystem.Instance.DataFields.levelNumber++;//displayLevelNumber++;

        //PlayerPrefs.SetInt("DISPLAY_LEVEL_NUMBER", displayLevelNumber);

        int accuracyRand = Random.Range(55, 100);

        Accuracy_Text.text = accuracyRand.ToString() + "%";

        float fillAmount = accuracyRand / 100f;

        Accuracy_Fill_Image.DOFillAmount(fillAmount, 2.5f);

        try
        {
            Statics.GA_LevelCompleteEvent("DrawingLevel" + currentlevel);
        }
        catch { }

        AppmetricaAnalytics.ReportCustomEvent(AnalyticsType.GameData, "DrawingLevel", $"Level_{currentlevel}", "Complete");

        if (PlayerPrefs.GetInt("CanPlayMusic", 1) == 1)
        {
            if (AudioSourceParticle == null)
            {
                AudioSourceParticle = gameObject.AddComponent<AudioSource>();
            }

            AudioSourceParticle.PlayOneShot(SoundClipParticle);
        }

        try
        {
            GadsmeAdsManager.intance.HideGadsMeAd();
        }

        catch { }
    }

    public void OpenGalleryPanel()
    {
        gamePaused = true;

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
            canPaint = false;

        }
        catch { }
        AdCaller._inst.callads();

        int clr1 = PlayerPrefs.GetInt("Color1");
        int clr2 = PlayerPrefs.GetInt("Color2");
        int clr3 = PlayerPrefs.GetInt("Color3");
        int clr4 = PlayerPrefs.GetInt("Color4");
        int clr5 = PlayerPrefs.GetInt("Color5");

        if (clr1 == 1)
        {
            if (ColorImage1 != null && SpriteColorImage1 != null)
                ColorImage1.sprite = SpriteColorImage1;
            if (BtnImg1 != null && SelectSprite != null)
                BtnImg1.sprite = SelectSprite;
            if (BtnTickImg1 != null)
                BtnTickImg1.gameObject.SetActive(true);
        }
        if (clr2 == 2)
        {
            if (ColorImage2 != null && SpriteColorImage2 != null)
                ColorImage2.sprite = SpriteColorImage2;
            if (BtnImg2 != null && SelectSprite != null)
                BtnImg2.sprite = SelectSprite;
            if (BtnTickImg2 != null)
                BtnTickImg2.gameObject.SetActive(true);
        }
        if (clr3 == 3)
        {
            if (ColorImage3 != null && SpriteColorImage3 != null)
                ColorImage3.sprite = SpriteColorImage3;
            if (BtnImg3 != null && SelectSprite != null)
                BtnImg3.sprite = SelectSprite;
            if (BtnTickImg3 != null)
                BtnTickImg3.gameObject.SetActive(true);
        }
        if (clr4 == 4)
        {
            if (ColorImage4 != null && SpriteColorImage4 != null)
                ColorImage4.sprite = SpriteColorImage4;
            if (BtnImg4 != null && SelectSprite != null)
                BtnImg4.sprite = SelectSprite;
            if (BtnTickImg4 != null)
                BtnTickImg4.gameObject.SetActive(true);
        }
        if (clr5 == 5)
        {
            if (ColorImage5 != null && SpriteColorImage5 != null)
                ColorImage5.sprite = SpriteColorImage5;
            if (BtnImg5 != null && SelectSprite != null)
                BtnImg5.sprite = SelectSprite;
            if (BtnTickImg5 != null)
                BtnTickImg5.gameObject.SetActive(true);
        }

        if (Gallery_Panel == null)
        {
            //  Debug.LogError("Gallery_Panel is null.");
        }
        else
        {
            Gallery_Panel.SetActive(true);
        }
        gamePaused = true;

        VibrateIt();

        AudioManagerButtons._inst.PlayMusicTap();

        try
        {

            if (scrollAnimCanvasGroup != null)
            {
                scrollAnimCanvasGroup.alpha = 0;
            }

        }
        catch { }
        try
        {
            if (scrollAnim != null && scrollAnim2 != null)
            {
                scrollAnim.localPosition = new Vector3(scrollAnim.localPosition.x, scrollAnim.localPosition.y - 1000f, scrollAnim.localPosition.z);

                Sequence openSequence = DOTween.Sequence();
                openSequence.Append(scrollAnimCanvasGroup.DOFade(1, 0.9f));
                openSequence.Join(scrollAnim.DOLocalMove(scrollAnim2.localPosition, 0.9f).SetEase(Ease.OutQuad));
            }

        }
        catch { }

        try
        {
            GadsmeAdsManager.intance.HideGadsMeAd();
        }

        catch { }
    }

    public void CloseGalleryPanel()
    {
        gamePaused = false;

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
        AdCaller._inst.callads();
        VibrateIt();
        Gallery_Panel.SetActive(false);
        gamePaused = false;
        AudioManagerButtons._inst.PlayMusicTap();

        try
        {
            GadsmeAdsManager.intance.ShowGadsMeAd();
        }

        catch { }
    }

    public static int selectedGalleryIndex;

    public void StartGalleryLevels(int index)
    {
        selectedGalleryIndex = index;
        SceneManager.LoadScene("Gallery");
    }

    private void CheckAndShowGalleryPanel()
    {
        if (PlayerPrefs.GetInt("ShowGalleryPanel", 0) == 1)
        {
            if (Gallery_Panel == null)
            {
                Debug.LogError("Gallery_Panel is null.");
            }
            else
            {
                Gallery_Panel.SetActive(true);

                scrollAnim.localPosition = new Vector3(scrollAnim.localPosition.x, scrollAnim.localPosition.y - 1000f, scrollAnim.localPosition.z);
                scrollAnimCanvasGroup.alpha = 0;

                Sequence openSequence = DOTween.Sequence();
                openSequence.Append(scrollAnimCanvasGroup.DOFade(1, 0.9f));
                openSequence.Join(scrollAnim.DOLocalMove(scrollAnim2.localPosition, 0.9f).SetEase(Ease.OutQuad));

            }

            PlayerPrefs.SetInt("ShowGalleryPanel", 0);
        }
    }

    PaintDragNew PDN;

    public void OpenSettings()
    {
        gamePaused = true;

        if (ToolMoveOnSpline.instance.audioSource != null)
            ToolMoveOnSpline.instance.audioSource.Stop();

        AudioManagerButtons._inst.PlayMusicTap();

        VibrateIt();

        Settings_Panel.SetActive(true);

        BigBanner.instance.bannerBigBannerShow();

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
            canPaint = false;
        }
        catch { }

        try
        {
            GadsmeAdsManager.intance.HideGadsMeAd();
        }

        catch { }
    }

    public void CloseSettings()
    {
        gamePaused = false;

        AudioManagerButtons._inst.PlayMusicTap();

        VibrateIt();

        Settings_Panel.SetActive(false);

        BigBanner.instance.bannerBigBannerHide();

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
            canPaint = true;
        }
        catch { }

        try
        {
            GadsmeAdsManager.intance.ShowGadsMeAd();
        }

        catch { }
    }

    public void OpenPrivacyPolicyLink()
    {
        Application.OpenURL("https://hmblapps.com/web/policy");
    }

    public void MoveToNextLevel()
    {
        if (currentlevel > 3)
            AdCaller._inst.callads();

        Complete_Panel.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CloseCompleteRewardPanel()
    {
        CompleteReward_Icon.transform.DOKill();
        CompleteReward_Icon.transform.localScale = new Vector3(1, 1, 1);
        CompleteReward_Icon.transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            CompleteReward_Panel.SetActive(false);
        });
    }

    #endregion

    #region SFX, Vibration

    public void VibrateIt()
    {
        if (PlayerPrefs.GetInt("Haptic", 1) == 1)
        {
            MMVibrationManager.Haptic(HapticTypes.SoftImpact);
        }
    }

    #endregion

    #region Particles

    public void ShowTextSpriteParticleFunction(Vector3 pos)
    {
        pos.z = 0;

        if (ShowTextSpriteParticles.Length > 0)
        {
            int randomNumber = Random.Range(0, ShowTextSpriteParticles.Length);

            ShowTextSpriteParticles[randomNumber].transform.parent.transform.localPosition = pos;


            ShowTextSpriteParticles[randomNumber].SetActive(true);


            StartCoroutine(DeactivateSpriteParticleAfterDelay(ShowTextSpriteParticles[randomNumber]));
        }
        else
        {
        }
    }

    IEnumerator DeactivateSpriteParticleAfterDelay(GameObject spriteParticle)
    {
        yield return new WaitForSeconds(2f);

        spriteParticle.SetActive(false);
    }

    #endregion

    #region TextSprites

    void ResetTextSprites()
    {
        for (int i = 0; i < TextSprites.Length; i++)
        {
            TextSprites[i].gameObject.SetActive(false);

            TextSprites[i].transform.localPosition = Vector3.zero;
        }
    }

    public void ShowTextSprite(Vector3 pos)
    {
        //pos = Camera.main.transform.position;
        pos.z = 0;

        ResetTextSprites();

        int randomNumber = Random.Range(0, TextSprites.Length);

        TextSprites[randomNumber].transform.parent.transform.localPosition = pos;

        TextSprites[randomNumber].gameObject.SetActive(true);

        TextSprites[randomNumber].Play("TextPop");

        Invoke(nameof(ResetTextSprites), 3f);

        //PlayClip2ndSource(TextSpritesClip);
    }

    public Vector3 GetMainCameraCenterPoint()
    {
        // Get the main camera
        Camera mainCamera = Camera.main;

        // Calculate the center of the viewport based on normalized coordinates
        Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0f);

        // Use ScreenPointToRay to get a ray from the center of the viewport towards the world
        Ray ray = mainCamera.ScreenPointToRay(viewportCenter);

        // Since the camera position and FOV can change, 
        // we need a plane at the near clipping plane distance
        float nearClipPlaneDistance = mainCamera.nearClipPlane;
        Plane nearPlane = new Plane(mainCamera.transform.forward, mainCamera.transform.position + mainCamera.transform.forward * nearClipPlaneDistance);

        // Intersect the ray with the near plane to get the center point in world space
        Vector3 worldCenterPoint;
        if (nearPlane.Raycast(ray, out float distance))
        {
            worldCenterPoint = ray.GetPoint(distance);
        }
        else
        {
            // Handle potential raycast failure (e.g., no intersection)
            worldCenterPoint = Vector3.zero;
            Debug.LogError("Failed to get main camera center point");
        }

        return worldCenterPoint;
    }

    #endregion

    IEnumerator UpdateCashText()
    {
        while (true)
        {
            for (int i = 0; i < CashTexts.Length; i++)
            {
                CashTexts[i].text = 0 + "";
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public int GetCurrentDay()
    {
        int dayNumber = 1;

        if (currentlevel == 0)
        {
            dayNumber = 1;
        }
        else if (currentlevel > 0 && currentlevel <= 2)
        {
            dayNumber = 2;
        }
        else if (currentlevel > 2 && currentlevel <= 5)
        {
            dayNumber = 3;
        }
        else if (currentlevel > 5 && currentlevel <= 9)
        {
            dayNumber = 4;
        }
        else if (currentlevel > 9 && currentlevel <= 11)
        {
            dayNumber = 5;
        }
        else if (currentlevel > 11 && currentlevel <= 15)
        {
            dayNumber = 6;
        }
        else if (currentlevel > 15)
        {
            dayNumber = 7;
        }
        return dayNumber;
    }

    public int GetLevelForDay()
    {
        int dayNumber = GetCurrentDay();
        int levelNumber = 1;

        if (dayNumber == 1)
        {
            levelNumber = 1;
        }

        else if (dayNumber == 2)
        {
            if (currentlevel == 1)
                levelNumber = 1;

            else if (currentlevel == 2)
                levelNumber = 2;
        }

        else if (dayNumber == 3)
        {
            if (currentlevel == 3)
                levelNumber = 1;

            else if (currentlevel == 4)
                levelNumber = 2;

            else if (currentlevel == 5)
                levelNumber = 3;
        }

        else if (dayNumber == 4)
        {
            if (currentlevel == 6)
                levelNumber = 1;

            else if (currentlevel == 7)
                levelNumber = 2;

            else if (currentlevel == 8)
                levelNumber = 3;

            else if (currentlevel == 9)
                levelNumber = 4;
        }

        else if (dayNumber == 5)
        {
            if (currentlevel == 10)
                levelNumber = 1;

            else if (currentlevel == 11)
                levelNumber = 2;
        }

        else if (dayNumber == 6)
        {
            if (currentlevel == 12)
                levelNumber = 1;

            else if (currentlevel == 13)
                levelNumber = 2;

            else if (currentlevel == 14)
                levelNumber = 3;

            else if (currentlevel == 15)
                levelNumber = 4;
        }

        else if (dayNumber == 7)
        {
            if (currentlevel == 16)
                levelNumber = 1;

            else if (currentlevel >= 17)
                levelNumber = 2;
        }

        return levelNumber;
    }
}