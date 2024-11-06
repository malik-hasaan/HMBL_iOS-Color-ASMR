using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RatingScript : MonoBehaviour
{
    public Image[] stars;
    public Sprite filledStar;
    public Sprite unfilledStar;
    private int rating = 0;

    [Space(20)]
    public GameObject RatingPanel;

    void Start()
    {
        UpdateStars(0);
    }

    void UpdateStars(int rating)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < rating)
            {
                stars[i].sprite = filledStar;

                AnimateStar(stars[i]);
            }
            else
            {
                stars[i].sprite = unfilledStar;
            }
        }
    }

    public void OnStarClick(int starIndex)
    {
        rating = starIndex + 1;

        UpdateStars(rating);

        if (rating == 4 || rating == 5)
        {
            OpenPlayStoreFallback();
        }
        else
        {
            StartCoroutine(offRatingPanel());
        }
    }

    IEnumerator offRatingPanel()
    {
        yield return new WaitForSeconds(1.5f);

        RatingPanel.SetActive(false);

        try
        {
            AdCaller._inst.resetTime();
        }
        catch 
        {
        }
    }

    private void OpenPlayStoreFallback()
    {
        UnityEngine.iOS.Device.RequestStoreReview();

        //Application.OpenURL("https://play.google.com/store/apps/details?id=com.sumraf.asmr.color.drawing");

        StartCoroutine(offRatingPanel());
    }

    void AnimateStar(Image star)
    {
        star.transform.DOKill();
        star.transform.DOBlendableLocalRotateBy(new Vector3(0, 0, 360), 0.8f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutBack);
    }
}