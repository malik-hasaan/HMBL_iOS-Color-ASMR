using UnityEngine;
using DG.Tweening;
using ScratchCardAsset;
using System.Collections;

public class PaintDragNew : MonoBehaviour
{
    public static PaintDragNew _instance;

    public bool isGallery = false;

    [Space()]
    public bool is_dragable = true;

    [Space()]
    public float returnTime = 0.2f;

    [Space()]
    public Transform Tool;
    public Transform Rotater;
    public Vector3 Tool_Offset = Vector3.zero;

    [Space()]
    public ScratchCard _paint;
    public EraseProgress _eraseProgress;

    [Space()]
    public bool isColoringTool = false;

    [Space()]
    public bool canVibrate = true;

    [Space()]
    public bool canSpread = false;

    [Space()]
    public ParticleSystem particles;

    [Space()]
    public GameObject Nib;

    [Space()]
    public Sprite idleSprite;
    public Sprite curvedSprite;
    public Sprite curvedSpriteLeft;
    public SpriteRenderer nibSpriteRenderer;

    [Space()]
    public Animator animator;

    [HideInInspector]
    public Vector3 old_position, old_scale, screenPoint, offset;

    [HideInInspector]
    public AudioSource audioSource;

    Vector3 startOffset;
    Vector3 startBrushSize;
    Vector3 curScreenPointN, curPositionN;

    Coroutine co;

    Vector3 lastPosition;
    float dragTolerance = 0.1f;

    bool isleft = false;
    bool isRight = false;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        audioSource = GameManager.Instance.Tool2AudioSource;

        if (particles != null)
            particles.gameObject.SetActive(false);

        //if (particles != null)
        //    particles.gameObject.SetActive(true);

        if (audioSource != null)
            audioSource.Stop();

        startOffset = Tool.transform.localPosition;

        if (Nib != null)
            Nib.transform.localPosition = new Vector2(0.03f, 5.722f);

        LeftAnimation();
    }

    public virtual void OnMouseDown()
    {
        try
        {
            if (!isGallery)
            {
                if (!GameManager.Instance.canPaint)
                    return;
            }
        }
        catch { }

        if (is_dragable)
        {
            old_position = gameObject.transform.localPosition;
            old_scale = gameObject.transform.localScale;
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

            //if (particles != null)
            //{
            //    particles.gameObject.SetActive(true);
            //    particles.Play();
            //}

            if (PlayerPrefs.GetInt("CanPlayMusic", 1) == 1 && audioSource != null)
                audioSource.Play();

            co = StartCoroutine(ProgressChecking());

            _paint.InputEnabled = true;

        }
    }

    public virtual void OnMouseDrag()
    {
        try
        {
            if (!isGallery)
            {
                if (!GameManager.Instance.canPaint)
                    return;
            }
        }
        catch { }

        if (is_dragable)
        {
            try
            {
                Vector2 curScreenPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Vector2 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

                float minX = Mathf.Clamp(curPosition.x, -.75f, 4.5f);
                float minY = Mathf.Clamp(curPosition.y, -5f, 4f);

                Vector2 TargetVal = new Vector2(minX, minY);

                transform.DOKill();
                transform.DOMove(TargetVal, .05f).SetEase(Ease.Linear);

                if (Vector2.Distance(curPosition, lastPosition) > dragTolerance)
                {
                    if (curPosition.x > lastPosition.x)
                    {
                        if (!isRight)
                            RightAnimation();
                    }

                    else if (curPosition.x < lastPosition.x)
                    {
                        if (!isleft)
                            LeftAnimation();
                    }

                    lastPosition = curPosition;
                }
            }
            catch { }
        }
    }

    public virtual void OnMouseUp()
    {
        nibSpriteRenderer.sprite = idleSprite;

        try
        {
            nibSpriteRenderer.sprite = idleSprite;
        }
        catch { }

        if (audioSource != null)
            audioSource.Pause();

        _paint.InputEnabled = false;

        try
        {
            if (!isGallery)
            {
                if (GameManager.Instance.gamePaused || !GameManager.Instance.canPaint)
                    return;
            }
        }
        catch { }

        //if (is_dragable)
        //    if (particles != null)
        //    {
        //        particles.gameObject.SetActive(false);
        //        particles.Stop();
        //    }


        if (co != null)
            StopCoroutine(co);
    }

    void RightAnimation()
    {
        isleft = false;
        isRight = true;

        nibSpriteRenderer.sprite = curvedSprite;

        Rotater.DOKill();
        Rotater.DOLocalRotate(new Vector3(0, 0, -5f), .3f).SetEase(Ease.OutQuad);
    }

    void LeftAnimation()
    {
        isleft = true;
        isRight = false;

        nibSpriteRenderer.sprite = curvedSpriteLeft;

        Rotater.DOKill();
        Rotater.DOLocalRotate(new Vector3(0, 0, -20f), .3f).SetEase(Ease.OutQuad);
    }

    IEnumerator ProgressChecking()
    {
        try
        {
            LevelManager.instance.UpdateCurrentScratchProgress(_eraseProgress.GiveProgress());
        }
        catch { }

        try
        {
            LevelManagerGallery.instance.UpdateCurrentScratchProgress(_eraseProgress.GiveProgress());
        }
        catch { }

        yield return new WaitForSeconds(2f);

        co = StartCoroutine(ProgressChecking());
    }
}
