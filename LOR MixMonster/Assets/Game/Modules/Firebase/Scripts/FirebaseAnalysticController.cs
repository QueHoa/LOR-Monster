using com.adjust.sdk;
using Cysharp.Threading.Tasks;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using IAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;

public class FirebaseAnalysticController : UnityEngine.MonoBehaviour
{
    private const string ADCOUNTEVENT = "ads_reward";
    private const string INAPPPURCHASECOUNTEVENT = "item_purchase";
    private const string VALUE = "value";
    private const string SOURCE = "source";
    private const string STAGE = "stage";
    private const string SUBSTAGE = "sublevel";
    private const string CHAPTER = "chapter";
    private const string EPISODE = "episode";
    private const string STAGE_START = "stage_start";
    private const string BUTTON_CLICK = "button_click";
    private const string CAMPAIGN_QUEST_COMPLETE = "campaign_quest_complete";
    private const string HUNTERCLAN_QUEST_COMPLETE = "hunterclan_quest_complete";
    private const string HUNTERCLAN_CHALLENGE_COMPLETE = "hunterclan_challenge_complete";
    private const string BATTLEPASS_QUEST_COMPLETE = "battlepass_quest_complete";
    private const string AD_LOCATION = "ads_location";
    private const string AD_IMPRESSION = "ads_impression";
    private const string BUTTON_NAME = "button_name";
    private const string BUTTON_ID = "button_id";
    private const string EARN_RESOURCE = "earn_resource";
    private const string SPEND_RESOURCE = "spend_resource";
    private const string ITEM_ID = "item_id";
    private const string CURRENT_DUNGEON = "currentChapter";
    private const string CURRENT_STAGE = "currentStage";



    private const string PLAYER_ID = "playerId";
    private const string CURRENT_LEVEL = "currentLevel";
    private const string CURRENT_RANK = "currentRank";
    private const string IAP_COUNT = "totalIapCount";
    private const string AD_COUNT = "totalAdCount";
    private const string ONLINE_TIME = "totalOnlineTime";
    private const string COIN_ACHIEVE = "totalCoinAchieve";
    private const string COIN_SPENT = "totalCoinSpend";
    private const string GEM_ACHIEVE = "totalGemAchieve";
    private const string GEM_SPENT = "totalGemSpend";


    private const string LEVEL_START = "LEVEL_START";
    private const string LEVEL_END = "LEVEL_END";
    private const string LEVEL_RESULT = "LEVEL_RESULT";
    public static FirebaseAnalysticController Instance;

    private bool isReady;

    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            FirebaseManager.onInit -= Init;
            FirebaseManager.onInit += Init;
            if (FirebaseManager.Instance != null && FirebaseManager.Instance.isReady)
            {
                Init();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Init()
    {
        isReady = true;
        Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        IAP.Controller.onPurchased -= OnPurchased;
        IAP.Controller.onPurchased += OnPurchased;
    }

    public void OnPurchased(Product product)
    {
        //string myId = DataManager.Instance.userSaveData.adId;
        //bool check = false;
        //for (int i = 0; i < RemoteConfigHandler.Instance.testDevices.Length; i++)
        //{
        //    if (myId.Equals(RemoteConfigHandler.Instance.testDevices[i]))
        //    {
        //        check = true;
        //        break;
        //    }
        //}
        //if (!check)
        Debug.Log("Purchased success, fire event!".Color("lime"));
        LogIAP(product, IAP.Controller.Instance.productPrices[product.definition.id]);
    }

    public void SetProperties()
    {
    }



    public void LogEvent(string eventName)
    {
        if (isReady)
        {

            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
        }
    }

    public void LogButtonClick(string name, int id)
    {
        LogEvent(BUTTON_CLICK, new Parameter(BUTTON_NAME, name), new Parameter(BUTTON_ID, id));
    }

    public void LogWatchAd(string place)
    {
        LogEvent("ADS_REWARD_DONE_" + place);
    }
    public void LogStartAd(string place)
    {
        LogEvent("ADS_REWARD_START_" + place);
    }
    public void LogOpenAdStart()
    {
        LogEvent("open_ad");
    }
    public void LogInterstitialStart()
    {
        LogEvent("institial_ad");
    }
    public void LogInterstitialFinish()
    {
        LogEvent("institial_ad_finish");
    }

    public void LogIAP(Product product, float price)
    {
        LogEvent(INAPPPURCHASECOUNTEVENT, new Parameter("product_id", product.definition.id), new Parameter("transaction_id", product.transactionID), new Parameter("revenue", price));
        Debug.Log($"{INAPPPURCHASECOUNTEVENT} : id={product.definition.id} price={price}".Color("lime"));
    }
    public void LogRevenue(int amount)
    {
        if (!isReady) return;
        try
        {
            FirebaseAnalytics.LogEvent("revenue", new Parameter("value", amount));
        }
        catch (System.Exception e)
        {
            GameUtility.GameUtility.LogError(e);
        }
    }

    public void LogUpgradeTrain()
    {
        if (!isReady) return;
        try
        {
            FirebaseAnalytics.LogEvent("UPGRADE_UPGRADE_TRAIN");
        }
        catch (System.Exception e)
        {
            GameUtility.GameUtility.LogError(e);
        }
    }
    public void LogBuyGun()
    {
        if (!isReady) return;
        try
        {
            FirebaseAnalytics.LogEvent("UPGRADE_BUY_GUN");
        }
        catch (System.Exception e)
        {
            GameUtility.GameUtility.LogError(e);
        }
    }

    public void LogEnterFreeReward()
    {
        if (!isReady) return;
        try
        {
            FirebaseAnalytics.LogEvent($"FIRST_FREE");
        }
        catch (System.Exception e)
        {
            GameUtility.GameUtility.LogError(e);
        }
    }
    public void LogEnterCollection()
    {
        if (!isReady) return;
        try
        {
            FirebaseAnalytics.LogEvent($"FIRST_COLLECT");
        }
        catch (System.Exception e)
        {
            GameUtility.GameUtility.LogError(e);
        }
    }
    StringBuilder builder = new StringBuilder();

    public void LogEvent(string eventName, params Parameter[] parameters)
    {
        if (!isReady) return;
        try
        {
#if UNITY_EDITOR
            builder.Clear();
            builder.Append("Log event: ").Append(eventName);
            GameUtility.GameUtility.Log(builder);
#endif
            FirebaseAnalytics.LogEvent(eventName, parameters);
        }
        catch (System.Exception e)
        {
            GameUtility.GameUtility.LogError(e);
        }
    }
    public void LogEvent(string eventName, string parameterKey, string parameterValue)
    {
        if (!isReady) return;
        try
        {
#if UNITY_EDITOR
            builder.Clear();
            builder.Append("Log event:" + eventName);
            builder.AppendLine(parameterKey + " " + parameterValue);
            GameUtility.GameUtility.Log(builder);
#endif
            FirebaseAnalytics.LogEvent(eventName, parameterKey, parameterValue);
        }
        catch (System.Exception e)
        {
            GameUtility.GameUtility.LogError(e);
        }
    }
    //

    public void SendRevenueToFirebase(GoogleMobileAds.Api.AdValue adValue)
    {
        double revenue = adValue.Value;
        var impressionParameters = new[]
        {
                new Parameter("ad_platform", "ADMOD"),
                new Parameter("ad_source", "admod"),
                new Parameter("value", (revenue / 1000000f)),
                new Parameter("currency", "USD"), // All Applovin revenue is sent in USD
            };
        FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        FirebaseAnalytics.LogEvent("ad_revenue_sdk", impressionParameters);
        FirebaseAnalytics.LogEvent("cc_openad_native_revenue", impressionParameters);
        GameUtility.GameUtility.Log($"EVENT  admob open ad ad_impression {revenue}");
    }
    public void SendRevenueToFireBase(IronSourceImpressionData adInfo)
    {
        double revenue = (double)adInfo.revenue;
        if (adInfo != null)
        {
            Parameter[] AdParameters = {
            new Parameter("ad_platform", "ironSource"),
            new Parameter("ad_source", adInfo.adNetwork),
            new Parameter("ad_unit_name", adInfo.adUnit),
            new Parameter("ad_format", adInfo.instanceName),
            new Parameter("currency","USD"),
            new Parameter("value", revenue)
        };
            FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
            FirebaseAnalytics.LogEvent("ad_revenue_sdk", AdParameters);
            GameUtility.GameUtility.Log($"EVENT IronSource ad_impression {revenue}");
        }
    }
    public void SendRevenueToAdjust(AdValue adValue)
    {
        AdjustAdRevenue adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
        adRevenue.setRevenue(adValue.Value / 1000000f, adValue.CurrencyCode);
        GameUtility.GameUtility.LogError("=> Adjust Value : " + (adValue.Value));
        GameUtility.GameUtility.LogError("=> Adjust 1000000 : " + (adValue.Value / 1000000f));
        Adjust.trackAdRevenue(adRevenue);
        GameUtility.GameUtility.Log($"EVENT admob open ad Adjust {adValue.Value}");

    }
    public void SendRevenueToAdjust(IronSourceImpressionData impressionData)
    {
        double revenue = (double)impressionData.revenue;
        AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceIronSource);
        adjustAdRevenue.setRevenue(revenue, "USD");
        // optional fields
        adjustAdRevenue.setAdRevenueNetwork(impressionData.adNetwork);
        adjustAdRevenue.setAdRevenueUnit(impressionData.adUnit);
        adjustAdRevenue.setAdRevenuePlacement(impressionData.placement);
        // track Adjust ad revenue
        Adjust.trackAdRevenue(adjustAdRevenue);
        GameUtility.GameUtility.Log($"EVENT IronSource Adjust {revenue}");
    }



}

