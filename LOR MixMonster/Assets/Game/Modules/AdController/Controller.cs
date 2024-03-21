using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AD
{
    public class Controller
    {
        public static Controller Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Controller();
                }
                return instance;
            }

            set
            {
                if (instance == null)
                {
                    instance = value;
                }
            }

        }
        private static Controller instance;
        List<IAdHandler> adHandler = new List<IAdHandler>();

        public delegate void OnAdCompleted();
        public OnAdCompleted onAdCompleted;
        public delegate void OnAdStarted();
        public OnAdStarted onAdStarted;

        public delegate void OnNativeAdRefresh();
        public OnNativeAdRefresh onNativeAdRefresh;

        public bool isBusy = false;
        public bool isAd = false;
        public float lastSuccessInterstitialTime = 0;
        public float lastShowOpenAd = 0;
        public Controller()
        {
        }
        bool isInit;
        public void Init(bool isAd)
        {
            if (isInit) return;
            isInit = true;
            GameUtility.GameUtility.Log("AD INIT");
            GameObject icObj = new GameObject("IronSourceController", typeof(IronSourceAdController));
            IronSourceAdController icAdCtr = icObj.GetComponent<IronSourceAdController>();
            adHandler.Add(icAdCtr);
            //ko dung admob thi bo di
            GameObject admobObj = new GameObject("AdmobController", typeof(AdmobController));
            AdmobController admobAdCtr = admobObj.GetComponent<AdmobController>();
            adHandler.Add(admobAdCtr);

            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.Init();
            }
            this.isAd = isAd;
        }

        float lastTimeShowAd = 0;
        public void ShowRewardedAd(string place, System.Action<bool> onRewared, bool canSkip = false)
        {
            // k cho spam gọi show QC
#if !UNITY_EDITOR
            if (Time.realtimeSinceStartup - lastTimeShowAd < 5) return;
#endif

            if (canSkip || Game.Controller.Instance.gameConfig.skipAd)
            {
                onRewared?.Invoke(true);
                return;
            }

            isBusy = true;
            lastTimeShowAd = Time.realtimeSinceStartup;

            // hiện màn hình Loading AD
            WaitingPanel.Create((panel) =>
            {
                ((WaitingPanel)panel).SetUp();

                Show();
            });

            void Show()
            {
                //log event start Reward ở 1 nút nào đấy
                FirebaseAnalysticController.Instance.LogStartAd(place);
                System.GC.Collect();

                //
                foreach (IAdHandler adHandler in adHandler)
                {
                    adHandler.ShowRewardedAd(async (result) =>
                    {
                        GameUtility.GameUtility.Log("AD REWARD :" + result);
                        if (result)
                        {
                            FirebaseAnalysticController.Instance.LogWatchAd(place);
                            lastSuccessInterstitialTime = Time.time;


                            Debug.Log("REWARD FINISH");
                        }
                        await UniTask.Delay(500, ignoreTimeScale: true);

                        onRewared?.Invoke(result);

                        if (WaitingPanel.Instance != null)
                            WaitingPanel.Instance.Close();

                    });
                }
            }

        }
        public void OnRewardStarted()
        {
            onAdStarted?.Invoke();
        }
        public void OnRewardClosed()
        {
            if (WaitingPanel.Instance != null)
                WaitingPanel.Instance.Close();
            onAdCompleted?.Invoke();
            isBusy = false;
            Debug.Log("REWARD CLOSE");
            if (Game.Controller.Instance.gameConfig.adConfig.openAdAfterRewardAd)
                ShowOpenAd();

        }
        public bool IsInterstitialReady()
        {
            if (!isAd || Game.Controller.Instance.gameConfig.skipAd || Time.time - lastSuccessInterstitialTime < Game.Controller.Instance.gameConfig.adConfig.interAdCoolDown)
            {
                return false;
            }
            return true;
        }
        public void ShowInterstitial(System.Action onClosed = null)
        {
            if (!IsInterstitialReady())
            {
                onClosed?.Invoke();
                return;
            }
            isBusy = true;
            FirebaseAnalysticController.Instance.LogInterstitialStart();

            //// hiện màn hình Loading AD
            WaitingPanel.Create((panel) =>
            {
                ((WaitingPanel)panel).SetUp();
                Show();
            });


            async UniTask Show()
            {
                await UniTask.Delay(500, ignoreTimeScale: true);
                foreach (IAdHandler adHandler in adHandler)
                {
                    adHandler.ShowInterstitial(res =>
                    {
                        if (res)
                        {
                            FirebaseAnalysticController.Instance.LogInterstitialFinish();
                            ShowOpenAd();
                        }

                    }, async () => {
                        try
                        {
                            Time.timeScale = 1;
                            if (WaitingPanel.Instance != null)
                                WaitingPanel.Instance.Close();
                        }
                        catch (System.Exception e) { Debug.LogError(e); }

                        onClosed?.Invoke();
                        await UniTask.Delay(1000, ignoreTimeScale: true);
                        isBusy = false;
                        Debug.Log("INTER FINISH");

                        if (Game.Controller.Instance.gameConfig.adConfig.openAdAfterInterAd)
                            ShowOpenAd();

                    });
                }
            }

        }
        public void LoadInterstitial()
        {
            if (!isAd || Game.Controller.Instance.gameConfig.skipAd) return;
            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.LoadInterstitial();
            }
        }
        public bool IsBannerLoaded()
        {
            foreach (IAdHandler adHandler in adHandler)
            {
                if (adHandler.IsBannerLoaded())
                {
                    return true;
                }
            }
            return false;
        }
        public void ShowBanner()
        {
            if (!isAd || Game.Controller.Instance.gameConfig.skipAd) return;
            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.ShowBanner();
            }
        }
        public void HideBanner()
        {
            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.HideBanner();
            }
        }
        public void LoadBanner()
        {
            if (!isAd) return;
            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.LoadBanner();
            }
        }

        public void LoadOpenAd()
        {
            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.LoadOpenAd();
            }
        }
        public void ShowOpenAd()
        {
            Debug.Log("SHOW OPEN AD " + Time.time + " " + lastShowOpenAd);
            if (!isAd || Game.Controller.Instance.gameConfig.skipAd || isBusy || !IsOpenAdAvailable()) return;

            // hiện màn hình Loading AD
            WaitingPanel.Create((panel) =>
            {
                ((WaitingPanel)panel).SetUp(1);

                Show();
            });


            async UniTask Show()
            {
                await UniTask.Delay(500, ignoreTimeScale: true);
                foreach (IAdHandler adHandler in adHandler)
                {
                    adHandler.ShowOpenAd(async (res) =>
                    {
                        if (res)
                        {
                            lastShowOpenAd = Time.time;
                            FirebaseAnalysticController.Instance.LogOpenAdStart();
                        }
                        await UniTask.Delay(250, ignoreTimeScale: true);
                        if (WaitingPanel.Instance != null)
                            WaitingPanel.Instance.Close();
                    });
                }
            }

        }
        public bool IsOpenAdAvailable()
        {
            foreach (IAdHandler adHandler in adHandler)
            {
                if (adHandler.IsOpenAdAvailable())
                {
                    return true;
                }
            }
            return false;
        }

        public void LoadNativeAd()
        {
            Debug.Log("LOAD NATIVE AD");
            foreach (IAdHandler adHandler in adHandler)
            {
                adHandler.LoadNativeAd();
            }
        }
        public bool IsNativeAdAvailable()
        {
            foreach (IAdHandler adHandler in adHandler)
            {
                if (adHandler.IsNativeAdLoaded())
                {
                    return true;
                }
            }
            return false;
        }

        public object GetNativeAd()
        {
            foreach (IAdHandler adHandler in adHandler)
            {
                if (adHandler.IsNativeAdLoaded())
                {
                    return adHandler.GetCurrentNativeAd();
                }
            }
            return null;
        }

        public void ReloadAllNativeAdBanner()
        {
            Debug.Log("RELOAD NATIVE AD");
            LoadNativeAd();
            onNativeAdRefresh?.Invoke();
        }

        public void RemoveAd()
        {
            isAd = false;
            //HideBanner();
            CollapsibleBanner.HideBanner();
            ReloadAllNativeAdBanner();
        }
        public void ReloadAd()
        {
            isAd = true;
            //HideBanner();
            CollapsibleBanner.ShowBanner();
            ReloadAllNativeAdBanner();
        }

    }
}