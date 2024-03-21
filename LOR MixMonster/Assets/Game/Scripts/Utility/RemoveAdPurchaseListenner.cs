using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class RemoveAdPurchaseListenner : MonoBehaviour,IOnPurchased
{
    [SerializeField]
    private string id;
    public void OnPurchaseCompleted(Product product)
    {
        if (product.definition.id.Equals(id))
        {
            OnPurchased();

        }
    }

    private static void OnPurchased()
    {
        DataManagement.DataManager.Instance.userData.IsAd = false;
        DataManagement.DataManager.Instance.userData.stageListData.isNoAds = false;
        DataManagement.DataManager.Instance.Save();
        AD.Controller.Instance.RemoveAd();


        UI.PanelManager.Create(typeof(MessagePanel), (panel, op) =>
         {
             ((MessagePanel)panel).SetUp("Thanks for your purchasing. All forced ads will not show");
             FirebaseAnalytics.LogEvent("in_app_purchase");
         });
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        if (product.definition.id.Equals(id) && reason.Equals(PurchaseFailureReason.DuplicateTransaction))
        {
            OnPurchased();
        }
        else
        {
            UI.PanelManager.Create(typeof(MessagePanel), (panel, op) =>
            {
                ((MessagePanel)panel).SetUp("Purchase failed\nReason: "+reason);
            });
        }
    }

}
