using com.adjust.sdk;
using Cysharp.Threading.Tasks;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AD
{
    public class IronSourceAdController : MonoBehaviour, IAdHandler
    {
        bool isReady = false;
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        public void Init()
        {
            var developerSettings = Resources.Load<IronSourceMediationSettings>(IronSourceConstants.IRONSOURCE_MEDIATION_SETTING_NAME);
            if (developerSettings != null)
            {

#if UNITY_IOS
            try{
            AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(AppTrackingListenner.isAllow);
            }catch(System.Exception e){
            GameUtility.GameUtility.LogError(e);
            }
#endif
                string appKey = "";
#if UNITY_ANDROID
                appKey = developerSettings.AndroidAppKey;
#elif UNITY_IOS
             appKey = developerSettings.IOSAppKey;
#endif

                GameUtility.GameUtility.Log("IronSourceInitilizer " + appKey);
                IronSourceConfig.Instance.setClientSideCallbacks(true);

                if (appKey.Equals(string.Empty))
                {
                    GameUtility.GameUtility.LogWarning("IronSourceInitilizer Cannot init without AppKey");
                }
                else
                {
                    //appKey= PlayerPrefs.GetString("IronSourceAdID", "165cebd45"); 
                    GameUtility.GameUtility.Log("-IronSourceInitilizer " + appKey);
                    IronSource.Agent.init(appKey);
                    IronSource.UNITY_PLUGIN_VERSION = "7.2.1-ri";
                }

                GameUtility.GameUtility.Log("Adapter debug " + developerSettings.EnableAdapterDebug);
                if (developerSettings.EnableAdapterDebug)
                {
                    GameUtility.GameUtility.Log("Ironsource EnableAdapterDebug ");
                    IronSource.Agent.setAdaptersDebug(true);
                }

                GameUtility.GameUtility.Log("EnableIntegrationHelper " + developerSettings.EnableIntegrationHelper);
                if (developerSettings.EnableIntegrationHelper)
                {
                    GameUtility.GameUtility.Log("Ironsource validateIntegration ");
                    IronSource.Agent.validateIntegration();
                }

                IronSourceEvents.onSdkInitializationCompletedEvent -= SdkInitializationCompletedEvent;
                IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
                IronSourceEvents.onImpressionDataReadyEvent -= ImpressionDataReadyEvent;
                IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;


                //string id = IronSource.Agent.getAdvertiserId();
                //GameUtility.GameUtility.Log("unity-script: IronSource.Agent.getAdvertiserId : " + id);
                //IronSource.Agent.setMetaData("is_test_suite", "enable");

                ISAdQualityConfig iSAdQualityConfig = new ISAdQualityConfig();
                IronSourceAdQuality.Initialize(appKey, iSAdQualityConfig);
            }
        }
        public bool IsReady()
        {
            return isReady;
        }
        void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            GameUtility.GameUtility.Log("----------IMPRESSION DATA READY EVENT " + impressionData.adNetwork);
            FirebaseAnalysticController.Instance.SendRevenueToFireBase(impressionData);
            FirebaseAnalysticController.Instance.SendRevenueToAdjust(impressionData);
        }





        private void SdkInitializationCompletedEvent()
        {
            GameUtility.GameUtility.Log("Ironsource INITialzied");
            isReady = true;
            //reward
            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdLoadFailedEvent += IronSourceEvents_onRewardedVideoAdLoadFailedEvent;


            //banner


            IronSourceBannerEvents.onAdLoadedEvent += IronSourceEvents_onBannerAdLoadedEvent; ;
            IronSourceBannerEvents.onAdLoadFailedEvent += IronSourceEvents_onBannerAdLoadFailedEvent; ;
            IronSourceBannerEvents.onAdClickedEvent += IronSourceEvents_onBannerAdClickedEvent; ;
            IronSourceBannerEvents.onAdScreenPresentedEvent += IronSourceEvents_onBannerAdScreenPresentedEvent; ;
            IronSourceBannerEvents.onAdScreenDismissedEvent += IronSourceEvents_onBannerAdScreenDismissedEvent; ;
            IronSourceBannerEvents.onAdLeftApplicationEvent += IronSourceEvents_onBannerAdLeftApplicationEvent; ;

            // interstitial
            IronSourceInterstitialEvents.onAdReadyEvent += IronSourceEvents_onInterstitialAdReadyEvent; ;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += IronSourceEvents_onInterstitialAdLoadFailedEvent; ;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += IronSourceEvents_onInterstitialAdShowSucceededEvent; ;
            IronSourceInterstitialEvents.onAdShowFailedEvent += IronSourceEvents_onInterstitialAdShowFailedEvent; ;
            IronSourceInterstitialEvents.onAdClickedEvent += IronSourceEvents_onInterstitialAdClickedEvent; ;
            IronSourceInterstitialEvents.onAdOpenedEvent += IronSourceEvents_onInterstitialAdOpenedEvent; ;
            IronSourceInterstitialEvents.onAdClosedEvent += IronSourceEvents_onInterstitialAdClosedEvent; ;



        }
        #region interstitial event
        private void IronSourceEvents_onInterstitialAdClosedEvent(IronSourceAdInfo adInfo)
        {
            onInstitialClose?.Invoke();
            onInstitialClose = null;
            onInstitialShow = null;
            Controller.Instance.lastSuccessInterstitialTime = Time.time;
            LoadInterstitial();
#if UNITY_IOS
            AudioListener.pause = false;
#endif
        }

        private void IronSourceEvents_onInterstitialAdOpenedEvent(IronSourceAdInfo adInfo)
        {

        }

        private void IronSourceEvents_onInterstitialAdClickedEvent(IronSourceAdInfo adInfo)
        {
        }

        private void IronSourceEvents_onInterstitialAdShowFailedEvent(IronSourceError obj, IronSourceAdInfo adInfo)
        {
            onInstitialShow?.Invoke(false);
            onInstitialShow = null;
            IronSource.Agent.SetPauseGame(false);
#if UNITY_IOS
                AudioListener.pause = false;
#endif
            GameUtility.GameUtility.Log("IronSourceEvents_onInterstitialAdShowFailedEvent " + obj.getDescription() + " " + obj.getErrorCode() + " " + obj.getCode());
        }

        private void IronSourceEvents_onInterstitialAdShowSucceededEvent(IronSourceAdInfo adInfo)
        {
            onInstitialShow?.Invoke(true);
            onInstitialShow = null;
            IronSource.Agent.SetPauseGame(false);
#if UNITY_IOS
                AudioListener.pause = false;
#endif
            GameUtility.GameUtility.Log("IronSourceEvents_onInterstitialAdShowSucceededEvent ");

        }

        private void IronSourceEvents_onInterstitialAdLoadFailedEvent(IronSourceError obj)
        {
            GameUtility.GameUtility.Log("IronSourceEvents_onInterstitialAdLoadFailedEvent " + obj.getDescription() + " " + obj.getErrorCode() + " " + obj.getCode());
        }

        private void IronSourceEvents_onInterstitialAdReadyEvent(IronSourceAdInfo adInfo)
        {
            GameUtility.GameUtility.Log("IronSourceEvents_onInterstitialAdReadyEvent ");
        }
        #endregion
        #region banner event
        private void IronSourceEvents_onBannerAdLeftApplicationEvent(IronSourceAdInfo adInfo)
        {
        }

        private void IronSourceEvents_onBannerAdScreenDismissedEvent(IronSourceAdInfo adInfo)
        {
        }

        private void IronSourceEvents_onBannerAdScreenPresentedEvent(IronSourceAdInfo adInfo)
        {
        }

        private void IronSourceEvents_onBannerAdClickedEvent(IronSourceAdInfo adInfo)
        {
        }

        private void IronSourceEvents_onBannerAdLoadFailedEvent(IronSourceError obj)
        {
            GameUtility.GameUtility.Log("IronSource: IronSourceEvents_onBannerAdLoadedEvent" + obj.getErrorCode() + " " + obj.getCode() + " " + obj.getDescription());
            isBannerLoaded = false;
        }

        private void IronSourceEvents_onBannerAdLoadedEvent(IronSourceAdInfo adInfo)
        {
            GameUtility.GameUtility.Log("IronSource: IronSourceEvents_onBannerAdLoadedEvent");
            isBannerLoaded = true;


        }
        #endregion
        #region reward event
        private void IronSourceEvents_onRewardedVideoAdLoadFailedEvent(IronSourceError obj)
        {
            GameUtility.GameUtility.LogError("Ironsource: RewardedVideoAdLoadFailedEvent " + obj.getErrorCode() + " " + obj.getCode() + " " + obj.getDescription());
            // Rewarded ad failed to load 
            rewardLoadRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardLoadRetryAttempt));
            Invoke("LoadRewardedAd", (float)retryDelay);
        }
        private void RewardedVideoAdShowFailedEvent(IronSourceError obj, IronSourceAdInfo adInfo)
        {
            GameUtility.GameUtility.LogError("Ironsource: RewardedVideoAdShowFailedEvent " + obj.getErrorCode() + " " + obj.getCode() + " " + obj.getDescription());

        }

        private void RewardedVideoAdRewardedEvent(IronSourcePlacement obj, IronSourceAdInfo adInfo)
        {
            GameUtility.GameUtility.Log("Ironsource: RewardedVideoAdRewardedEvent ");
            OnReward();

        }

        private void RewardedVideoAdEndedEvent()
        {
            GameUtility.GameUtility.Log("Ironsource VIDEO ENDED");
        }

        private void RewardedVideoAdStartedEvent()
        {
            GameUtility.GameUtility.Log("Ironsource VIDEO STARTED");

        }

        private void RewardedVideoAvailabilityChangedEvent(IronSourceAdInfo adInfo)
        {
            GameUtility.GameUtility.Log("Ironsource: RewardedVideoAvailabilityChangedEvent " + (adInfo != null));
        }

        private void RewardedVideoAdClosedEvent(IronSourceAdInfo adInfo)
        {
            GameUtility.GameUtility.Log("Ironsource VIDEO CLOSED");
            isAdPlaying = false;
            AudioListener.pause = false;
            Time.timeScale = 1;
            Controller.Instance.OnRewardClosed();
            if (WaitingPanel.Instance != null && WaitingPanel.Instance.gameObject.activeSelf)
            {
                WaitingPanel.Instance.gameObject.SetActive(false);
            }
            IronSource.Agent.SetPauseGame(false);
        }

        private void RewardedVideoAdClickedEvent(IronSourcePlacement obj)
        {
            GameUtility.GameUtility.Log("Ironsource VIDEO CLICKED");
        }

        private void RewardedVideoAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            GameUtility.GameUtility.Log("Ironsource VIDEO OPENED");
            IronSource.Agent.SetPauseGame(true);
            isAdPlaying = true;
            AudioListener.pause = true;
            Time.timeScale = 0;
            Controller.Instance.OnRewardStarted();
        }
        #endregion





        #region rewarded ad
        public bool isAdPlaying = false;
        Action<bool> onRewarded;
        async UniTaskVoid Invoke(System.Action onAction, float time)
        {
            await UniTask.Delay((int)(time * 1000), ignoreTimeScale: true);
            onAction?.Invoke();
        }
        public void ShowRewardedAd(Action<bool> onRewarded)
        {
            CancelInvoke();
            if (onRewarded == null) return;
            this.onRewarded = onRewarded;
            rewardSuccess = false;

            GameUtility.GameUtility.Log("Ironsource: showrewardad " + IsRewardAvailable());
            if (IsRewardAvailable())
            {
                Invoke(ShowAd, 1f).Forget();
            }
            else
            {
                LoadRewardedAd();
                GameUtility.GameUtility.Log("Waiting for ad");
                WaitForRewardAd(result =>
                {
                    if (result)
                    {
                        ShowRewardedAd(onRewarded);
                    }
                    else
                    {
                        OnShowRewardFailed();
                    }

                }).Forget();

            }
        }

        void ShowAd()
        {
            if (IsRewardAvailable())
            {
                IronSource.Agent.showRewardedVideo();
            }
        }
        //đợi ad khi nào available thì trả về true, timeout thì trả false
        async UniTaskVoid WaitForRewardAd(System.Action<bool> onAction)
        {
            float timeOut = 8;
            while (timeOut > 0)
            {
                timeOut -= Time.fixedUnscaledDeltaTime;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

                if (IsRewardAvailable())
                {
                    onAction?.Invoke(true);
                    return;
                }
            }
            onAction?.Invoke(false);

        }
        public void LoadRewardedAd()
        {
            GameUtility.GameUtility.Log("IronSource Loadreward ad");
            IronSource.Agent.loadRewardedVideo();

        }

        public bool IsRewardAvailable()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }
        public void OnReward()
        {
            rewardSuccess = true;
            onRewarded?.Invoke(true);
            onRewarded = null;
        }
        public void OnShowRewardFailed()
        {
            onRewarded?.Invoke(false);
            onRewarded = null;
        }
        float previousTimeScale = 1;

        bool rewardSuccess = false;

        int rewardLoadRetryAttempt;
        #endregion

        #region interstitialAd
        Action<bool> onInstitialShow;
        Action onInstitialClose;
        public void LoadInterstitial()
        {
            IronSource.Agent.loadInterstitial();
        }
        public void OnShowInterstitialFailed()
        {
        }
        public void ShowInterstitial(Action<bool> onShow, Action onInstitialClose)
        {
            GameUtility.GameUtility.Log("IS: show inte");
            if (IronSource.Agent.isInterstitialReady())
            {
#if UNITY_IOS
                AudioListener.pause = true;
#endif
                IronSource.Agent.SetPauseGame(true);

                this.onInstitialShow = onShow;
                this.onInstitialClose = onInstitialClose;
                IronSource.Agent.showInterstitial();

            }
            else
            {
                onInstitialClose?.Invoke();
                onInstitialClose = null;
                LoadInterstitial();
                GameUtility.GameUtility.Log("unity-script: IronSource.Agent.isInterstitialReady - False");
            }
        }
        #endregion
        #region banner ad
        bool isBannerLoaded = false;
        public bool IsBannerLoaded()
        {
            return isBannerLoaded;
        }
        public void ShowBanner()
        {
            GameUtility.GameUtility.Log("IronSource: ShowBanner");
            if (IsBannerLoaded())
                IronSource.Agent.displayBanner();
        }
        public void LoadBanner()
        {
            GameUtility.GameUtility.Log("IronSource: Loadbanner");
            IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
        }

        public void HideBanner()
        {
            GameUtility.GameUtility.Log("IronSource: HideBanner");
            IronSource.Agent.hideBanner();
        }

        #endregion





        #region openAD

        public bool IsOpenAdAvailable()
        {
            return false;
        }

        public void LoadOpenAd()
        {
        }

        public void ShowOpenAd(System.Action<bool> onShow)
        {
        }

        #endregion

        void OnApplicationPause(bool isPaused)
        {
            GameUtility.GameUtility.Log("IronSource Pause:" + isPaused);
            IronSource.Agent.onApplicationPause(isPaused);
        }

        public void LoadNativeAd()
        {
        }

        public bool IsNativeAdLoaded()
        {
            return false;
        }

        public object GetCurrentNativeAd()
        {
            return null;
        }
    }
}