using DataManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorPanel : UI.Panel
{
    GameUtility.Pooling.PoolHandler pool;
    [SerializeField]
    int pageIndex = 0;
    int totalPage;
    public StageData currentStageData;
    public override void PostInit()
    {
        totalPage = Sheet.SheetDataManager.Instance.gameData.itemData.stageItemSets.Count;
        pool = GetComponentInChildren<GameUtility.Pooling.PoolHandler>();
    }
    public void SetUp()
    {
        currentStageData = ((StageGameController)Game.Controller.Instance.gameController).GetStageHandler().stageData;
        StageItemButton.onStageItemSelected += OnButtonSelected;
        SetCategory(pageIndex);
        Show();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        StageItemButton.onStageItemSelected -= OnButtonSelected;

    }
    void OnButtonSelected(ItemData.StageItem stageItem)
    {
        SetCategory(pageIndex);
    }
    public void SetPage(int dir)
    {
        pageIndex += dir;
        if (pageIndex < 1) pageIndex = totalPage - 1;
        if (pageIndex >= totalPage) pageIndex = 1;

        SetCategory(pageIndex);
    }
    public List<ItemData.StageItem> test = new List<ItemData.StageItem>();
    void SetCategory(int set)
    {
        pool.Clear();
        test.Clear();
        foreach (var item in Sheet.SheetDataManager.Instance.gameData.itemData.GetStageSet(set).items)
        {
            StageItemButton button = pool.Get().GetComponent<StageItemButton>();
            test.Add(item);
            button.SetUp(item, currentStageData.index);
        }
    }
}
