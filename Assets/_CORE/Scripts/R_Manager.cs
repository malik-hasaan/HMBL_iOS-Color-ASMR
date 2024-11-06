using UnityEngine;
using System.Collections;
using Google.Play.Review;

public class R_Manager : MonoBehaviour
{
    ReviewManager _reviewManager;
    PlayReviewInfo _playReviewInfo;

    void Start()
    {
        StartCoroutine(RequestReview ());
    }

    IEnumerator RequestReview()
    {
        _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();

        yield return requestFlowOperation;

        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);

        yield return launchFlowOperation;

        _playReviewInfo = null; // Reset the object

        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
    }
}