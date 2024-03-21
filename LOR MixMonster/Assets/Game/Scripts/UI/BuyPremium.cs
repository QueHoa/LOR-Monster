using Cysharp.Threading.Tasks;
using DG.Tweening;
using IAP;
using ItemData;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;
using UnityEngine.Purchasing;
using static DataManagement.MergeSlotData;

public class BuyPremium : MonoBehaviour,IOnPurchased
{
    [SerializeField]
    private string productId = "leftrightamanda_bundleset";
    [SerializeField]
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
        if (product.definition.id.Contains(productId) && reason == PurchaseFailureReason.DuplicateTransaction)
        {
            Unlock();
        }
    }
    async UniTaskVoid Unlock()
    {
        var bundleSet = Sheet.SheetDataManager.Instance.gameData.itemData.GetBundle(bundleId);

        foreach (ModelSet modelSet in bundleSet.modelSets)
        {
            foreach (string itemId in modelSet.itemIds)
            {
                DataManagement.DataManager.Instance.userData.inventory.SetItemState(itemId, 1);
            }
        }
        DataManagement.DataManager.Instance.userData.inventory.SetItemState(bundleId, 1);
        onUnlock?.Invoke();


        List<ItemData.Item> rewardItems = new List<ItemData.Item>();
        foreach (ItemData.ModelSet modelSet in bundleSet.modelSets)
        {
            rewardItems.Clear();
            foreach (string itemId in modelSet.itemIds)
            {
                rewardItems.Add(Sheet.SheetDataManager.Instance.gameData.itemData.GetItem(itemId));
            }
            RewardPanel rewardPanel = (RewardPanel)await UI.PanelManager.CreateAsync(typeof(RewardPanel));
            rewardPanel.SetUp(rewardItems);

            await UniTask.WaitUntil(() => rewardPanel == null);
        }
        gameObject.SetActive(false);
    }
}
