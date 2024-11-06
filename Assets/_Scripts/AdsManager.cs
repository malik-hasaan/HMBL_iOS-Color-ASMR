
//using ByteBrewSDK;
using com.adjust.sdk;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public bool isPausedDuetoAd = false;
    public static AdsManager instance = null;

    [SerializeField] private string maxSdkKey = "";

#if UNITY_IPHONE || UNITY_IOS
    //[SerializeField] string BannerAdUnitId_IOS = "";
    [SerializeField] string InterstitialAdUnitId_IOS = "";
    [SerializeField] string RewardedAdUnitId_IOS = "";
    [Space(50)]
#else
    //[SerializeField] string BannerAdUnitId_Android = "";
    [SerializeField] string InterstitialAdUnitId_Android = "";
    [SerializeField] string RewardedAdUnitId_Android = "";
    [SerializeField] string BannerAdUnitId = "";
    [SerializeField] string MRecAdUnitId = "";

#endif

    [SerializeField]
    MaxSdkBase.BannerPosition bannerPosition;

    public static string InterstitialAdUnitId = "";
    public static string RewardedAdUnitId = "";

    private int interRequestTime = 0;
    private int rewardRequestTime = 0;

    public bool isBannerShowing;
    public bool isMrecLoaded;

    private Action<bool> Callback
    {
        get;
        set;
    }
    public static bool bannerShowing = false;

    public delegate void Rewarded();
    public Rewarded _rewarded;

    public bool IsVideoLoaded
    {
        get
        {
            if (!isMaxInitialized || !MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
                return false;
            else
                return true;
        }
    }

    private bool isMaxInitialized = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);

            return;
        }
        else
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        Invoke(nameof(LoadAds), 0.01f);
    }

    void LoadAds()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) 
            return;

        //AdSettings.SetDataProcessingOptions(new string[] { "LDU" }, 1, 1000);
#if UNITY_IPHONE || UNITY_IOS
        BannerAdUnitId = BannerAdUnitId_IOS;
        InterstitialAdUnitId = InterstitialAdUnitId_IOS;
        RewardedAdUnitId = RewardedAdUnitId_IOS;
        mrecAdUnitId = MRecAdUnitId_IOS;
#else
        //BannerAdUnitId = BannerAdUnitId_Android;
        InterstitialAdUnitId = InterstitialAdUnitId_Android;
        RewardedAdUnitId = RewardedAdUnitId_Android;
#endif
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            // AppLovin SDK is initialized, start loading ads
            //Debug.Log("MAX SDK Initialized");
            isMaxInitialized = true;

            if (PlayerPrefs.GetInt("RemoveAds") == 0)
            {
                InitializeInterstitialAds();

                InitializeBannerAds();

                InitializeMRecAds();
                //Invoke(nameof(ShowBanner), 1);
            }

            InitializeRewardedAds();

        };

        MaxSdk.InitializeSdk();
    }

    public void SelfDestroy()
    {
        instance = null;
        Destroy(this.gameObject);
    }
    public void RequestInter()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) return;

        try
        {
            if (!isMaxInitialized)
                return;

            if (PlayerPrefs.GetInt("RemoveAds") == 0)
            {
                if (!MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
                    LoadInterstitial();
            }
        }
        catch { }


    }
    public void RequestVideo()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) return;

        try
        {

            if (!isMaxInitialized)
                return;
            if (!MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
                LoadRewardedAd();
        }
        catch { }

    }
    public void MuteAudio()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) return;

        try
        {
            MaxSdk.SetMuted(PlayerPrefs.GetInt("Audio", 1) == 0);
        }
        catch { }

    }

    #region Interstitial Ad Methods

    public void InitializeInterstitialAds()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1)
            return;

        try
        {
            if (!isMaxInitialized)
                return;

            // Attach callback
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += AdRevenuePaidEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterAdRevenuePaidEventAppmetrica;
            // Load the first interstitial

            LoadInterstitial();
        }
        catch { }
    }

    private void LoadInterstitial()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) return;

        try
        {
            if (!isMaxInitialized)
                return;
            //Debug.Log("Interstitial loading start=>");
            MuteAudio();
            MaxSdk.LoadInterstitial(InterstitialAdUnitId);
        }
        catch { }


    }
    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(adUnitId) will now return 'true'
        //Debug.Log("Max Interstitial ad loaded");
        interRequestTime = 0;
    }
    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
        //print("Interstitial FailedToReceiveAd=>" + errorInfo);
        if (interRequestTime >= 3)
            return;
        interRequestTime += 1;
        Invoke(nameof(LoadInterstitial), 5f);
        Time.timeScale = 1;
    }
    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        if (interRequestTime >= 3)
            return;
        interRequestTime += 1;
        Invoke(nameof(LoadInterstitial), 5f);
        Time.timeScale = 1;
    }
    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        //Debug.Log("Interstitial ad is dismissed=>");
        Invoke("LoadInterstitial", 0.5f);
        Time.timeScale = 1;
        AdCaller._inst.resetTime();
    }

    int currentCount = 0;
    public void ShowInterstitial()
    {
        currentCount = PlayerPrefs.GetInt("AdmobCount", 0);

        if (PlayerPrefs.GetInt("RemoveAds", 0) == 1)
            return;

        try
        {

            if (isMaxInitialized && MaxSdk.IsInterstitialReady(InterstitialAdUnitId) && PlayerPrefs.GetInt("MaxAdStop") == 0)
            {
                AdmobIntilization._instance.isPausedDuetoAd = true;
                ShowMaxInterstitial();
            }
            else
            {
                LoadInterstitial();
                if (AdmobIntilization._instance.HasAdmobInterstialAvaible())
                {
                    AdmobIntilization._instance.isPausedDuetoAd = true;
                    AdmobIntilization._instance.ShowInterstialAd();
                }
                else
                {
                    AdmobIntilization._instance.RequestInterstitial();
                }
            }

        }
        catch { }
    }

    public void ShowMaxInterstitial()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) return;

        try
        {
            MaxSdk.ShowInterstitial(InterstitialAdUnitId);
        }
        catch { }

    }

    #endregion

    #region Rewarded Ad Methods
    private void InitializeRewardedAds()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) return;

        try
        {
            if (!isMaxInitialized)
                return;
            // Attach callback
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += AdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardAdRevenuePaidEventAppmetrica;
            // Load the first rewarded ad
            LoadRewardedAd();
        }
        catch { }


    }
    private void LoadRewardedAd()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) return;

        try
        {
            //Debug.Log("Rewarded loading start=>");
            MuteAudio();
            MaxSdk.LoadRewardedAd(RewardedAdUnitId);
        }
        catch { }


    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        //print("RewardedAd Loaded event");
        rewardRequestTime = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)
        //print("Rewarded Failed ToReceive with error code" + errorInfo);
        Callback?.Invoke(false);
        Callback = null;
        if (rewardRequestTime >= 3)
            return;
        rewardRequestTime += 1;
        Invoke(nameof(LoadRewardedAd), 5f);
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        //print("RewardedAd FailedToShow with error code" + errorInfo);
        Callback?.Invoke(false);
        Callback = null;
        //LoadRewardedAd();
    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        //print("RewardedAd hidden event received");
        Callback?.Invoke(false);
        Callback = null;
        LoadRewardedAd();
        //AdCaller._instance.resetTime();
    }
    private void OnRewardedAdDismissedEvent(string adUnitId)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        //print("RewardedAd Dismissed event received");

        LoadRewardedAd();
        //AdCaller._instance.resetTime();


    }
    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        //print("Video event for EarnedReward=>");

        rewardRequestTime = 0;
        LoadRewardedAd();
        AdCaller._inst.resetTime();
        _rewarded();
    }
    public void ShowRewardedAd()
    {
        try
        {
            if (!isMaxInitialized)
                return;

            if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId) && PlayerPrefs.GetInt("MaxAdStop") == 0)
            {
                AdmobIntilization._instance.isPausedDuetoAd = true;
                MaxSdk.ShowRewardedAd(RewardedAdUnitId);
            }
            else
            {
                LoadRewardedAd();
                AdmobIntilization._instance.ShowRewardAd();
                AdmobIntilization._instance.isPausedDuetoAd = true;
            }
        }
        catch { }
    }

    #endregion

    #region Banner Ad Methods

    // Retrieve the ID from your account

    public void InitializeBannerAds()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) return;

        try
        {
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(BannerAdUnitId, bannerPosition);
            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, Color.clear);
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += AdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEventAppmetrica;
        }
        catch { }


    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        isBannerShowing = true;
        AdmobIntilization._instance.bannerHide();
        ShowBanner();

        Debug.Log("Banner Load Event");

    }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        isBannerShowing = false;
        AdmobIntilization._instance.bannerShow();
        AdsManager.instance.HideBanner();

        Debug.Log("Banner FailedEvent Event");

    }

    public void ShowBanner()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) return;
        if (PlayerPrefs.GetInt("RemoveAds") == 1) return;

        try
        {
            MaxSdk.ShowBanner(BannerAdUnitId);
        }
        catch { }
        Debug.Log(" Show Banner  Event");
    }

    public void HideBanner()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) return;

        try
        {
            MaxSdk.HideBanner(BannerAdUnitId);
        }
        catch { }
        Debug.Log(" Hide Banner  Event");
    }

    #endregion

    #region MREC Ad Methods

    private void InitializeMRecAds()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1) return;

        try
        {
            MaxSdk.CreateMRec(MRecAdUnitId, MaxSdkBase.AdViewPosition.Centered);
            // Attach Callbacks
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += AdRevenuePaidEvent;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMrecAdRevenuePaidEventAppmetrica;
            MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
            MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdFailedEvent;

            // MRECs are automatically sized to 300x250.
        }
        catch { }
    }
    private void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // MRec ad is ready to be shown.
        // If you have already called MaxSdk.ShowMRec(MRecAdUnitId) it will automatically be shown on the next MRec refresh.
        isMrecLoaded = true;
    }

    private void OnMRecAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // MRec ad failed to load. MAX will automatically try loading a new ad internally.
        isMrecLoaded = false;
    }

    public void ShowMRECBanner()
    {
        if (PlayerPrefs.GetInt("MaxAdStop") == 1)
            return;

        try
        {
            MaxSdk.ShowMRec(MRecAdUnitId);
        }
        catch
        {

        }
    }

    public void ShowMRECBannerPosition(MaxSdkBase.AdViewPosition AdPos)
    {
        MaxSdk.CreateMRec(MRecAdUnitId, AdPos);
        MaxSdk.ShowMRec(MRecAdUnitId);
    }

    public void HideMRECBanner()
    {
        try
        {
            MaxSdk.HideMRec(MRecAdUnitId);
        }
        catch 
        {

        }
    }



    #endregion

    private void AdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        try
        {
            double revenue = impressionData.Revenue;
            var impressionParameters = new[] {
        new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
        new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
        new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
        new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
        new Firebase.Analytics.Parameter("value", revenue),
        new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);


            //Rev Event For Adjust
            AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
            adjustAdRevenue.setRevenue(revenue, "USD");
            adjustAdRevenue.setAdRevenueNetwork(impressionData.NetworkName);
            adjustAdRevenue.setAdRevenueUnit($"{impressionData.AdFormat}_{impressionData.AdUnitIdentifier}");
            Adjust.trackAdRevenue(adjustAdRevenue);
        }
        catch { }
    }

    //Appmetric Revenue Events
    private void OnBannerAdRevenuePaidEventAppmetrica(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        //AppmetricaAnalytics.ReportRevenue_Applovin(impressionData, AppmetricaAnalytics.AdFormat.Banner, "");
    }
    private void OnInterAdRevenuePaidEventAppmetrica(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        //AppmetricaAnalytics.ReportRevenue_Applovin(impressionData, AppmetricaAnalytics.AdFormat.Interstitial, "");
    }
    private void OnRewardAdRevenuePaidEventAppmetrica(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        //AppmetricaAnalytics.ReportRevenue_Applovin(impressionData, AppmetricaAnalytics.AdFormat.Rewarded, "");
    }
    private void OnMrecAdRevenuePaidEventAppmetrica(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        //AppmetricaAnalytics.ReportRevenue_Applovin(impressionData, AppmetricaAnalytics.AdFormat.MREC, "");
    }
}