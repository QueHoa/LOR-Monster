using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Ump;
using GoogleMobileAds.Ump.Api;

public class GDPRHandler : SingletonPersistent<GDPRHandler>
{
    [SerializeField]
    private List<string> testDevices = new List<string>();
    [SerializeField]
    private bool testMode = true;
    ConsentForm _consentForm;


    // Start is called before the first frame update
    public void RequestConsent()
    {
        var debugSettings = new ConsentDebugSettings
        {
            // Geography appears as in EEA for debug devices.
            DebugGeography = DebugGeography.EEA,
            TestDeviceHashedIds = testDevices
        };
        ConsentRequestParameters request;
        // Here false means users are not under age.
        if (testMode)
        {
            Debug.Log("INIT1 " + testMode);
            request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
                ConsentDebugSettings = debugSettings,
            };
        }
        else
        {
            Debug.Log("INIT2 " + testMode);
            request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };
        }


        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadConsentForm();
        }
        else
        {

        }
        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
    }

    void LoadConsentForm()
    {
        Time.timeScale = 0;
        // Loads a consent form.
        ConsentForm.Load(OnLoadConsentForm);
    }

    void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            Time.timeScale = 1;
            return;
        }

        // The consent form was loaded.
        // Save the consent form for future requests.
        _consentForm = consentForm;

        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            _consentForm.Show(OnShowForm);
        }
        else
        {
            Time.timeScale = 1;
        }
    }


    void OnShowForm(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            Time.timeScale = 1;
            return;
        }

        // Handle dismissal by reloading form.
        LoadConsentForm();
    }

}

