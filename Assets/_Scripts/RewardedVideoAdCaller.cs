using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RewardedVideoAdCaller : MonoBehaviour
{
    public UnityEvent rewardEvent;
    public GameManager _uiScript;
    void Start()
    {
        if (PlayerPrefs.GetInt(gameObject.name) == 1 || PlayerPrefs.GetInt("unlockall") == 1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            GetComponent<Button>().onClick.AddListener(WatchRewardedVideo);
        }
    }

    public void WatchRewardedVideo()
    {
        try
        {
            AdsManager.instance._rewarded = VideoWatches;

            if (PlayerPrefs.GetInt("MaxAdStop") == 0)
            {
                if (AdsManager.instance != null)
                    AdsManager.instance.ShowRewardedAd();
            }
            else
            {
                if (AdmobIntilization._instance != null)
                {
                    AdmobIntilization._instance.ShowRewardAd();
                }
            }
        }
        catch {
        }
    }

    public bool isUnlock, isSkip ;
    public void VideoWatches()
    {
        if(isUnlock)
        {
            Debug.Log("reward granted");
        }
     
        if(isSkip)
        {
            _uiScript.SkipLevelBtn();
        }     
    }

}

