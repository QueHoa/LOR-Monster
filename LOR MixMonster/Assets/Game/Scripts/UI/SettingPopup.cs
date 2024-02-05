using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SettingPopup : UI.Panel
{
    bool isProcessing = false;
    public override void PostInit()
    {
    }
    public void SetUp()
    {
        isProcessing = false;
        Show();
    }
    public void BackHome()
    {
        if (DataManagement.DataManager.Instance.userData.progressData.playCount >= Game.Controller.Instance.gameConfig.adConfig.adStart)
        {
            AD.Controller.Instance.ShowInterstitial(() =>
            {
                Back();

            });
        }
        else
        {
            Back();
        }

        void Back()
        {
            if (isProcessing) return;
            isProcessing = true;
            (Game.Controller.Instance.gameController).hideMonster = false;
            Close();
        }

    }
}
