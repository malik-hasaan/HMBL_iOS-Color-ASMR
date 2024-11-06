using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

public class AdsIds : MonoBehaviour
{
    public static AdsIds instance;

    public static GameObject inappObj;

    [Space()]
    [Header("Unity InApp Define")]
    public InAppKeys[] InAppIds;

    [Space()]
    public GameObject inAppGameObject;

    void Awake()
    {
        if (SystemInfo.systemMemorySize <= 1024)
        {
            PlayerPrefs.SetInt("NoAds", 1);

            PlayerPrefs.SetInt("AdsStop", 1);
        }

        if (SystemInfo.systemMemorySize <= 2048)
        {
            PlayerPrefs.SetInt("MaxAdStop", 1);
        }

        if (instance == null)
            instance = this;

        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        inappObj = inAppGameObject;
    }


    #region In App Call Back

    public void AfterPurchased(int value)
    {
        if (InAppIds[value].removeads)
        {
            PlayerPrefs.SetInt("RemoveAds", 1);

            PlayerPrefs.SetInt("MaxAdStop", 1);

            BigBanner.instance.bannerBigBannerHide();

            AdsManager.instance.HideMRECBanner();

            AdsManager.instance.HideBanner();

            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (InAppIds[value].unLockAll)
        {
            PlayerPrefs.SetInt("UnlockAllIAP_Purchased", 1);

            PlayerPrefs.SetInt("RemoveAds", 1);

            PlayerPrefs.SetInt("MaxAdStop", 1);

            AdsManager.instance.HideMRECBanner();

            AdsManager.instance.HideBanner();
        }
    }

    #endregion
}

[System.Serializable]
public class InAppKeys
{
    public string Id;

    public ProductType Type;

    public bool removeads;

    public bool unLockAll;

    public int rewardAmount;
}
