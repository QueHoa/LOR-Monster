using com.adjust.sdk;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AD
{
    public class NativeAdPack
    {
        public NativeAd nativeAd;
        public bool isReady = false;
    }
    public class AdmobController : MonoBehaviour, IAdHandler
    {

        private string openAdId, nativeAdId;
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private AppOpenAd appOpenAd;
        public Stack<NativeAd> nativeAds = new Stack<NativeAd>();

        public void Init()
        {
            SDKConfigData config = Resources.Load<SDKConfigData>("SdkConfig");
#if UNITY_ANDROID
            openAdId = config.sdkIdConfig.admobOpenAdID_Android;
            nativeAdId = config.sdkIdConfig.admobNativeAdID_Android;
#elif UNITY_IOS
            openAdId = config.sdkIdConfig.admobOpenAdID_Ios;
            nativeAdId = config.sdkIdConfig.admobNativeAdID_Ios;

#endif

#if UNITY_EDITOR
            GameUtility.GameUtility.Log("OpenadId " + openAdId);
            GameUtility.GameUtility.Log("nativeAdId " + nativeAdId);
#endif
            GoogleMobileAds.Api.MobileAds.Initialize((GoogleMobileAds.Api.InitializationStatus initStatus) =>
            {
                // This callback is called once the MobileAds SDK is initialized.
                GameUtility.GameUtility.Log("INIT ADMOB");
            });


        }
        System.Action<bool> onShow;
        public void ShowOpenAd(System.Action<bool> onShow)
        {
            this.onShow = onShow;
            if (appOpenAd != null)
            {
                GameUtility.GameUtility.Log("Showing app open ad.");
                appOpenAd.Show();

            }
            else
            {
                GameUtility.GameUtility.LogError("App open ad is not ready yet.");
                onShow?.Invoke(false);
                LoadOpenAd();
            }

        }
        public void LoadOpenAd()
        {
            if (appOpenAd != null) return;

            GameUtility.GameUtility.Log("Loading the app open ad.");

            var adRequest = new AdRequest();
            AppOpenAd.Load(openAdId, ScreenOrientation.Portrait, adRequest,
                (AppOpenAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        GameUtility.GameUtility.LogError("app open ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }
                    GameUtility.GameUtility.Log("App open ad loaded with response : "
                              + ad.GetResponseInfo());
                    appOpenAd = ad;
                    appOpenAd.OnAdPaid += AppOpenAd_OnPaidEvent;
                    appOpenAd.OnAdFullScreenContentClosed += AppOpenAd_OnAdDidPresentFullScreenContent;
                    appOpenAd.OnAdFullScreenContentOpened += AppOpenAd_OnAdFullScreenContentOpened; ;
                });
        }

        private void AppOpenAd_OnAdFullScreenContentOpened()
        {
#if UNITY_IOS
                AudioListener.pause = true;
#endif
        }

        private void AppOpenAd_OnAdDidPresentFullScreenContent()
        {
            GameUtility.GameUtility.Log("ADMOB OnAdFullScreenContentClosed");
            onShow?.Invoke(true);
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }

            LoadOpenAd();
#if UNITY_IOS
                AudioListener.pause = false;
#endif
        }

        private void AppOpenAd_OnPaidEvent(AdValue adValue)
        {
            GameUtility.GameUtility.Log("ADMOB ONPAID EVENT");
            FirebaseAnalysticController.Instance.SendRevenueToFirebase(adValue);
            FirebaseAnalysticController.Instance.SendRevenueToAdjust(adValue);
        }

        public bool IsOpenAdAvailable()
        {
            return appOpenAd != null;
        }

        public bool IsReady()
        {
            return true;
        }

        public bool IsRewardAvailable()
        {
            return false;

        }

        public void LoadBanner()
        {
        }

        public void LoadInterstitial()
        {
        }
        public void LoadRewardedAd()
        {
        }
        public void HideBanner()
        {
        }
        public void OnReward()
        {
        }

        public void OnShowRewardFailed()
        {
        }

        public void ShowBanner()
        {
        }

        public void ShowInterstitial(System.Action<bool> onShow, System.Action onClose)
        {
        }



        public void ShowRewardedAd(Action<bool> onRewared)
        {
        }

        public void ShowRewardedAd2()
        {
        }

        public bool IsBannerLoaded()
        {
            return false;
        }

        #region NativeAd
        public void LoadNativeAd()
        {
            if (IsNativeAdLoaded()) return;
            Debug.Log("ADMOB LOAD NATIVE AD");
            AdLoader adLoader = new AdLoader.Builder(nativeAdId)
         .ForNativeAd().SetNumberOfAdsToLoad(1)
         .Build();
            adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
            adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;
            adLoader.OnNativeAdImpression += AdLoader_OnNativeAdImpression;
            adLoader.OnNativeAdClicked += AdLoader_OnNativeAdClicked;
            adLoader.OnNativeAdOpening += AdLoader_OnNativeAdOpening;
            adLoader.LoadAd(new AdRequest());
        }

        private void AdLoader_OnNativeAdOpening(object sender, EventArgs e)
        {
            Debug.Log("Native ad AdLoader_OnNativeAdOpening ");

        }

        private void AdLoader_OnNativeAdClicked(object sender, EventArgs e)
        {
            Debug.Log("Native ad AdLoader_OnNativeAdClicked ");
        }

        private void AdLoader_OnNativeAdImpression(object sender, EventArgs e)
        {
            Debug.Log("Native ad AdLoader_OnNativeAdImpression ");
        }
        int reloadTime = 0;
        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Debug.Log("Native ad HandleAdFailedToLoad ");
            if (reloadTime < 4)
            {
                Invoke(nameof(LoadNativeAd), 3);
                reloadTime++;
            }

        }

        private void HandleNativeAdLoaded(object sender, NativeAdEventArgs e)
        {
            reloadTime = 0;
            NativeAd nativeAd = e.nativeAd;
            nativeAd.OnPaidEvent -= NativeAd_OnPaidEvent;
            nativeAd.OnPaidEvent += NativeAd_OnPaidEvent;

            nativeAds.Push(nativeAd);
            Debug.Log("Native ad loaded " + e.nativeAd.GetHeadlineText() + " " + nativeAd.GetResponseInfo().GetResponseId() + " count:" + nativeAds.Count);
        }

        private void NativeAd_OnPaidEvent(object sender, AdValueEventArgs e)
        {

            Debug.Log("ADMOB natve ONPAID EVENT");
            FirebaseAnalysticController.Instance.SendRevenueToFirebase(e.AdValue);
            FirebaseAnalysticController.Instance.SendRevenueToAdjust(e.AdValue);
        }

        public void ShowNativeAd(Action<bool> onShow)
        {
        }

        public bool IsNativeAdLoaded()
        {
            return nativeAds.Count > 0;
        }

        public object GetCurrentNativeAd()
        {
            Debug.Log("GET NATIVE AD: " + nativeAds.Count);
            if (nativeAds.Count > 0)
            {
                Debug.Log("+native>>>>> " + nativeAds.Peek().GetHeadlineText());
                return nativeAds.Pop();
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}