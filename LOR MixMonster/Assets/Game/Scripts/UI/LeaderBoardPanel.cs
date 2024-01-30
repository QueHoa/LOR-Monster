using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class LeaderBoardPanel : Panel
{
    bool isProcessing;
    public override void PostInit()
    {
    }
    public void SetUp()
    {
        isProcessing = false;
        Show();
    }
    public void Back()
    {
        if (DataManagement.DataManager.Instance.userData.progressData.playCount >= Game.Controller.Instance.gameConfig.adConfig.adStart)
        {
            AD.Controller.Instance.ShowInterstitial(() =>
            {
                BackHome();

            });
        }
        else
        {
            BackHome();
        }


        void BackHome()
        {
            if (isProcessing) return;
            isProcessing = true;
            LevelLoading.Instance.Active(() =>
            {
                Close();
                UI.PanelManager.Create(typeof(HomePanel), (panel, op) =>
                {
                    ((HomePanel)panel).SetUp();
                    ((StageGameController)Game.Controller.Instance.gameController).ShowCurrentStageMonster();
                });
                LevelLoading.Instance.Close();
            });
        }
    }
}
