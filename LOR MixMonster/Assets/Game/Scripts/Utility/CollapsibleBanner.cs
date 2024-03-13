using System.Collections;
using System.Collections.Generic;
using com.adjust.sdk;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using UnityEngine;

public class CollapsibleBanner
{
    public static string bannerAdID = "ca-app-pub-1370756799605534/6463281002";
    private static BannerView _bannerView;
    public static void CreateBannerView()
    {
        Logger.Log("Creating banner view");
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }

        _bannerView = new BannerView(bannerAdID, AdSize.Banner, AdPosition.Bottom);
        ListenToAdEvents();
    }
    private static void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Logger.Log("Banner view loaded an ad with response : "
                       + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Logger.LogWarning("Banner view failed to load an ad with error : "
                              + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (adValue) => { OnBannerAdPaid(adValue); };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () => { Logger.Log("Banner view recorded an impression."); };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () => { Logger.Log("Banner view was clicked."); };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () => { Logger.Log("Banner view full screen content opened."); };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () => { Logger.Log("Banner view full screen content closed."); };
    }
    public static void LoadBanner()
    {
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();
        adRequest.Extras.Add("collapsible", "bottom");
        Logger.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }
    public static void ShowBanner()
    {
        Logger.LogWarning("Call Show Banner from admob");
        LoadBanner();
        _bannerView.Show();
        AD.Controller.Instance.HideBanner();
    }
    public static void HideBanner()
    {
        _bannerView.Hide();
        AD.Controller.Instance.ShowBanner();
    }
    public static void OnBannerAdPaid(AdValue adValue)
    {
        Logger.LogWarning("ADS MANAGER: On App Open Ad Paid");

        double revenue = adValue.Value / 1000000f;
        var imp = new[]
        {
            new Parameter("ad_platform", "Admob"),
            new Parameter("ad_source", "Admob"),
            new Parameter("ad_unit_name", "banner_ads)"),
            new Parameter("ad_format", "banner_ads"),
            new Parameter("value", revenue),
            new Parameter("currrency", adValue.CurrencyCode)
        };
        FirebaseAnalytics.LogEvent("ad_impression", imp);
        FirebaseAnalytics.LogEvent("ad_revenue_sdk", imp);
        FirebaseAnalytics.LogEvent("cc_openad_native_revenue", imp);

        AdjustAdRevenue adjustEvent = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
        //most important is calling setRevenue with two parameters
        adjustEvent.setRevenue(revenue, adValue.CurrencyCode);
        //Sent event to Adjust server
        Adjust.trackAdRevenue(adjustEvent);
    }
}