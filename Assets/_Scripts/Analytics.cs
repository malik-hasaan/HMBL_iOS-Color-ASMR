using UnityEngine;
using GameAnalyticsSDK;
using System;
//using Facebook.Unity;

public class Analytics : MonoBehaviour
{
    public static Analytics inst = null;
    private int rulesRecallTime = 0;
    public static bool isNewUser = false;

    public string valSt;
    private void Awake()
    {
        if (inst != null && inst != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        //   Invoke(nameof(InitializeGMAnalytics), 2f);

        GameAnalytics.Initialize();
    }

    void InitializeGMAnalytics()
    {
        GameAnalytics.Initialize();
    }

}