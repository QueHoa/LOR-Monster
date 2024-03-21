using Cysharp.Threading.Tasks;
using DG.Tweening;
using ItemData;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using static DataManagement.MergeSlotData;

public class BuyCoin : MonoBehaviour,IOnPurchased
{
    public ShopPanel shopPanel;
    [SerializeField]
    private int coin;
    [SerializeField]
    private string productId = "leftrightamanda_bundleset";
    
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
    public void WatchAd()
    {
        AD.Controller.Instance.ShowRewardedAd("GetGold", res =>
        {
            if (res)
            {
                shopPanel.ChangeGold(coin);
                Unlock();
            }
            else
            {
#if UNITY_EDITOR
                shopPanel.ChangeGold(coin);
                Unlock();
#endif
            }
        });
    }
    async UniTaskVoid Unlock()
    {
        shopPanel.ChangeGold(coin);
    }
}
