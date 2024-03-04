using Cysharp.Threading.Tasks;
using ItemData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class SetBundlePanel : UI.Panel,IOnPurchased
{
    [SerializeField]
    private TMPro.TextMeshProUGUI adCountText;
    [SerializeField]
    private string productId= "leftrightamanda_bundleset";

    private string bundleId;
    System.Action onUnlock;
    [SerializeField]
    private AudioClip openSFX;
    public void OnPurchaseCompleted(Product product)
    {
        if (product.definition.id.Contains(productId))
        {
            Debug.LogError("Removeads");
            RemoveAd();
            Unlock();
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        if (product.definition.id.Contains(productId) && reason==PurchaseFailureReason.DuplicateTransaction)
        {
            Debug.LogError("NoRemoveads");
            RemoveAd();
            Unlock();
        }
    }

    public override void PostInit()
    {
    }
    public void SetUp(string bundleId, System.Action onUnlock)
    {
        Sound.Controller.Instance.PlayOneShot(openSFX);
        this.onUnlock = onUnlock;
        this.bundleId = bundleId;
        adCountText.text = $"{DataManagement.DataManager.Instance.userData.progressData.GetAdProgress("SetBundle_"+bundleId)}/{Game.Controller.Instance.gameConfig.bundleAdRequire} AD";
        Show();
    }
    public void WatchAd()
    {
        AD.Controller.Instance.ShowRewardedAd("SetBundle", res =>
        {
            if (res)
            {
                FirebaseAnalysticController.Instance.LogEvent($"ADS_REWARD_START_SETBUNDLE");
                int adCount = DataManagement.DataManager.Instance.userData.progressData.GetAdProgress("SetBundle_" + bundleId);
                DataManagement.DataManager.Instance.userData.progressData.SetAdProgress("SetBundle_" + bundleId, adCount + 1);
                DataManagement.DataManager.Instance.Save();

                adCountText.text = $"{adCount + 1}/{Game.Controller.Instance.gameConfig.bundleAdRequire} AD";

                //unlock set
                if (adCount + 1 >= Game.Controller.Instance.gameConfig.bundleAdRequire)
                {
                    Unlock();
                }

            }
            else
            {
#if UNITY_EDITOR
                FirebaseAnalysticController.Instance.LogEvent($"ADS_REWARD_START_SETBUNDLE");
                int adCount = DataManagement.DataManager.Instance.userData.progressData.GetAdProgress("SetBundle_" + bundleId);
                DataManagement.DataManager.Instance.userData.progressData.SetAdProgress("SetBundle_" + bundleId, adCount + 1);
                DataManagement.DataManager.Instance.Save();

                adCountText.text = $"{adCount + 1}/{Game.Controller.Instance.gameConfig.bundleAdRequire} AD";

                //unlock set
                if (adCount + 1 >= Game.Controller.Instance.gameConfig.bundleAdRequire)
                {
                    Unlock();
                }
#endif
            }
        });
    }
    void RemoveAd()
    {
        DataManagement.DataManager.Instance.userData.IsAd = false;
        DataManagement.DataManager.Instance.Save();
        AD.Controller.Instance.RemoveAd();
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
