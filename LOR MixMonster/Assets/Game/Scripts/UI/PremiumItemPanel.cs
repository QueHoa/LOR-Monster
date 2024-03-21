using Cysharp.Threading.Tasks;
using ItemData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class PremiumItemPanel : UI.Panel,IOnPurchased
{
    [SerializeField]
    private string productId= "leftrightmonster_premium_bundle";

    private string bundleId;
    System.Action onUnlock;
    public void OnPurchaseCompleted(Product product)
    {
        if (product.definition.id.Contains(productId))
        {
            Unlock();
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        if (product.definition.id.Contains(productId) && reason==PurchaseFailureReason.DuplicateTransaction)
        {
            Unlock();
        }
    }

    public override void PostInit()
    {
    }
    public void SetUp(string bundleId, System.Action onUnlock)
    {
        (Game.Controller.Instance.gameController).hideMonster = true;
        this.onUnlock = onUnlock;
        this.bundleId = bundleId;
        Show();
    }
    async UniTaskVoid Unlock()
    {
        Close();
        var bundleSet=Sheet.SheetDataManager.Instance.gameData.itemData.GetBundle(bundleId);
      
        foreach (ModelSet modelSet in bundleSet.modelSets)
        {
            foreach(string itemId in modelSet.itemIds)
            {
                DataManagement.DataManager.Instance.userData.inventory.SetItemState(itemId, 1);
            }
        }
        DataManagement.DataManager.Instance.userData.inventory.SetItemState(bundleId, 1);
        onUnlock?.Invoke();

        
        List<ItemData.Item> rewardItems = new List<ItemData.Item>();
        foreach( ItemData.ModelSet modelSet in bundleSet.modelSets)
        {
            rewardItems.Clear();
            foreach(string itemId in modelSet.itemIds)
            {
                rewardItems.Add(Sheet.SheetDataManager.Instance.gameData.itemData.GetItem(itemId));
            }
            RewardPanel rewardPanel= (RewardPanel)await UI.PanelManager.CreateAsync(typeof(RewardPanel));
            rewardPanel.SetUp(rewardItems);

            await UniTask.WaitUntil(() => rewardPanel==null);
        }

        //UI.PanelManager.Create(typeof(MessagePanel), (panel, op) =>
        //{
        //    ((MessagePanel)panel).SetUp("Thanks for your purchasing");
        //});
    }
    public override void Close()
    {
        base.Close();
        (Game.Controller.Instance.gameController).hideMonster = false;
    }
}
