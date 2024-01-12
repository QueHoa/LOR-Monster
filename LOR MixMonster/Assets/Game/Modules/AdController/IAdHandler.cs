using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface  IAdHandler  
{
    bool IsReady();
    void Init();
    bool IsBannerLoaded();
    void LoadBanner();
    void ShowBanner();
    void HideBanner();
    void LoadInterstitial();
    void ShowInterstitial(System.Action<bool> onShow, System.Action onClose);
    void ShowRewardedAd(System.Action<bool> onRewared);
    void LoadRewardedAd();
    bool IsRewardAvailable();
    void OnReward();
    void OnShowRewardFailed();

    bool IsOpenAdAvailable();
    void LoadOpenAd();
    void ShowOpenAd(System.Action<bool> onShow);

    void LoadNativeAd();
    bool IsNativeAdLoaded();
    object GetCurrentNativeAd();

}
