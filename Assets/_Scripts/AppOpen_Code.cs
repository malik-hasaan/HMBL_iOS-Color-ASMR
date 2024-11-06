using System;
using UnityEngine;
using com.adjust.sdk;
using System.Collections;
using GoogleMobileAds.Api;

public class AppOpen_Code : MonoBehaviour
{
    bool APPopendelay = false;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(6f);

        if (PlayerPrefs.GetInt("FirstTimeAppOpen", 0) == 0) 
        {
            PlayerPrefs.SetInt("FirstTimeAppOpen", 1);
        }

        else
        {
            ShowAppOpenAdIfAvailable();
        }
    }

    public void CallingAppOpen()
    {
        LoadOpenApp();
    }

    public string AppOpenId;

    [Header("Screen Orientation")]
    public ScreenOrientation screenOrientation;

    readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
    DateTime _expireTime;

    AppOpenAd _ad;
    bool isShowingAd = false;

    public void LoadOpenApp()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1) return;
        if (PlayerPrefs.GetInt("NoAds") == 1) return;

        // Clean up the old ad before loading a new one.
        if (_ad != null)
        {
            _ad.Destroy();
            _ad = null;
        }

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        AppOpenAd.Load(AppOpenId, screenOrientation, adRequest,
            (AppOpenAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null)
                {

                    return;
                }



                _ad = ad;
                _expireTime = DateTime.Now + TIMEOUT;
                RegisterEventHandlers(ad);

            });
    }

    public void ShowAppOpenAdIfAvailable()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1) 
            return;

        if (!AdmobIntilization._instance.isPausedDuetoAd)
        {
            if (_ad == null || isShowingAd || !IsAppOpenAdTimedOut)
            {
                return;
            }

            _ad.Show();
        }

        else
        {
            AdmobIntilization._instance.isPausedDuetoAd = false;
        }
    }

    void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {


            isShowingAd = true;
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {

            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            _ad = null;
            isShowingAd = false;
            LoadOpenApp();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {

            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            _ad = null;
            LoadOpenApp();
        };
        ad.OnAdPaid += (AdValue adValue) =>
        {
            paidEvent(adValue);

            //AppmetricaAnalytics.ReportRevenue_Admob(adValue, AppmetricaAnalytics.AdFormat.AppOpen, "");
        };

    }

    public void OnApplicationFocus(bool focus)
    {
        if (focus && !APPopendelay)
        {
            ShowAppOpenAdIfAvailable();
        }
    }

    bool IsAppOpenAdTimedOut
    {
        get
        {
            return _ad != null
                   && DateTime.Now < _expireTime;
        }
    }

    public void paidEvent(AdValue adValue)
    {
        ////ADJUST
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
        adValue.CurrencyCode, adValue.Value);

        double revenue = adValue.Value / 1000000f;

        var impressionParameters = new[] {
         new Firebase.Analytics.Parameter("ad_platform", "AdMob"),

         new Firebase.Analytics.Parameter("value", revenue),
         new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
         };

        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        Firebase.Analytics.FirebaseAnalytics.LogEvent("paid_ad_impression", impressionParameters);

        AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
        adjustAdRevenue.setRevenue(revenue, "USD");
        Adjust.trackAdRevenue(adjustAdRevenue);
    }
}

