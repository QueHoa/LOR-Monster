using DataManagement;
using ItemData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorPanel : UI.Panel
{
    GameUtility.Pooling.PoolHandler pool;
    public StageData currentStageData;
    public override void PostInit()
    {
        pool = GetComponentInChildren<GameUtility.Pooling.PoolHandler>();
    }
    public void SetUp()
    {
        currentStageData = ((StageGameController)Game.Controller.Instance.gameController).GetStageHandler().stageData;
        StageItemButton.onStageItemSelected += OnButtonSelected;
        SetCategory();
        Camera.main.orthographicSize = 20;
        CameraController.Instance.LerpOffset(new Vector3(0, -10, -10));
        Show();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        StageItemButton.onStageItemSelected -= OnButtonSelected;

    }
    void OnButtonSelected(ItemData.StageItem stageItem)
    {
        SetCategory();
    }
    public List<ItemData.StageItem> test = new List<ItemData.StageItem>();
    void SetCategory()
    {
        pool.Clear();
        test.Clear();
        foreach (var item in Sheet.SheetDataManager.Instance.gameData.itemData.GetStageSet(0).items)
        {
            StageItemButton button = pool.Get().GetComponent<StageItemButton>();
            test.Add(item);
            button.SetUp(item, currentStageData.index);
            if(button.state != 2)
            {
                button.transform.SetAsFirstSibling();
            }
        }
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
            CameraController.Instance.LerpOffset(new Vector3(0, 0, -10));
            Close();
            UI.PanelManager.Create(typeof(HomePanel), (panel, op) =>
            {
                ((HomePanel)panel).SetUp();
                ((StageGameController)Game.Controller.Instance.gameController).homePanel = panel as HomePanel;
                ((StageGameController)Game.Controller.Instance.gameController).ShowCurrentStageMonster();
                ((StageGameController)Game.Controller.Instance.gameController).RestoreStageView();
                Camera.main.orthographicSize = 20;
            });
        }
    }
}
