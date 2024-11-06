using UnityEngine;
using UnityEngine.UI;

public class RewardedVideoAdCaller3 : MonoBehaviour
{
    public static bool isRewardAd = false;
    public GameObject RewardedPanel;

    // Use this for initialization
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

    public void VideoWatches()
    {
        PlayerPrefs.SetInt(gameObject.name, 1);
        gameObject.SetActive(false);
    }

}
