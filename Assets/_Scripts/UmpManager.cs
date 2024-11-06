using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using GoogleMobileAds.Ump;
using GoogleMobileAds.Ump.Api;

public class UmpManager : MonoBehaviour
{
    public bool isTestMode;
    public void Start()
    {
        var debugSettings = new ConsentDebugSettings
        {
            // Geography appears as in EEA for debug devices.
            DebugGeography = DebugGeography.EEA,
            TestDeviceHashedIds = new List<string>
        {
            ""
        }
        };

        // Set tag for under age of consent.
        // Here false means users are not under age.
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
            ConsentDebugSettings = debugSettings,
        };

        if (isTestMode)
            request.ConsentDebugSettings.TestDeviceHashedIds.Add("3021088FF8D5A32DC449A6D54751B332");

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }
    void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            //  UnityEngine.Debug.LogError("Form>> info Updated error "+error.Message);
            return;
        }

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadForm();
        }
    }
    public void Restart()
    {
        PlayerPrefs.DeleteAll();
        Application.LoadLevel(Application.loadedLevel);
    }
    private ConsentForm _consentForm;

    void LoadForm()
    {
        // Loads a consent form.
        ConsentForm.Load(OnLoadConsentForm);
    }

    void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            // UnityEngine.Debug.LogError("Form Load Error "+error.Message);
            return;
        }
        // The consent form was loaded.
        // Save the consent form for future requests.
        _consentForm = consentForm;
        //  UnityEngine.Debug.LogError("Form>> Form Loaded");
        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            // Debug.LogError("Form>> Form Required Done");
            _consentForm.Show(OnDismissForm);
        }
    }
    char[] purposeConsentsDefault = new char[11];

    char ispersonlizd;
    void OnDismissForm(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            // UnityEngine.Debug.LogError("Form Show Error "+error.Message);
            return;
        }
        //Debug.Log("OnDismissForm");
        // Debug.LogError(" 1 ");
        string purposeConsents = ApplicationPreferences.GetString("IABTCF_PurposeConsents");
        //Debug.LogError(" 2 purposeConsents.Length" + purposeConsents.Length);
        int GdprApplies = ApplicationPreferences.GetInt("IABTCF_gdprApplies");
        //Debug.LogError(" 3 " + purposeConsents + " 2 " + GdprApplies);
        for (int i = 0; i < purposeConsentsDefault.Length; i++)
        {
            purposeConsentsDefault[i] = '0';
        }
        if (!string.IsNullOrEmpty(purposeConsents))
        {
            for (int i = 0; i < purposeConsents.Length; i++)
            {
                purposeConsentsDefault[i] = purposeConsents[i];//11111111
                //Debug.Log(purposeConsents[i]);
            }

            if (purposeConsents[0] == '1' && purposeConsents[1] == '1' && purposeConsents[2] == '1' && purposeConsents[3] == '1' && purposeConsents[6] == '1' && purposeConsents[8] == '1' && purposeConsents[9] == '1')
            {
                //personalized
                ispersonlizd = '1';
            }
            else if (purposeConsents[0] == '1' && purposeConsents[1] == '1' && purposeConsents[6] == '1' && purposeConsents[8] == '1' && purposeConsents[9] == '1')
            {
                //nonpersonalized
                ispersonlizd = '0';
            }
            FirebaseManager.Instance.SendFirebaseConsentDetail(ispersonlizd);
            //AdjustCustomEvents.Instance.AdjustUpdateOption(GdprApplies.ToString(), ispersonlizd, ispersonlizd);
        }
        // Handle dismissal by reloading form.
        LoadForm();
    }
}