using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Ump.Api;
using Sirenix.OdinInspector;
using UnityEngine;

public class GDPR_Android : MonoBehaviour
{
#if UNITY_ANDROID
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
            Debug.LogError("CMP UpdateError: " + consentError);
            InitLoadAds(isConsent: true);
            return;
        }

        // stop time to load consent form
        Time.timeScale = 0f;

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            Debug.Log("CMP LoadAndShowConsentFormIfRequired");
            Time.timeScale = 1f;

            if (formError != null)
            {
                // Consent gathering failed.
                Debug.LogError("CMP LoadAndShowError " + formError);
                InitLoadAds(true);
                return;
            }

            // Consent has been gathered.
            InitLoadAds(ConsentInformation.CanRequestAds());
        });
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