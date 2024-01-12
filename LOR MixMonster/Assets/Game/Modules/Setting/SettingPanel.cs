using Cysharp.Threading.Tasks;
using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Purchasing;

public class SettingPanel : UI.Panel,IOnPurchased
{
    public static SettingPanel Instance;
    public override void PostInit()
    {
        Instance = this;
    }
    [SerializeField]
    private GameObject adBtn;
    [SerializeField]
    private SettingButton soundButton, musicButton;
    [SerializeField]
    private TMPro.TextMeshProUGUI versionText;
    public void SetUp()
    {
        //adBtn.SetActive(DataManagement.DataManager.Instance.userData.isAd);

        soundButton.SetUp((isTriggered) =>
        {
            Sound.Controller.SfxEnable = isTriggered;

        }, Sound.Controller.SfxEnable);

        musicButton.SetUp((isTriggered) =>
        {
            Sound.Controller.MusicEnable = isTriggered;
        }, Sound.Controller.MusicEnable);


        versionText.text = string.Format(Localization.LocalizationHandler.GetText("Setting/Version",versionText) , Application.version);
        Show();
        Localization.LocalizationHandler.onLanguageChanged -= OnLanguageChange;
        Localization.LocalizationHandler.onLanguageChanged += OnLanguageChange;
    }
    void OnDisable()
    {
        Localization.LocalizationHandler.onLanguageChanged -= OnLanguageChange;
    }
    void OnLanguageChange(Localization.LanguageKey language)
    {
        versionText.text = string.Format(Localization.LocalizationHandler.GetText("Setting/Version", versionText), Application.version);
    }
    
    public void RestorePurchase()
    {

    }
    public void RemoveAd()
    {

    }
    public void OnPurchaseCompleted(Product product)
    {
        
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
    }
////#if UNITY_EDITOR
//    string level = "0",seed="0",levelPack="A";
//    private void OnGUI()
//    {
//        if (!Game.Controller.Instance.gameConfig.debugMode) return;
//        GUILayout.Space(Screen.height / 2);
//        GUILayout.BeginHorizontal();
//        GUILayout.Space(300);

//        level = GUILayout.TextField(level, GUILayout.Width(200), GUILayout.Height(100));
       
   
//        GUILayout.EndHorizontal();
     

//        if (GUILayout.Button("Add 1000c", GUILayout.Width(200), GUILayout.Height(100)))
//        {
//            DataManagement.DataManager.Instance.userData.inventory.Coin += 10000;
//            DataManagement.DataManager.Instance.Save();
//        }
//        GUILayout.BeginHorizontal();
//        GUILayout.Space(300);
//        if (GUILayout.Button("reward chest", GUILayout.Width(200), GUILayout.Height(100)))
//        {
//            ClaimReward();
//        }
//        GUILayout.EndHorizontal();
//    }

    //#endif
}
