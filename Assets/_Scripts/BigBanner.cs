
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Diagnostics;
using com.adjust.sdk;

public class BigBanner : MonoBehaviour
{
    // Start is called before the first frame update
    public AdPosition adsPosition;

    public static BigBanner instance;
    private void Awake()
    {
        instance = this;
    }

    static BannerView bannerView;
    string test_bannerID = "ca-app-pub-3940256099942544/6300978111";
    public string bannerID = "ca-app-pub-3940256099942544/6300978111";

    public void toSenRquest()
    {
        RequestBanner();
    }

    public void RequestBanner()
    {

        if (PlayerPrefs.GetInt("NoAds") == 1) return;
        if (PlayerPrefs.GetInt("RemoveAds") == 1) return;
        // Create a 320x50 banner at the top of the screen.
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        //AdSize adSize = new AdSize(250, 250);
        bannerView = new BannerView(AdmobIntilization._instance.isTestIdOn == false ? bannerID : test_bannerID, AdSize.MediumRectangle, adsPosition);
        ListenToAdEvents(bannerView);
        var adRequest = new AdRequest();

        // send the request to load the ad.

        bannerView.LoadAd(adRequest);
        bannerView.Hide();
    }
    private void ListenToAdEvents(BannerView _bannerView)
    {
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            AdmobIntilization._instance.isPausedDuetoAd = true;
        };

        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            print(String.Format("BigBanner view paid {0} {1}.",
            adValue.Value,
            adValue.CurrencyCode));
            paidEvent(adValue);
            //AppmetricaAnalytics.ReportRevenue_Admob(adValue, AppmetricaAnalytics.AdFormat.MREC, "");

        };
    }
    public void bannerBigBannerShow()
    {
        if (PlayerPrefs.GetInt("NoAds") == 1) return;
        if (PlayerPrefs.GetInt("RemoveAds") == 1) return;

        if (PlayerPrefs.GetInt("MaxAdStop") == 0)
        {
            if (AdsManager.instance.isMrecLoaded)
            {
                AdsManager.instance.ShowMRECBanner();
            }
            else
            {

                if (bannerView != null)
                {
                    bannerView.Show();

                }
                else
                {
                    RequestBanner();
                }
            }
        }
        else
        {
            if (bannerView != null)
            {
                bannerView.Show();

            }
            else
            {
                RequestBanner();
            }
        }

    }
    public void bannerBigBannerHide()
    {
        if (PlayerPrefs.GetInt("NoAds") == 1) return;
        if (PlayerPrefs.GetInt("RemoveAds") == 1) return;

        AdsManager.instance.HideMRECBanner();
        if (bannerView != null)
        {
            bannerView.Hide();
        }
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
