using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ScratchCardAsset;
using System.Collections;

public class LevelManagerGallery : MonoBehaviour
{
    public static LevelManagerGallery instance;

    public LevelTypeGallery level_Type = LevelTypeGallery.TattooMaking;

    [Space()]
    [Space()]
    public GameObject Level_Data;
    public Image Fade_Img;
    public Button PaintDoneBtn;

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
    [Header("------------------------ COLORs --------------------------")]
    public GameObject MainColorParent;
    public GameObject ColorIndicatorParent;
    public GameObject ColorComplete;

    [Space()]
    [Header("First Element Will be a Dummy For Setup Use")]
    public ColorDataGallery[] AllColors;
    public int currentColor = 0;
    public GameObject Color_ToolTip;

    [HideInInspector]
    public bool canPaintColor = false, colorEnded = false;

    [Space()]
    [Header("------------------------ REMOVAL --------------------------")]
    public ColorDataGallery RemovalData;
    public GameObject Removal_ToolTip;

    [Space()]
    [Header("------------------------ TOOLs --------------------------")]
    public GameObject[] Tools;

    [Space()]
    [Header("------------------------ PENS COLOR --------------------------")]
    public SpriteRenderer penBg;
    public SpriteRenderer penNib;

    [Space()]
    public scratchTypeGallery currentScratch = scratchTypeGallery.Color;

    void Awake()
    {
        instance = this;

        SetCurrentColor();
    }


    IEnumerator Start()
    {
        Level_Data.SetActive(true);

        PreLevelAssests.SetActive(true);

        ResetTools();

        StartFadeAnim();

        yield return new WaitForSeconds(0.05f);

        scratchCardMat = _ScratchCard.ScratchSurface;

        if (level_Type == LevelTypeGallery.TattooMaking)
        {
            Invoke(nameof(StartColoring), 0.05f);

            yield return new WaitForSeconds(.05f);

            _PaintManager.Card.FillInstantly();
            _PaintManager.Card.Mode = ScratchCard.ScratchMode.Restore;
            isPaintCompleted = false;
        }

        else
        {
            StartRemoval();

            GameManager.Instance.RemoveTxt.SetActive(true);
        }
    }

    void StartFadeAnim()
    {
        GalleryManager.Instance.LoadingBar();

        BigBanner.instance.bannerBigBannerShow();

        Fade_Img.gameObject.SetActive(true);
        Fade_Img.DOFade(0, 1.8f).SetDelay(1.8f).OnComplete(() =>
        {
            Fade_Img.gameObject.SetActive(false);

            BigBanner.instance.bannerBigBannerHide();

        });
    }

    void ResetTools()
    {
        for (int i = 0; i < Tools.Length; i++)
        {
            Tools[i].SetActive(false);
        }
    }

    void ResetColorIndicators()
    {
        for (int i = 0; i < AllColors.Length; i++)
        {
            AllColors[i].Indicator.SetActive(false);
        }
    }

    void StartColoring()
    {
        CameraZooming.instance.ZoomTo(AllColors[0].zoomPosition.TargetPos, AllColors[0].zoomPosition.FOV);

        for (int i = 0; i < AllColors.Length; i++)
        {
            AllColors[i].SpriteCard.gameObject.SetActive(false);
        }

        AllColors[currentColor].Tool.SetActive(false);

        currentScratch = scratchTypeGallery.Color;

        PaintDoneBtn.gameObject.SetActive(false);

        _PaintManager.Card.ToolTip = Color_ToolTip;

        MainColorParent.SetActive(true);
        ColorIndicatorParent.SetActive(true);
        ColorComplete.SetActive(false);

        ResetColorIndicators();

        currentColor = 0;

        try
        {
            PDN = AllColors[currentColor].Tool.GetComponent<PaintDragNew>();

            PDN.is_dragable = true;

            GameManager.Instance.canPaint = false;

            AllColors[currentColor].Tool.transform.localPosition = AllColors[currentColor].ToolStartPos;
        }
        catch
        {
        }

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
            yield return new WaitForSeconds(.002f);

            _PaintManager.Card.FillInstantly();
            _PaintManager.Card.Mode = ScratchCard.ScratchMode.Restore;
            isPaintCompleted = false;

            AllColors[currentColor].SpriteCard.gameObject.SetActive(true);

            CameraZooming.instance.ZoomTo(AllColors[0].zoomPosition.TargetPos, AllColors[0].zoomPosition.FOV);
        }

        StartCoroutine(delay());
    }

    PaintDragNew PDN;

    IEnumerator ColorDoneExecution()
    {
        AllColors[currentColor].Tool.transform.DOLocalJump(AllColors[currentColor].ToolStartPos, 1, 1, .055f).OnComplete(() =>
        {
            try
            {
                PDN = AllColors[currentColor].Tool.GetComponent<PaintDragNew>();

                PDN.is_dragable = true;

                GameManager.Instance.canPaint = false;

                AllColors[currentColor].Tool.transform.localPosition = AllColors[currentColor].ToolStartPos;
            }
            catch
            {
            }
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
            GameManager.Instance.VibrateIt();

            yield return new WaitForSeconds(.011f);

            AllColors[currentColor].SpriteCard.material = scratchCardMat;

            _PaintManager.SpriteCard = AllColors[currentColor].SpriteCard.gameObject;
            _PaintManager.ScratchSurfaceSprite = AllColors[currentColor].Sprite;

            _ScratchCard.Surface = AllColors[currentColor].SpriteCard.transform;

            yield return new WaitForSeconds(.011f);

            _PaintManager.Card.SetScratchTexture(_PaintManager.Card.GetScratchTexture());
            _PaintManager.Card.ResetRenderTexture();
            AllColors[currentColor].SpriteCard.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.01f);

            _PaintManager.Card.FillInstantly();
            _PaintManager.Card.Mode = ScratchCard.ScratchMode.Restore;
            isPaintCompleted = false;
            _EraseProgress.currentProgress = 1;

            AllColors[currentColor].SpriteCard.gameObject.SetActive(true);
            AllColors[currentColor].SpriteCard.material = scratchCardMat;

            AllColors[currentColor].Indicator.SetActive(true);

            CameraZooming.instance.ZoomTo(AllColors[currentColor].zoomPosition.TargetPos, AllColors[currentColor].zoomPosition.FOV);

            yield return new WaitForSeconds(.05f);

            AllColors[currentColor].Tool.SetActive(true);
        }

        yield return null;
    }

    void ColorCompleted()
    {
        if (MainColorParent == null)
        {
            Debug.LogError("MainColorParent is null");
        }
        else
        {
            MainColorParent.SetActive(false);
        }

        if (ColorIndicatorParent == null)
        {
            Debug.LogError("ColorIndicatorParent is null");
        }
        else
        {
            ColorIndicatorParent.SetActive(false);
        }

        if (ColorComplete == null)
        {
            Debug.LogError("ColorComplete is null");
        }
        else
        {
            ColorComplete.SetActive(true);
        }

        colorEnded = true;

        StartCoroutine(LevelComplete());

        Debug.Log("Colors Done");

        AppmetricaAnalytics.ReportCustomEvent(AnalyticsType.GameData, "GalleryLevels_Color", $"Level_{GalleryManager.Instance.index + 1}", "Color Complete");

        if (CameraZooming.instance != null)
        {
            CameraZooming.instance.ZoomTo(CameraZooming.instance.defaultPosition, CameraZooming.instance.initialFOV);
        }
    }

    public void SetCurrentColor()
    {
        if (AllColors == null || AllColors.Length == 0)
        {
            return;
        }

        if (currentColor < 0 || currentColor >= AllColors.Length)
        {
            return;
        }

        AudioManagerButtons._inst.PlayMusicTapColor();

        AdCaller._inst.callads();

        if (AllColors[currentColor].SpriteCard != null)
        {
            AllColors[currentColor].SpriteCard.color = AllColors[currentColor].color;
        }
        else
        {
        }

        Color selectedColor = AllColors[currentColor].color;

        if (penBg != null)
        {
            penBg.color = selectedColor;
        }
        else
        {
        }

        if (penNib != null)
        {
            penNib.color = selectedColor;
        }
        else
        {
        }

        GameManager.Instance.canPaint = true;

        if (AllColors[currentColor].Tool != null)
        {
            AllColors[currentColor].Tool.transform.DOLocalJump(AllColors[currentColor].ToolStartPos, 1, 1, .01f).OnComplete(delegate
            {
                try
                {
                    PDN = AllColors[currentColor].Tool.GetComponent<PaintDragNew>();

                    if (PDN != null)
                    {
                        PDN.is_dragable = true;

                        GameManager.Instance.canPaint = true;
                    }
                    else
                    {
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"Exception caught: {e.Message}");
                }
            });
        }

        else
        {
        }
    }

    //public void SetCurrentColor()
    //{

    //    AudioManagerButtons._inst.PlayMusicTapColor();

    //    AdCaller._inst.callads();

    //    AllColors[currentColor].SpriteCard.color = AllColors[currentColor].color;

    //    Color selectedColor = AllColors[currentColor].color;

    //    penBg.color = selectedColor;
    //    penNib.color = selectedColor;

    //    GameManager.Instance.canPaint = true;

    //    AllColors[currentColor].Tool.transform.DOLocalJump(AllColors[currentColor].ToolStartPos, 1, 1, .01f).OnComplete(delegate
    //    {
    //        try
    //        {
    //            PDN = AllColors[currentColor].Tool.GetComponent<PaintDragNew>();

    //            PDN.is_dragable = true;
    //            //     PDN.is_allowed_to_return = true;

    //            GameManager.Instance.canPaint = true;
    //        }
    //        catch
    //        {

    //        }
    //    });

    //}

    #region TattooRemoval

    void StartRemoval()
    {
        ResetTools();

        currentScratch = scratchTypeGallery.Removal;

        PaintDoneBtn.gameObject.SetActive(false);

        _PaintManager.Card.ToolTip = Removal_ToolTip;

        RemovalData.Tool.transform.localPosition = RemovalData.ToolStartPos;

        RemovalData.Tool.SetActive(true);

        RemovalData.Indicator.SetActive(true);

        IEnumerator delay()
        {
            yield return new WaitForSeconds(.01f);
            //  yield return new WaitForSeconds(.5f);

            _PaintManager.Card.ClearInstantly();
            _PaintManager.Card.Mode = ScratchCard.ScratchMode.Erase;
            isPaintCompleted = false;

            RemovalData.SpriteCard.gameObject.SetActive(true);

            CameraZooming.instance.ZoomTo(RemovalData.zoomPosition.TargetPos, RemovalData.zoomPosition.FOV);

        }

        StartCoroutine(delay());
    }

    void RemovalCompleted()
    {
        RemovalData.Indicator.SetActive(false);

        _PaintManager.Card.FillInstantly();

        StartCoroutine(LevelComplete());

        CameraZooming.instance.ZoomTo(CameraZooming.instance.defaultPosition, CameraZooming.instance.initialFOV);

        GameManager.Instance.RemoveTxt.SetActive(false);
    }

    #endregion

    #region ScratchEffects

    public void UpdateCurrentScratchProgress(float progress)
    {
        switch (currentScratch)
        {
            case scratchTypeGallery.Color:
                UpdateColorProgress(progress);
                break;
            case scratchTypeGallery.Removal:
                UpdateRemovalProgress(progress);
                break;

            default:
                break;
        }
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

            PaintDoneBtn.gameObject.SetActive(true);
        }
    }

    public void UpdateRemovalProgress(float progress)
    {
        if (progress >= RemovalData.progressLimit && isPaintCompleted == false)
        {
            isPaintCompleted = true;

            PaintDoneBtn.gameObject.SetActive(true);
        }
    }

    public void CurrentScratchDone()
    {
        AudioManagerButtons._inst.PlayMusicTap();

        SetCurrentColor();

        PaintDoneBtn.gameObject.SetActive(false);

        switch (currentScratch)
        {
            case scratchTypeGallery.Color:
                StartCoroutine(ColorDoneExecution());
                break;

            case scratchTypeGallery.Removal:
                RemovalCompleted();
                break;

            default:
                StartCoroutine(ColorDoneExecution());
                break;
        }
    }

    #endregion

    IEnumerator LevelComplete()
    {
        ResetTools();

        yield return new WaitForSeconds(1);

        PreLevelAssests.transform.DORotateQuaternion(PreLevelAssetsTARGET.localRotation, .5f);
        PreLevelAssests.transform.DOScale(PreLevelAssetsTARGET.localScale, .5f);
        PreLevelAssests.transform.DOMove(PreLevelAssetsTARGET.position, .5f);

        GalleryManager.Instance.Complete();
    }

}

public enum scratchTypeGallery
{
    Color,
    Spray,
    Removal,
}

public enum LevelTypeGallery
{
    TattooMaking,
    TattooRemoval
}

[Serializable]
public class ColorDataGallery
{
    public GameObject Tool;

    public GameObject Indicator;

    public SpriteRenderer SpriteCard;

    public Sprite Sprite;

    public float progressLimit = .2f;

    [Space()]
    public ZoomPositionsGallery zoomPosition;

    [Space]
    public Vector3 ToolStartPos;

    [Space]
    [Header(" FOR COLORING ONLY ")]
    public Color color;

}


[Serializable]
public class ZoomPositionsGallery
{
    public Vector3 TargetPos = new Vector3(0, 0, -10f);

    public float FOV = 6f;
}