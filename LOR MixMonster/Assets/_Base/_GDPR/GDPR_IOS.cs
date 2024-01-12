using UnityEngine;
using GoogleMobileAds.Ump.Api;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

public class GDPR_IOS : MonoBehaviour
{
#if UNITY_IOS
    public int delayMilliseconds = 500;
    public bool testMode;

    [ShowIf("@testMode")]
    public List<string> testDevices = new List<string> { "TEST-DEVICE-HASHED-ID" };

    private ConsentForm _consentForm;

    private async void Start()
    {
        await UniTask.Delay(delayMilliseconds);
        StarRequestCMP();
    }

    public void StarRequestCMP()
    {
        Debug.Log("CMP Start");
        ConsentRequestParameters request;

        if (testMode)
        {
            Debug.Log("CMP Test mode");
            ConsentInformation.Reset();

            var debugSettings = new ConsentDebugSettings
            {
                // Geography appears as in EEA for debug devices.
                DebugGeography = DebugGeography.EEA,
                TestDeviceHashedIds = testDevices
            };

            // Here false means users are not under age.
            request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
                ConsentDebugSettings = debugSettings
            };
        }
        else
        {
            request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
            };
        }

        Debug.Log("CMP ConsentInformation.Update...");
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    private void OnConsentInfoUpdated(FormError consentError)
    {
        Debug.Log("CMP OnConsentInfoUpdated");
        if (consentError != null)
        {
            // Handle the error.
            Debug.LogError("CMP UpdateError: " + consentError);
            InitLoadAds(isConsent: true);
            return;
        }

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        if (ConsentInformation.IsConsentFormAvailable())
        {
            Debug.Log("CMP IsConsentFormAvailable = true, LoadConsentForm...");
            LoadConsentForm();
        }
        else
        {
            Debug.Log("CMP IsConsentFormAvailable = false, init ads...");
            InitLoadAds(true);
        }
    }

    private void LoadConsentForm()
    {
        Debug.Log("CMP LoadConsentForm...");
        Time.timeScale = 0; // pause to load form
        ConsentForm.Load(OnLoadConsentForm);
    }

    private void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        Debug.Log("CMP OnLoadConsentForm");

        if (error != null)
        {
            Debug.LogError("CMP LoadFormError" + error);
            InitLoadAds(true);
            return;
        }

        // The consent form was loaded.
        // Save the consent form for future requests.
        _consentForm = consentForm;

        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            Debug.Log("CMP Show Form Required...");
            _consentForm.Show(OnShowForm);
        }
        else
        {
            Debug.Log($"CMP ConsentStatus = {ConsentInformation.ConsentStatus}, InitLoadAds...");
            InitLoadAds(true);
        }
    }

    private void OnShowForm(FormError error)
    {
        Debug.Log("CMP OnShowForm");

        if (error != null)
        {
            // Handle the error.
            Debug.LogError("CMP ShowFormError " + error);
            InitLoadAds(true);
            return;
        }

        // Handle dismissal by reloading form.
        LoadConsentForm();
    }

    private void InitLoadAds(bool isConsent)
    {
        Time.timeScale = 1f;

        AD.Controller.Instance.Init(DataManagement.DataManager.Instance.userData.IsAd);
        AD.Controller.Instance.LoadOpenAd();
        AD.Controller.Instance.LoadBanner();
        AD.Controller.Instance.LoadInterstitial();
        AD.Controller.Instance.LoadNativeAd();
    }
#endif
}