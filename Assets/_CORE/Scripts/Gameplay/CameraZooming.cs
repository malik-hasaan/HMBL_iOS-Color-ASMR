using UnityEngine;
using System.Collections;

public class CameraZooming : MonoBehaviour
{
    public static CameraZooming instance;

    public Camera targetCamera; 
    public float zoomDuration = 1f; 

    [Space()]
    public float initialFOV = 60f;
    public float startFOV = 60f; 
    public float targetFOV = 40f; 

    [Space()]
    [HideInInspector]
    public Vector3 defaultPosition, startPosition, targetPosition;

    [Space()]
    public float currentFOV; 
    private float zoomStartTime; 
    private bool isZooming = false; 

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        initialFOV = currentFOV = targetCamera.orthographicSize; 

        defaultPosition = targetCamera.transform.position;
    }

    void DoZoom()
    {
        if (!isZooming) 
        {
            zoomStartTime = Time.time;

            isZooming = true;

            StartCoroutine(ZoomCoroutine()); 
        }
    }

    IEnumerator ZoomCoroutine()
    {
        float timeElapsed = 0f;

        while (timeElapsed < zoomDuration)
        {
            timeElapsed = Time.time - zoomStartTime;
            float progress = Mathf.Clamp01(timeElapsed / zoomDuration);

            currentFOV = Mathf.Lerp(startFOV, targetFOV, progress);
            targetCamera.orthographicSize = currentFOV;

            targetCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

            yield return null; 
        }

        isZooming = false;
    }

    public void ZoomTo(Vector3 targetPos, float FOV_target)
    {
        startPosition = targetCamera.transform.position;

        startFOV = targetCamera.orthographicSize;

        targetPosition = targetPos;

        targetFOV = FOV_target;

        DoZoom();
    }


    public void ZoomTo(Vector3 startPos, Vector3 targetPos, float FOV_target)
    {
        startPosition = startPos;

        startFOV = targetCamera.orthographicSize;

        targetPosition = targetPos;

        targetFOV = FOV_target;

        DoZoom();
    }
}
