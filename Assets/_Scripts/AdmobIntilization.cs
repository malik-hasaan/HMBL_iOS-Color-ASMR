using UnityEngine;
using com.adjust.sdk;
using GoogleMobileAds.Api;

public class AdmobIntilization : MonoBehaviour
{
    static BannerView bannerView;
    static InterstitialAd interstitial;
    static RewardedAd rewardedAd;
    public bool isTestIdOn;
    string test_bannerID = "ca-app-pub-3940256099942544/6300978111";
    string test_interstitialID = "ca-app-pub-3940256099942544/1033173712";
    string test_rewardID = "ca-app-pub-3940256099942544/5224354917";

    public string bannerID = "ca-app-pub-3940256099942544/6300978111";
    public string interstitialID = "ca-app-pub-3940256099942544/1033173712";
    public string rewardID = "ca-app-pub-3940256099942544/5224354917";

    public AdSize adSize;
    public AdPosition adPosition;

    public static AdmobIntilization _instance;
    public GameObject callBackObject;

    public bool isPausedDuetoAd;

    //Action _callback;
    public AppOpen_Code _appOpen;
    public BigBanner _bigBan;

    void Awake()
    {
        if (_instance == null)
            _instance = this;

        _appOpen = GetComponent<AppOpen_Code>();
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1)
            return;

        MobileAds.SetiOSAppPauseOnBackground(true);

        MobileAds.Initialize(initStatus =>
        {
            if (PlayerPrefs.GetInt("RemoveAds") == 0)
            {
                RequestBanner();

                RequestInterstitial();

                _appOpen.CallingAppOpen();

                _bigBan.toSenRquest();
            }

            RequesRewardAd();
        });
    }

    public void RequestBanner()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1) return;
        try
        {
            if (bannerView != null)
            {
                bannerView.Destroy();
            }
            bannerView = new BannerView(isTestIdOn == false ? bannerID : test_bannerID, AdSize.Banner, adPosition);
            ListenToAdEvents();
            var adRequest = new AdRequest();
            // send the request to load the ad.
            bannerView.LoadAd(adRequest);
            //GameAnalyticsILRD.SubscribeAdMobImpressions(bannerID, bannerView);
        }
        catch { }
    }

    public void BannerTop()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1) return;
        try
        {
            if (bannerView != null)
            {
                bannerView.SetPosition(AdPosition.Top);
            }
        }
        catch { }
    }

    public void BannerBottom()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1) return;

        try
        {
            if (bannerView != null)
            {
                bannerView.SetPosition(AdPosition.Bottom);
            }
        }
        catch { }
    }

    private void ListenToAdEvents()
    {
        // Raised when an ad fails to load into the banner view.
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            //Debug.Log("Banner view failed to load an ad with error : " + error);
            bannerHide();
        };
        // Raised when the ad is estimated to have earned money.
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            //Debug.Log(String.Format("Banner view paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            
            paidEvent(adValue);
            
            //AppmetricaAnalytics.ReportRevenue_Admob(adValue, AppmetricaAnalytics.AdFormat.Banner, "");
        };
    }

    public void bannerShow()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1) return;
        if (PlayerPrefs.GetInt("AdsStop") == 1) return;
        try
        {
            if (AdsManager.instance.isBannerShowing)
            {

                if (bannerView != null)
                {
                    bannerView.Hide();
                }
                return;
            }

            if (bannerView != null)
            {
                bannerView.Show();
            }
            else
            {
                if (PlayerPrefs.GetInt("RemoveAds") == 0)
                    RequestBanner();
            }
        }
        catch { }
    }

    public void bannerHide()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1) return;

        try
        {
            if (bannerView != null)
                bannerView.Hide();
        }
        catch { }
    }

    // Event handler for the AdFailedToLoad event

    AdRequest interstitialrequest;
    public void RequestInterstitial()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1) return;

        try
        {
            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }
            var adRequest = new AdRequest();

            // send the request to load the ad.
            InterstitialAd.Load(isTestIdOn == false ? interstitialID : test_interstitialID, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        //Debug.Log("interstitial ad failed to load an ad " + "with error : " + error);
                        return;
                    }

                    //Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());

                    interstitial = ad;
                    InterstitialRegisterEventHandlers(interstitial);
                });
        }
        catch { }
    }

    private void InterstitialRegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            //Debug.Log(String.Format("Interstitial ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            paidEvent(adValue);
            //AppmetricaAnalytics.ReportRevenue_Admob(adValue, AppmetricaAnalytics.AdFormat.Interstitial, "");

        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            RequestInterstitial();
            AdCaller._inst.resetTime();

        };
    }

    public void ShowInterstialAd()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1) return;
        if (PlayerPrefs.GetInt("AdsStop") == 1) return;

        try
        {
            if (interstitial != null)
            {
                interstitial.Show();
                isPausedDuetoAd = true;
            }
        }
        catch { }


    }

    public bool HasAdmobInterstialAvaible()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1) return false;
        try
        {
            if (!interstitial.CanShowAd())
            {
                RequestInterstitial();
            }
        }
        catch { }
        return interstitial.CanShowAd();
    }

    static AdRequest requestReward;
    public void RequesRewardAd()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1) return;
        try
        {
            // Clean up the old ad before loading a new one.
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }

            //Debug.Log("Loading the rewarded ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            RewardedAd.Load(isTestIdOn == false ? rewardID : test_rewardID, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        //Debug.LogError("Rewarded ad failed to load an ad " + "with error : " + error);
                        return;
                    }
                    //Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
                    rewardedAd = ad;
                    RewardedRegisterEventHandlers(rewardedAd);
                });
        }
        catch { }
    }

    private void RewardedRegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            //Debug.Log(String.Format("Rewarded ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            paidEvent(adValue);
            //AppmetricaAnalytics.ReportRevenue_Admob(adValue, AppmetricaAnalytics.AdFormat.Rewarded, "");
        };

        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            DelayRequest();

            AdCaller._inst.resetTime();
        };
    }

    public void ShowRewardAd()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1) return;
        try
        {
            if (rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    isreward = true;
                    Invoke("giveReward", 0.2f);
                    Invoke("DelayRequest", 0.1f);
                    //Debug.Log("This is admob rewards");
                    AdCaller._inst.resetTime();

                });
                isPausedDuetoAd = true;
            }
            else
            {
                RequesRewardAd();
            }
        }
        catch { }
    }
    public void giveReward()
    {
        AdsManager.instance._rewarded();
    }
    public bool HasRewardedAvaiable()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1) return false;

        try
        {
            if (!rewardedAd.CanShowAd())
            {
                RequesRewardAd();
            }
        }
        catch { }
        return rewardedAd.CanShowAd();
    }

    #region RewardedAd callback handlers
    bool isreward;
    void DelayRequest()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1) return;

        try
        {
            RequesRewardAd();

        }
        catch { }

    }
    #endregion

    //for destroy
    public void DestroyBanner()
    {
        if (PlayerPrefs.GetInt("AdsStop") == 1) return;
        try
        {

            if (bannerView != null)
                bannerView.Destroy();
        }
        catch { }
    }

    public void paidEvent(AdValue adValue)
    {
        //ADJUST
        double revenue = adValue.Value / 1000000f;
        AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
        adjustAdRevenue.setRevenue(revenue, "USD");
        Adjust.trackAdRevenue(adjustAdRevenue);

        //FIREBASE
        var impressionParameters = new[] {
         new Firebase.Analytics.Parameter("ad_platform", "AdMob"),
         new Firebase.Analytics.Parameter("value", revenue),
         new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
         };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }
}
