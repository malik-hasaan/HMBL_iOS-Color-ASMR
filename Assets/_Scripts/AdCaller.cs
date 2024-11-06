using UnityEngine;

public class AdCaller : MonoBehaviour
{
    public static AdCaller _inst;
    public static int _countAds = 0;

    [Space()]
    public float time;

    [Space()]
    public bool TimerOn;
    public float timeLeft;

    bool isShowAd = true;

    void Awake()
    {
        if (_inst != null)
            return;

        else
            _inst = this;
    }

    public void callads()
    {
        if (isShowAd)
        {
            isShowAd = false;

            timeLeft = time;

            TimerOn = true;

            CallInterstitial();
        }
    }
    
    void Update()
    {
        if (TimerOn)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;

                isShowAd = false;
            }

            else
            {
                timeLeft = 0;

                TimerOn = false;

                isShowAd = true;
            }
        }
    }

    public void resetTime()
    {
        isShowAd = false;

        timeLeft = time;

        TimerOn = true;
    }

    public void CallInterstitial()
    {
        isShowAd = false;

        timeLeft = time;

        TimerOn = true;

        if (PlayerPrefs.GetInt("NoAds") == 1) 
            return;

        if (PlayerPrefs.GetInt("RemoveAds") == 0)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                AdsManager.instance.ShowInterstitial();
            }
        }
    }
}
