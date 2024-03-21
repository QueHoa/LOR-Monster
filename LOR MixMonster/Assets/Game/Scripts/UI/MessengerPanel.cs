using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class MessengerPanel : UI.Panel
{
    [SerializeField]
    private TextMeshProUGUI title;
    bool isProcessing = false;
    public override void PostInit()
    {
    }
    public void SetUp(int time)
    {
        isProcessing = false;
        title.text = "interstitial ads will not show in " + time.ToString() + " minutes!";
        DataManagement.DataManager.Instance.userData.stageListData.noAdsTime = 60 * time;
        DataManagement.DataManager.Instance.userData.stageListData.isNoAds = true;
        DataManagement.DataManager.Instance.userData.progressData.timeNoAds = System.DateTime.Now.Ticks;
        DataManagement.DataManager.Instance.Save();
        Show();
    }
    public void BackHome()
    {
        if (isProcessing) return;
        isProcessing = true;
        (Game.Controller.Instance.gameController).hideMonster = false;
        Close();
    }
}
