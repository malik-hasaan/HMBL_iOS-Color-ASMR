using UnityEngine;
using DG.Tweening;
using ScratchCardAsset;
using Dreamteck.Splines;
using System.Collections;

public class ToolMoveOnSpline : MonoBehaviour
{
    public static ToolMoveOnSpline instance;

    [Space()]
    public GameObject Object;

    [Space()]
    public ParticleSystem particle;

    [Space()]
    public SplineComputer SC;
    public SplineFollower Tool;

    [Space()]
    public ScratchCard _paint;
    public ScratchCardManager _paintManager;

    [Space()]
    public AudioClip newTapAudio;

    [HideInInspector]
    public bool scratching = false, isVibrating = false;

    [HideInInspector]
    public AudioSource audioSource;

    [Space()]
    public Vector2 firstPoint;

    [Space()]
    [Space()]
    public bool canActivateTool = true;

    [Space()]
    public bool isNotReached = true;
    bool isFirstTime = true;

    Camera cam;

    Vector3 calculatedPos;
    bool isIndicatorAnimPlayed = false;

    Coroutine toolMoveCoroutine;

    void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        Application.targetFrameRate = 120;

        cam = GameManager.Instance.MainCam;
        audioSource = GameManager.Instance.Tool1AudioSource;

        if (audioSource != null)
            audioSource.Stop();

        isVibrating = false;

        if (particle != null)
            particle.Stop();

        Tool.enabled = false;

        Tool.spline = SC;
        Tool.follow = false;

        Tool.followSpeed = 2.1f;

        _paint.InputEnabled = false;
        _paintManager.InputEnabled = false;

        isNotReached = true;

        firstPoint = SC.GetPointPositionSpecial(0);

        if (canActivateTool)
        {
            Tool.transform.DOKill();
            Tool.transform.DOMove(new Vector2(firstPoint.x, firstPoint.y), 0.3f)
                .SetEase(Ease.Linear)
                .OnComplete(delegate
                {
                    StartCoroutine(Execution());
                });
        }

        else
        {
            isFirstTime = false;

            Tool.transform.DOKill();
            Tool.gameObject.SetActive(true);
            Tool.transform.localPosition = new Vector3(5, -2f, 0);
            Tool.transform.DOMove(new Vector2(firstPoint.x, firstPoint.y), 0.3f)
                .SetDelay(3f)
                .SetEase(Ease.Linear)
                .OnComplete(delegate
                {
                    StartCoroutine(Execution());
                });
        }
    }

    IEnumerator Execution()
    {
        isNotReached = false;

        yield return new WaitForSeconds(.01f);

        Tool.gameObject.SetActive(true);

        yield return new WaitForSeconds(.01f);

        Tool.ResetSpecial(SC);
        Tool.SetPercentNew(0);

        Tool.enabled = true;

        Object.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);

    }

    void Update()
    {
        if (GameManager.Instance.gamePaused)
            return;

        if (isNotReached)
            return;

        try
        {
            if (scratching)
            {
                calculatedPos = cam.WorldToScreenPoint(_paintManager.Card.ToolTip.transform.position);
                calculatedPos.z = 0;

                _paint.cardInput.ScratchAtPoint(calculatedPos);
            }
        }
        catch { }
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.gamePaused)
            return;

        if (isFirstTime)
            isFirst();

        if (isNotReached)
            return;

        if (audioSource != null && newTapAudio != null)
            audioSource.PlayOneShot(newTapAudio);


        if (toolMoveCoroutine != null)
            StopCoroutine(toolMoveCoroutine);

        toolMoveCoroutine = StartCoroutine(MoveToSplineAndStartScratching());

        HandleRotation(90f);

        if (!isIndicatorAnimPlayed)
        {
            isIndicatorAnimPlayed = true;
            LevelManager.instance.indicatorChildsOnOff();
        }
    }

    void OnMouseUp()
    {
        if (isNotReached)
            return;

        if (toolMoveCoroutine != null)
            StopCoroutine(toolMoveCoroutine);

        if (audioSource != null)
            audioSource.Stop();

        if (particle != null)
        {
            particle.gameObject.SetActive(false);
            particle.Stop();
        }

        if (GameManager.Instance.gamePaused)
            return;

        scratching = false;
        Tool.follow = false;
        isVibrating = false;
        _paint.InputEnabled = false;
        _paintManager.InputEnabled = false;

        toolMoveCoroutine = StartCoroutine(MoveAwayFromSpline());

        HandleRotation(79f);

    }

    void HandleRotation(float val)
    {
        Tool.transform.DORotate(new Vector3(0, 0, val), .35f).SetEase(Ease.Linear);
    }

    IEnumerator MoveToSplineAndStartScratching()
    {
        Vector3 splinePosition = SC.EvaluatePosition(Tool.GetPercent());
        float distanceToSpline = Vector3.Distance(Tool.transform.position, splinePosition);

        Tool.transform.DOKill();
        while (distanceToSpline > 0.01f)
        {
            Tool.transform.position = Vector3.Lerp(Tool.transform.position, splinePosition, Time.deltaTime * 10f);

            distanceToSpline = Vector3.Distance(Tool.transform.position, splinePosition);

            yield return null;
        }

        if (audioSource != null)
            audioSource.Play();

        if (particle != null)
        {
            particle.gameObject.SetActive(true);

            particle.Play();
        }

        yield return new WaitForSeconds(0.05f);

        Tool.follow = true;
        scratching = true;
        isVibrating = true;
        _paint.InputEnabled = true;
        _paintManager.InputEnabled = true;
    }

    public void isFirst()
    {
        Tool.ResetSpecial(SC);
        Tool.SetPercentNew(0);

        Tool.enabled = true;
        Object.gameObject.SetActive(true);
        isNotReached = false;

        isFirstTime = false;

        if (audioSource != null)
            audioSource.Play();

        if (particle != null)
        {
            particle.gameObject.SetActive(true);

            particle.Play();
        }

        Tool.follow = true;
        scratching = true;
        isVibrating = true;
        _paint.InputEnabled = true;
        _paintManager.InputEnabled = true;
    }

    float distanceToTarget;
    Vector3 targetPosition;
    IEnumerator MoveAwayFromSpline()
    {
        try
        {
            double percent = Tool.GetPercent();
            Vector3 splinePosition = SC.EvaluatePosition(percent);
            float offset = 0.01f;
            Vector3 nextPosition = SC.EvaluatePosition(percent + offset);
            Vector3 tangent = (nextPosition - splinePosition).normalized;
            Vector3 perpendicular = Vector3.Cross(tangent, Vector3.forward).normalized;

            float offsetDistance = 0.3f;
            targetPosition = splinePosition + (perpendicular * offsetDistance);
            distanceToTarget = Vector3.Distance(Tool.transform.position, targetPosition);

        }
        catch { }
        while (distanceToTarget > 0.01f)
        {
            Tool.transform.position = Vector3.Lerp(Tool.transform.position, targetPosition, Time.deltaTime * 10f);
            distanceToTarget = Vector3.Distance(Tool.transform.position, targetPosition);
            yield return null;
        }
    }

}
