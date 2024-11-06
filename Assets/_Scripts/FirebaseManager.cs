using System;
using Firebase;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.Messaging;
using System.Threading.Tasks;
using System.Collections.Generic;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    public static string FirebaseRemoteKey;
    public LevelData levelList = new LevelData();

    //public static int FirstAdsTimer;
    //public static int AdsTimer;

    void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;

        DontDestroyOnLoad(gameObject);

        OnFireBase();
    }

    #region Firebase

    DependencyStatus _dependencyStatus = DependencyStatus.UnavailableOther;
    bool _firebaseInitialized = false;
    void OnFireBase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            _dependencyStatus = task.Result;
            if (_dependencyStatus == DependencyStatus.Available)
            {
                StartCoroutine(delay());
                InitializeFirebaseMessaging();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + _dependencyStatus);
            }
        });
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(4f);
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        try
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

            FirebaseAnalytics.SetUserProperty(
                FirebaseAnalytics.UserPropertySignUpMethod,
                "Google");

            _firebaseInitialized = true;
            var app = FirebaseApp.DefaultInstance;
            var defaults = new Dictionary<string, object>
            {
            };
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
                .ContinueWithOnMainThread(task =>
                {
                    FetchDataAsync();
                });
        }
        catch { }
    }

    void InitializeFirebaseMessaging()
    {
        FirebaseMessaging.MessageReceived += OnMessageReceived;
        FirebaseMessaging.TokenReceived += OnTokenReceived;

        FirebaseMessaging.SubscribeAsync("general_notifications").ContinueWithOnMainThread(task =>
        {
            Debug.Log("Subscribed to general_notifications topic.");
        });
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("___Firebase Messaging Token: " + token.Token);
        // You can save or use the token for server-side operations.
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from Firebase!");
        if (e.Message.Notification != null)
        {
            string title = e.Message.Notification.Title;
            string body = e.Message.Notification.Body;
            string icon = e.Message.Notification.Icon;

            Debug.Log("___Title: " + title);
            Debug.Log("___Body: " + body);
            Debug.Log("___Icon: " + icon);

            // Display the message in the UI
            try
            {
                ShowNotificationInGame(title, body);
            }
            catch
            {
            }
        }

    }

    void ShowNotificationInGame(string title, string body)
    {
        if (GameManager.Instance.notificationPanel != null)
        {
            GameManager.Instance.notificationTitleText.text = title;
            GameManager.Instance.notificationBodyText.text = body;

            // Show the notification panel
            GameManager.Instance.notificationPanel.SetActive(true);

            // Optionally, hide it after a few seconds
            StartCoroutine(HideNotificationAfterDelay(5f)); // Hides after 5 seconds
        }
    }

    IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.notificationPanel.SetActive(false);
    }

    #endregion
    public void ReportEvent(string eventName)
    {
        if (_firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent(eventName);
        }
    }
    public void ReportEvent(string eventName, string parameterName, string parameterValue)
    {
        if (_firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent(eventName, parameterName, parameterValue);
        }
    }

    Task FetchDataAsync()
    {
        var fetchTask =
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    void FetchComplete(Task fetchTask)
    {
        try
        {
            if (fetchTask.IsCanceled)
            {
            }
            else if (fetchTask.IsFaulted)
            {
            }
            else if (fetchTask.IsCompleted)
            {
            }

            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case Firebase.RemoteConfig.LastFetchStatus.Success:

                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                        .ContinueWithOnMainThread(task =>
                        {
                            GetRemoteData();
                        });

                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            //defaultRemoteDate();
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            //Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    //Debug.Log("Latest Fetch call still pending.");
                    break;
            }
        }
        catch { }
    }

    void GetRemoteData()
    {

        //try
        //{
        //    FirstAdsTimer = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("firstadstimer").DoubleValue;

        //    Debug.Log("First Ad Timer = " + FirstAdsTimer);
        //}
        //catch
        //{
        //    FirstAdsTimer = 0;
        //}


        //try
        //{
        //    AdsTimer = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("adstimer").DoubleValue;

        //    Debug.Log("Ads Timer = " + AdsTimer);
        //}
        //catch
        //{
        //    AdsTimer = 0;
        //}


        //if (PlayerPrefs.GetInt("FirstAdTimer", 0) == 0)
        //{
        //    AdCaller._inst.time = FirstAdsTimer;

        //    AdCaller._inst.resetTime();

        //    AdCaller._inst.time = AdsTimer;

        //    PlayerPrefs.SetInt("FirstAdTimer", 1);
        //}

        //else
        //{
        //    AdCaller._inst.time = AdsTimer;

        //    AdCaller._inst.resetTime();
        //}

        try
        {
            FirebaseRemoteKey = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("levelorder").StringValue;

            takeStrFun(FirebaseRemoteKey);
        }
        catch
        {
        }

    }

    public void takeStrFun(string jsonString)
    {
        levelList = JsonUtility.FromJson<LevelData>(jsonString);
    }

    ConsentStatus _consentStatus;
    Dictionary<ConsentType, ConsentStatus> consentdata = new Dictionary<ConsentType, ConsentStatus>();
    public void SendFirebaseConsentDetail(char ad_Personalization)
    {
        _consentStatus = (ad_Personalization == '1') ? (ConsentStatus.Granted) : (ConsentStatus.Denied);
        consentdata.Add(ConsentType.AnalyticsStorage, _consentStatus);

        _consentStatus = (ad_Personalization == '1') ? (ConsentStatus.Granted) : (ConsentStatus.Denied);
        consentdata.Add(ConsentType.AdStorage, _consentStatus);

        _consentStatus = (ad_Personalization == '1') ? (ConsentStatus.Granted) : (ConsentStatus.Denied);
        consentdata.Add(ConsentType.AdUserData, _consentStatus);

        _consentStatus = (ad_Personalization == '1') ? (ConsentStatus.Granted) : (ConsentStatus.Denied);
        consentdata.Add(ConsentType.AdPersonalization, _consentStatus);

        FirebaseAnalytics.SetConsent(consentdata);
    }
}

[Serializable]
public class LevelData
{
    public List<int> Levels;
}