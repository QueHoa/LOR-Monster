using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : UI.Panel
{

    bool isProcessing;
    public override void PostInit()
    {
    }
    public void SetUp()
    {
        isProcessing = false;
    }
    public void WatchAds()
    {
        if (isProcessing) return;
        isProcessing = true;
        AD.Controller.Instance.ShowRewardedAd("Upgrade", res =>
        {
            if (res)
            {
                
            }
            else
            {
                isProcessing = false;
            }
        });
    }
}
