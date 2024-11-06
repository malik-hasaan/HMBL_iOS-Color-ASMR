using System;
using DG.Tweening;
using UnityEngine;
using ScratchCardAsset;
using System.Collections;
using Dreamteck.Splines;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Space()]
    public GameObject PreLevelAssests;

    public Transform PreLevelAssetsTARGET;

    [Space()]
    [Header("------------------------ SCRATCH EFFECTS --------------------------")]
    [Space()]
    public ScratchCardManager _PaintManager;
    public ScratchCard _ScratchCard;
    public EraseProgress _EraseProgress;
    public bool isPaintCompleted = false;

    [Space()]
    public Material scratchSurfaceMat;

    [HideInInspector]
    public Material scratchCardMat;

    [Space()]
    [Header("------------------------ OUTLINEs --------------------------")]
    public GameObject MainOutlineParent;
    public GameObject OutlineIndicatorParent;
    public GameObject OutlineComplete;

    [Space()]
    public SplineData[] OutlineSplines;
    public int currentOutlineSpline = 0;
    public GameObject Outline_ToolTip;

    [Space()]
    [Header("------------------------ COLORs --------------------------")]
    public GameObject MainColorParent;
    public GameObject ColorIndicatorParent;
    public GameObject ColorComplete;

    [Space()]
    public ColorData[] AllColors;
    public int currentColor = 0;
    public GameObject Color_ToolTip;

    [HideInInspector]
    public bool canPaintColor = false, colorEnded = false;

    [Space()]
    [Header("------------------------ TOOLs --------------------------")]
    public GameObject[] Tools;

    [Space()]
    [Header("------------------------ PENS --------------------------")]
    public SpriteRenderer penNib;
    public SpriteRenderer penBg;
    public SpriteRenderer penBody;
    public SpriteRenderer penColorShadow;

    public SpriteRenderer penPlain;
    public SpriteRenderer penShadow;

    private float fadeDuration = 0.5f;

    private SpriteRenderer[] spriteRenderers;

    [Space()]
    [HideInInspector]
    public bool isPaint = false;

    PaintDragNew PDN;

    void Awake()
    {
        instance = this;

        Application.targetFrameRate = 120;
    }

    IEnumerator Start()
    {
        try
        {
            StartFadeAnim();

            ChangePenFunction();
        }
        catch { }

        PreLevelAssests.SetActive(true);

        ResetTools();

        try
        {
            spriteRenderers = OutlineComplete.GetComponentsInChildren<SpriteRenderer>();
        }
        catch { }

        yield return new WaitForSeconds(1.0f);

        scratchCardMat = _ScratchCard.ScratchSurface;

        StartOutSplines();

        yield return new WaitForSeconds(.3f);

        _PaintManager.Card.FillInstantly();
        _PaintManager.Card.Mode = ScratchCard.ScratchMode.Restore;
        isPaintCompleted = false;
    }

    void StartFadeAnim()
    {
        BigBanner.instance.bannerBigBannerShow();

        StartCoroutine(FadeOutAndDeactivateImage());
    }

    public void ChangePenFunction()
    {
        int pen = PlayerPrefs.GetInt("PenImage");

        penBg.sprite = GameManager.Instance._PenBg[pen];
        penBody.sprite = GameManager.Instance._PenBody[pen];
        penColorShadow.sprite = GameManager.Instance._PenShadow[pen];
        penPlain.sprite = GameManager.Instance._PenPlain[pen];
        penShadow.sprite = GameManager.Instance._PenShadow[pen];
    }

    IEnumerator FadeOutAndDeactivateImage()
    {
        MainScript.instance.SceneLoad();

        yield return new WaitForSeconds(2.1f);

        BigBanner.instance.bannerBigBannerHide();

        //AdCaller._inst.callads();

        yield return new WaitForSeconds(1f);

        BlackFadeOut();

        GameManager.Instance.Fade_Img.gameObject.SetActive(false);
    }

    void BlackFadeOut()
    {
        GameManager.Instance.Black_Img.DOFade(0, 1.5f).SetDelay(.2f).OnComplete(() =>
        {
            GameManager.Instance.Black_Img.gameObject.SetActive(false);
        });
    }

    void StartFadeAnim2()
    {
        MainScript.instance.SceneLoad();

        BigBanner.instance.bannerBigBannerShow();

        GameManager.Instance.Fade_Img.DOFade(0, 12.8f).SetDelay(1.8f).OnComplete(() =>
        {
            GameManager.Instance.Fade_Img.gameObject.SetActive(false);

            BigBanner.instance.bannerBigBannerHide();

        });

        MainScript.instance.loadingBar.DOFade(0, 1.5f).SetDelay(1.5f).OnComplete(() =>
        {
            MainScript.instance.loadingBar.gameObject.SetActive(false);
        });
    }

    void ResetTools()
    {
        for (int i = 0; i < Tools.Length; i++)
        {
            Tools[i].SetActive(false);
        }
    }

    #region OutlineSpline

    void ResetOutSplines()
    {
        for (int i = 0; i < OutlineSplines.Length; i++)
        {
            OutlineSplines[i].ToolMoveManager.gameObject.SetActive(false);

            OutlineSplines[i].Indicator.SetActive(false);

            OutlineSplines[i].Spline.SetActive(false);
        }
    }

    public void FadeInSplinesForStart()
    {
        try
        {
            OutlineSplines[0].Indicator.GetComponent<SpriteRenderer>().DOKill();
            OutlineSplines[0].Indicator.GetComponent<SpriteRenderer>().DOFade(0, .001f).SetEase(Ease.Linear).OnComplete(() =>
            {
                OutlineSplines[0].Indicator.GetComponent<SpriteRenderer>().DOFade(0.5f, 1f).SetDelay(3.5f).SetEase(Ease.Linear);
            });

            OutlineSplines[0].Indicator.transform.GetChild(0).GetComponent<SpriteRenderer>().DOKill();
            OutlineSplines[0].Indicator.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0, .001f).SetEase(Ease.Linear).OnComplete(() =>
            {
                OutlineSplines[0].Indicator.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(1, 1f).SetDelay(3.5f).SetEase(Ease.Linear);
            });

            OutlineSplines[0].Indicator.transform.GetChild(1).GetComponent<SpriteRenderer>().DOKill();
            OutlineSplines[0].Indicator.transform.GetChild(1).GetComponent<SpriteRenderer>().DOFade(0, .001f).SetEase(Ease.Linear).OnComplete(() =>
            {
                OutlineSplines[0].Indicator.transform.GetChild(1).GetComponent<SpriteRenderer>().DOFade(1, 1f).SetDelay(3.5f).SetEase(Ease.Linear);
            });

        }
        catch { }

    }

    void StartOutSplines()
    {
        _PaintManager.Card.ToolTip = Outline_ToolTip;
        _PaintManager.Card.BrushScale = OutlineSplines[0].BrushSize;

        MainOutlineParent.SetActive(true);
        OutlineIndicatorParent.SetActive(true);
        OutlineComplete.SetActive(false);

        IEnumerator Execution()
        {
            CameraZooming.instance.ZoomTo(OutlineSplines[currentOutlineSpline].zoomPosition.TargetPos, OutlineSplines[currentOutlineSpline].zoomPosition.FOV);

            ResetOutSplines();

            currentOutlineSpline = 0;

            Outline_ToolTip.transform.localPosition = OutlineSplines[0].toolTipPos;

            try
            {
                OutlineSplines[0].ToolMoveManager.transform.GetComponent<ToolMoveOnSpline>().canActivateTool = false;
            }
            catch
            {
            }

            OutlineSplines[0].ToolMoveManager.gameObject.SetActive(true);

            //OutlineSplines[0].Tool.SetActive(true);
            OutlineSplines[0].Spline.SetActive(true);

            yield return new WaitForSeconds(.05f);

            OutlineSplines[0].SpriteCard.material = scratchCardMat;

            _PaintManager.SpriteCard = OutlineSplines[0].SpriteCard.gameObject;
            _PaintManager.ScratchSurfaceSprite = OutlineSplines[0].Sprite;
            _ScratchCard.Surface = OutlineSplines[0].SpriteCard.transform;

            _PaintManager.Card.SetScratchTexture(_PaintManager.Card.GetScratchTexture());
            _PaintManager.Card.ResetRenderTexture();

            _PaintManager.Card.FillInstantly();
            _PaintManager.Card.Mode = ScratchCard.ScratchMode.Restore;
            isPaintCompleted = false;
            _EraseProgress.currentProgress = 1;

            OutlineSplines[0].Indicator.SetActive(true);

            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(Execution());
    }

    public void indicatorChildsOnOff()
    {
        try
        {
            OutlineSplines[currentOutlineSpline].Indicator.transform.GetChild(1).transform.GetComponent<SpriteRenderer>().DOFade(1, .001f);
            OutlineSplines[currentOutlineSpline].Indicator.transform.GetChild(1).transform.GetComponent<SpriteRenderer>().DOFade(0, .5f).SetEase(Ease.Linear).SetDelay(.1f);
            OutlineSplines[currentOutlineSpline].Indicator.transform.GetChild(1).gameObject.SetActive(false);

            OutlineSplines[currentOutlineSpline].Indicator.transform.GetChild(0).gameObject.SetActive(true);
            OutlineSplines[currentOutlineSpline].Indicator.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().DOFade(0, .001f);
            OutlineSplines[currentOutlineSpline].Indicator.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().DOFade(1, .5f).SetEase(Ease.Linear).SetDelay(.1f);
        }
        catch { }
    }

    public void OutlineSplineDone()
    {
        if (ToolMoveOnSpline.instance != null)
        {
            ToolMoveOnSpline.instance.Tool.enabled = false;

            ToolMoveOnSpline.instance.scratching = false;
        }

        _PaintManager.Card.FillInstantly();
        _PaintManager.Card.Mode = ScratchCard.ScratchMode.Restore;
        isPaintCompleted = false;
        _EraseProgress.currentProgress = 1;

        if (ToolMoveOnSpline.instance.audioSource != null)
            ToolMoveOnSpline.instance.audioSource.Stop();

        ResetOutSplines();

        if (currentOutlineSpline > -1 && currentOutlineSpline < OutlineSplines.Length)
        {
            OutlineSplines[currentOutlineSpline].SpriteCard.material = scratchSurfaceMat;

            OutlineSplines[currentOutlineSpline].SpriteCard.gameObject.SetActive(true);

            OutlineSplines[currentOutlineSpline].Indicator.SetActive(false);
        }

        currentOutlineSpline++;


        if (currentOutlineSpline >= OutlineSplines.Length)
        {
            // OUTLINES DONE
            OutlinesComplete();

            if (ToolMoveOnSpline.instance.audioSource != null)
                ToolMoveOnSpline.instance.audioSource.Stop();
        }
        else
        {
            OutlineSplines[currentOutlineSpline].SpriteCard.gameObject.SetActive(false);

            IEnumerator Execution()
            {

                yield return new WaitForSeconds(.01f);

                if (currentOutlineSpline - 1 >= 0 && currentOutlineSpline - 1 < OutlineSplines.Length)
                {
                    GameManager.Instance.ShowTextSprite(OutlineSplines[currentOutlineSpline - 1].Tool.transform.localPosition);
                    GameManager.Instance.ShowTextSpriteParticleFunction(OutlineSplines[currentOutlineSpline - 1].Tool.transform.position);
                }
                else
                {
                    yield break;
                }

                yield return new WaitForSeconds(.01f);

                if (currentOutlineSpline < OutlineSplines.Length)
                {
                    CameraZooming.instance.ZoomTo(OutlineSplines[currentOutlineSpline].zoomPosition.TargetPos, OutlineSplines[currentOutlineSpline].zoomPosition.FOV);

                    OutlineSplines[currentOutlineSpline].ToolMoveManager.gameObject.SetActive(true);

                    OutlineSplines[currentOutlineSpline].Tool.SetActive(true);

                    OutlineSplines[currentOutlineSpline].Spline.SetActive(true);

                    yield return new WaitForSeconds(.01f);

                    OutlineSplines[currentOutlineSpline].SpriteCard.material = scratchCardMat;

                    _PaintManager.SpriteCard = OutlineSplines[currentOutlineSpline].SpriteCard.gameObject;
                    _PaintManager.ScratchSurfaceSprite = OutlineSplines[currentOutlineSpline].Sprite;

                    _ScratchCard.Surface = OutlineSplines[currentOutlineSpline].SpriteCard.transform;

                    _PaintManager.Card.SetScratchTexture(_PaintManager.Card.GetScratchTexture());
                    _PaintManager.Card.ResetRenderTexture();

                    yield return new WaitForSeconds(.01f);

                    _PaintManager.Card.FillInstantly();
                    _PaintManager.Card.Mode = ScratchCard.ScratchMode.Restore;
                    isPaintCompleted = false;
                    _EraseProgress.currentProgress = 1;

                    OutlineSplines[currentOutlineSpline].SpriteCard.material = scratchCardMat;

                    Outline_ToolTip.transform.localPosition = OutlineSplines[currentOutlineSpline].toolTipPos;
                    _PaintManager.Card.BrushScale = OutlineSplines[currentOutlineSpline].BrushSize;

                    OutlineSplines[currentOutlineSpline].Indicator.SetActive(true);

                    try
                    {
                        OutlineSplines[currentOutlineSpline].Indicator.GetComponent<SpriteRenderer>().DOFade(.5f, .01f).SetEase(Ease.Linear);
                    }
                    catch { }

                }
                else
                {
                    yield break;
                }
            }

            StartCoroutine(Execution());

        }
    }

    void OutlinesComplete()
    {
        //OutlineSplines[0].Tool.SetActive(false);

        GameManager.Instance.ShowTextSprite(OutlineSplines[OutlineSplines.Length - 1].Tool.transform.localPosition);

        GameManager.Instance.VibrateIt();

        GameManager.Instance.ShowTextSpriteParticleFunction(OutlineSplines[OutlineSplines.Length - 1].Tool.transform.position);

        if (ToolMoveOnSpline.instance.audioSource != null)
            ToolMoveOnSpline.instance.audioSource.Stop();

        OutlineIndicatorParent.SetActive(false);

        ActivateIt();

        isPaint = true;

        Invoke(nameof(StartColoring), 1.1f);

        AppmetricaAnalytics.ReportCustomEvent(AnalyticsType.GameData, "DrawingLevels_OutlineAndColor", $"Level_{GameManager.Instance.currentlevel + 1}", "Outline Complete");

        if (ToolMoveOnSpline.instance.audioSource != null)
            ToolMoveOnSpline.instance.audioSource.Stop();

        //CameraZooming.instance.ZoomTo(CameraZooming.instance.defaultPosition, CameraZooming.instance.initialFOV);

        try
        {
            OutlineSplines[0].Tool.GetComponent<SplineFollower>().enabled = false;

            OutlineSplines[0].Tool.transform.DOLocalMove(new Vector3(6f, -7f, 0), .5f).SetEase(Ease.Linear).OnComplete(delegate 
            {
                OutlineSplines[0].Tool.SetActive(false);
            });
        }
        catch
        {
        }
    }

    public void ActivateIt()
    {
        StartCoroutine(FadeInAndActivate());
    }

    IEnumerator FadeInAndActivate()
    {
        OutlineComplete.SetActive(true);

        foreach (var spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = 0;
            spriteRenderer.color = color;
        }

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            foreach (var spriteRenderer in spriteRenderers)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
            yield return null;
        }

        foreach (var spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = 255;
            spriteRenderer.color = color;
        }
    }

    #endregion

    #region Colors

    void ResetColorIndicators()
    {
        for (int i = 0; i < AllColors.Length; i++)
        {
            AllColors[i].Indicator.SetActive(false);
        }
    }

    void StartColoring()
    {
        for (int i = 0; i < AllColors.Length; i++)
        {
            AllColors[i].SpriteCard.gameObject.SetActive(false);
        }

        ShowSelectionColorsParent();

        UpdateColorPlaceholdersColors();

        AllColors[currentColor].Tool.SetActive(false);

        GameManager.Instance.PaintDoneBtn.gameObject.SetActive(false);

        _PaintManager.Card.ToolTip = Color_ToolTip;

        _PaintManager.Card.BrushScale = AllColors[currentColor].BrushSizeColor;

        MainColorParent.SetActive(true);
        ColorIndicatorParent.SetActive(true);
        ColorComplete.SetActive(false);

        ResetColorIndicators();

        currentColor = 0;

        try
        {
            PDN = AllColors[currentColor].Tool.GetComponent<PaintDragNew>();

            PDN.is_dragable = false;

            GameManager.Instance.canPaint = false;

            AllColors[currentColor].Tool.transform.localPosition = new Vector3(6f, -9f, 0);

            AllColors[currentColor].Tool.GetComponent<Collider2D>().enabled = false;

            _ScratchCard.InputEnabled = false;
            _PaintManager.InputEnabled = false;
        }
        catch { }

        AllColors[currentColor].Tool.SetActive(true);

        AllColors[currentColor].Indicator.SetActive(true);

        AllColors[currentColor].SpriteCard.material = scratchCardMat;

        _PaintManager.SpriteCard = AllColors[currentColor].SpriteCard.gameObject;
        _PaintManager.ScratchSurfaceSprite = AllColors[currentColor].Sprite;

        _ScratchCard.Surface = AllColors[currentColor].SpriteCard.transform;

        _PaintManager.Card.SetScratchTexture(_PaintManager.Card.GetScratchTexture());
        _PaintManager.Card.ResetRenderTexture();

        IEnumerator delay()
        {
            yield return new WaitForSeconds(.005f);

            _PaintManager.Card.FillInstantly();
            _PaintManager.Card.Mode = ScratchCard.ScratchMode.Restore;
            isPaintCompleted = false;
            _EraseProgress.currentProgress = 1;

            AllColors[currentColor].SpriteCard.gameObject.SetActive(true);

            CameraZooming.instance.ZoomTo(AllColors[0].zoomPosition.TargetPos, AllColors[0].zoomPosition.FOV);
        }

        StartCoroutine(delay());
    }

    void ActivateAndFadeInIndicator(int colorIndex)
    {
        GameObject indicator = AllColors[colorIndex].Indicator;

        CanvasGroup canvasGroup = indicator.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = indicator.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        canvasGroup.DOFade(1f, 0.7f).OnComplete(() =>
        {
            indicator.SetActive(true);
        });
    }

    IEnumerator ColorDoneExecution()
    {
        AllColors[currentColor].Tool.transform.DOLocalMove(new Vector3(6f, -9f, 0), .6f).OnComplete(() =>
        {
            try
            {
                PDN = AllColors[currentColor].Tool.GetComponent<PaintDragNew>();

                PDN.is_dragable = false;

                GameManager.Instance.canPaint = false;

                AllColors[currentColor].Tool.transform.localPosition = new Vector3(6f, -9f, 0);

                AllColors[currentColor].Tool.GetComponent<Collider2D>().enabled = false;

                _ScratchCard.InputEnabled = false;
                _PaintManager.InputEnabled = false;
            }
            catch { }
        });

        ResetColorIndicators();

        if (currentColor > -1)
        {
            AllColors[currentColor].SpriteCard.material = scratchSurfaceMat;

            AllColors[currentColor].SpriteCard.gameObject.SetActive(true);

            AllColors[currentColor].Indicator.SetActive(false);
        }

        currentColor++;

        if (currentColor >= AllColors.Length)
        {
            ColorCompleted();
        }

        else
        {
            ShowSelectionColorsParent();

            yield return new WaitForSeconds(.01f);

            UpdateColorPlaceholdersColors();

            GameManager.Instance.ShowTextSprite(AllColors[currentColor - 1].SpriteCard.transform.localPosition - new Vector3(0.35f, 0.35f, 0.35f));

            GameManager.Instance.VibrateIt();

            yield return new WaitForSeconds(.1f);

            _PaintManager.Card.BrushScale = AllColors[currentColor].BrushSizeColor;

            AllColors[currentColor].SpriteCard.material = scratchCardMat;

            _PaintManager.SpriteCard = AllColors[currentColor].SpriteCard.gameObject;
            _PaintManager.ScratchSurfaceSprite = AllColors[currentColor].Sprite;

            _ScratchCard.Surface = AllColors[currentColor].SpriteCard.transform;

            yield return new WaitForSeconds(.1f);

            _PaintManager.Card.SetScratchTexture(_PaintManager.Card.GetScratchTexture());
            _PaintManager.Card.ResetRenderTexture();
            AllColors[currentColor].SpriteCard.gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);

            _PaintManager.Card.FillInstantly();
            _PaintManager.Card.Mode = ScratchCard.ScratchMode.Restore;
            isPaintCompleted = false;
            _EraseProgress.currentProgress = 1;

            AllColors[currentColor].SpriteCard.gameObject.SetActive(true);
            AllColors[currentColor].SpriteCard.material = scratchCardMat;

            AllColors[currentColor].Indicator.SetActive(true);

            CameraZooming.instance.ZoomTo(AllColors[currentColor].zoomPosition.TargetPos, AllColors[currentColor].zoomPosition.FOV);

            yield return new WaitForSeconds(.5f);

            AllColors[currentColor].Tool.SetActive(true);
        }

        yield return null;
    }

    void ColorCompleted()
    {
        if (MainColorParent == null)
            Debug.LogError("MainColorParent is null");
        else
            MainColorParent.SetActive(false);

        if (ColorIndicatorParent == null)
            Debug.Log("ColorIndicatorParent is null");
        else
            ColorIndicatorParent.SetActive(false);


        if (ColorComplete == null)
            Debug.Log("ColorComplete is null");
        else
            ColorComplete.SetActive(true);

        colorEnded = true;

        StartCoroutine(LevelComplete());

        AppmetricaAnalytics.ReportCustomEvent(AnalyticsType.GameData, "DrawingLevels_OutlineAndColor", $"Level_{GameManager.Instance.currentlevel + 1}", "Color Complete");

        if (CameraZooming.instance != null)
            CameraZooming.instance.ZoomTo(CameraZooming.instance.defaultPosition, CameraZooming.instance.initialFOV);

        GameManager.Instance.ShowTextSprite(AllColors[AllColors.Length - 1].SpriteCard.transform.localPosition - new Vector3(0.35f, 0.35f, 0.35f));
    }

    void UpdateColorPlaceholdersColors()
    {
        GameManager.Instance.SelectionColorPlaceholders[0].SetColor(AllColors[currentColor].color1);
        GameManager.Instance.SelectionColorPlaceholders[1].SetColor(AllColors[currentColor].color2);
        GameManager.Instance.SelectionColorPlaceholders[2].SetColor(AllColors[currentColor].color3);
    }

    Color selectedColor;

    public void SetCurrentColor(int buttonIndex)
    {
        AudioManagerButtons._inst.PlayMusicTapColor();

        AllColors[currentColor].SpriteCard.color = GameManager.Instance.SelectionColorPlaceholders[buttonIndex].thisColor;

        selectedColor = GameManager.Instance.SelectionColorPlaceholders[buttonIndex].thisColor;
        penBg.color = selectedColor;
        penNib.color = selectedColor;

        HideSelectionColorsParent();

        GameManager.Instance.canPaint = true;

        AllColors[currentColor].Tool.transform.DOLocalMove(AllColors[currentColor].ToolStartPos, .6f).SetEase(Ease.OutQuad).OnComplete(delegate
        {
            SetThingsForColor();
        });

        try
        {
            AllColors[currentColor].Indicator.SetActive(false);
        }
        catch { }

    }

    void SetThingsForColor()
    {
        try
        {
            PDN = AllColors[currentColor].Tool.GetComponent<PaintDragNew>();

            AllColors[currentColor].Tool.GetComponent<Collider2D>().enabled = true;

            PDN.is_dragable = true;

            var mainModule = PDN.particles.main;
            mainModule.startColor = selectedColor;

            GameManager.Instance.canPaint = true;

            _ScratchCard.InputEnabled = true;
            _PaintManager.InputEnabled = true;

            //AllColors[currentColor].Tool.transform.DOKill();
        }
        catch { }
    }

    public void ShowSelectionColorsParent()
    {
        GameManager.Instance.SelectionColorsParent.SetActive(true);

        GameManager.Instance.SelectionColorsParent.transform.DOKill();
        GameManager.Instance.SelectionColorsParent.transform.DOLocalMove(GameManager.Instance.SelectionColorsParentTARGET.localPosition, 1)
            .SetDelay(0.7f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                if (GameManager.Instance.SelectionColorPlaceholders.Length >= 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        GameManager.Instance.SelectionColorPlaceholders[i].transform.DOKill();
                        GameManager.Instance.SelectionColorPlaceholders[i].transform.localScale = Vector3.one;
                    }

                    GameManager.Instance.SelectionColorPlaceholders[0].transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo);
                    GameManager.Instance.SelectionColorPlaceholders[1].transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo).SetDelay(.3f);
                    GameManager.Instance.SelectionColorPlaceholders[2].transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo).SetDelay(.6f);
                    GameManager.Instance.SelectionColorPlaceholders[3].transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo).SetDelay(.9f);
                }
                else
                {
                }
            });
    }

    public void HideSelectionColorsParent()
    {
        GameManager.Instance.SelectionColorsParent.transform.DOKill();
        GameManager.Instance.SelectionColorsParent.transform.DOLocalMoveY(-1500f, 1);
    }

    #endregion

    #region ScratchEffects

    public void UpdateCurrentScratchProgress(float progress)
    {
        UpdateColorProgress(progress);
    }

    public void UpdateColorProgress(float progress)
    {
        if (currentColor >= AllColors.Length)
            return;

        if (progress <= AllColors[currentColor].progressLimit && isPaintCompleted == false)
        {
            if (colorEnded)
                return;

            isPaintCompleted = true;

            GameManager.Instance.PaintDoneBtn.gameObject.SetActive(true);

            GameManager.Instance.PaintDoneBtn.transform.DOKill();
            GameManager.Instance.PaintDoneBtn.transform.localScale = Vector3.one;
            GameManager.Instance.PaintDoneBtn.transform.DOScale(.85f, .5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }

    public void CurrentScratchDone()
    {
        AudioManagerButtons._inst.PlayMusicTap();

        GameManager.Instance.PaintDoneBtn.gameObject.SetActive(false);

        StartCoroutine(ColorDoneExecution());
    }

    #endregion

    IEnumerator LevelComplete()
    {
        yield return new WaitForSeconds(2f);

        ResetTools();

        PreLevelAssests.transform.DORotateQuaternion(PreLevelAssetsTARGET.localRotation, .5f);
        PreLevelAssests.transform.DOScale(PreLevelAssetsTARGET.localScale, .5f);
        PreLevelAssests.transform.DOMove(PreLevelAssetsTARGET.position, .5f);

        GameManager.Instance.Complete();
    }
}

[Serializable]
public class SplineData
{
    public Transform ToolMoveManager;

    public GameObject Spline;

    public GameObject Tool;

    public GameObject Indicator;

    public SpriteRenderer SpriteCard;

    public Sprite Sprite;

    [Space]
    public Vector2 BrushSize;
    public Vector3 toolTipPos;

    [Space]
    public ZoomPositions zoomPosition;
}

[Serializable]
public class ColorData
{
    public GameObject Tool;

    public GameObject Indicator;

    public SpriteRenderer SpriteCard;

    public Sprite Sprite;

    public float progressLimit = .2f;

    [Space()]
    public ZoomPositions zoomPosition;

    [Space]
    public Vector3 ToolStartPos;

    [Space]
    [Space]
    [Header("---------------------- COLORs -----------------")]
    public Color color1;

    [Space]
    public Color color2;

    [Space]
    public Color color3;

    [Space]
    public Vector2 BrushSizeColor = new Vector2(2, 2);
}

[Serializable]
public class ZoomPositions
{
    public Vector3 TargetPos = new Vector3(0, 0, -10f);
    public float FOV = 6f;
}