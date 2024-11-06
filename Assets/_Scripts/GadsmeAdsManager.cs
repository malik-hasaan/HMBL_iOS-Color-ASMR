using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gadsme;

public class GadsmeAdsManager : MonoBehaviour
{
    public Camera _mainCamera;
    public GadsmeRawImage rawImg;
    public static GadsmeAdsManager intance;
    void Start()
    {
        if (intance != null && intance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            intance = this;
        }

        GadsmeSDK.SetMainCamera(_mainCamera);
        //GadsmeEvents.ImpressionEvent += OnImpressionReceived;
        GadsmeSDK.Init();
    }

    void OnGamePhaseChange(Camera newCamera)
    {
        GadsmeSDK.SetMainCamera(newCamera); // Update Main camera
    }

    public void ShowGadsMeAd()
    {
        rawImg.enabled = true;
    }

    public void HideGadsMeAd()
    {
        rawImg.enabled = false;
    }

    private void OnImpressionReceived(GadsmeImpressionData impressionData)
    {
        //Debug.Log("IMPRESSION EVENT RECEIVED:");

        // Log the impression data
        Debug.Log("  placementId: " + impressionData.placementId);
        //Debug.Log("  gameId: " + impressionData.gameId);
        //Debug.Log("  countryCode: " + impressionData.countryCode);
        //Debug.Log("  currency: " + impressionData.currency);
        //Debug.Log("  netRevenue: " + impressionData.netRevenue);
        //Debug.Log("  lineItemType: " + impressionData.lineItemType);
        //Debug.Log("  platform: " + impressionData.platform);


        //FIREBASE
        //double revenue = impressionData.netRevenue;
        //var impressionParameters = new[] {
        // new Firebase.Analytics.Parameter("ad_platform", "Gadsme"),
        // new Firebase.Analytics.Parameter("value", revenue),
        // new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        // };
        //Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);

    }
}
