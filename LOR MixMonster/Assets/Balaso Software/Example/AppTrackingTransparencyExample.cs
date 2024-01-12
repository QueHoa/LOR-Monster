using System.Collections;
using Balaso;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Example MonoBehaviour class requesting iOS Tracking Authorization
/// </summary>
public class AppTrackingTransparencyExample : UnityEngine.MonoBehaviour
{
    private void Awake()
    {

#if UNITY_IOS
        AppTrackingTransparency.RegisterAppForAdNetworkAttribution();
        AppTrackingTransparency.UpdateConversionValue(3);
#endif
    }

    void Start()
    {
#if UNITY_IOS
        AppTrackingTransparency.OnAuthorizationRequestDone += OnAuthorizationRequestDone;

        AppTrackingTransparency.AuthorizationStatus currentStatus = AppTrackingTransparency.TrackingAuthorizationStatus;
        GameUtility.GameUtility.Log(string.Format("Current authorization status: {0}", currentStatus.ToString()));
        if (currentStatus != AppTrackingTransparency.AuthorizationStatus.AUTHORIZED)
        {
            GameUtility.GameUtility.Log("Requesting authorization...");
            AppTrackingTransparency.RequestTrackingAuthorization();
        }
#endif
    }

#if UNITY_IOS

    /// <summary>
    /// Callback invoked with the user's decision
    /// </summary>
    /// <param name="status"></param>
    private void OnAuthorizationRequestDone(AppTrackingTransparency.AuthorizationStatus status)
    {
        switch (status)
        {
            case AppTrackingTransparency.AuthorizationStatus.NOT_DETERMINED:
                GameUtility.GameUtility.Log("AuthorizationStatus: NOT_DETERMINED");
                break;
            case AppTrackingTransparency.AuthorizationStatus.RESTRICTED:
                GameUtility.GameUtility.Log("AuthorizationStatus: RESTRICTED");
                break;
            case AppTrackingTransparency.AuthorizationStatus.DENIED:
                GameUtility.GameUtility.Log("AuthorizationStatus: DENIED");
                break;
            case AppTrackingTransparency.AuthorizationStatus.AUTHORIZED:
                GameUtility.GameUtility.Log("AuthorizationStatus: AUTHORIZED");
                break;
        }

        // Obtain IDFA
        GameUtility.GameUtility.Log(string.Format("IDFA: {0}", AppTrackingTransparency.IdentifierForAdvertising()));
    }
#endif
}   
