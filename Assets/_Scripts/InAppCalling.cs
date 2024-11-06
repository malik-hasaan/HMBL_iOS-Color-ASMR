using UnityEngine;

public class InAppCalling : MonoBehaviour
{

    public bool isInAppBtn;

    public GameObject RemoveAdsButton;

    void Start()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1 && isInAppBtn)
        {
            RemoveAdsButton.SetActive(false);
        }
    }

    public void BuyInApp(int value)
    {
        unityInApp.instance.BuyProductID(AdsIds.instance.InAppIds[value].Id, value);

    }

    public void restore()
    {
        unityInApp.instance.RestorePurchases();
    }
}

