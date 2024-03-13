using GameUtility.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageItemButton : PoolComponent
{
    public delegate void OnStageItemSelected(ItemData.StageItem item);
    public static OnStageItemSelected onStageItemSelected;

    public delegate void OnStageItemPreview(ItemData.StageItem item);
    public static OnStageItemPreview onStageItemPreview;

    [SerializeField]
    private Image iconImg, iconBg, bg;
    [SerializeField]
    private Sprite disableIconBg, disableBg;
    [SerializeField]
    private Button preview;

    [SerializeField]
    private TMPro.TextMeshProUGUI bonusText, cashText, adText, titleText;
    [SerializeField]
    private GameObject purchaseBtn, adBtn, usingObj, orObj, iconUsing;
    public int state;
    public ItemData.StageItem stageItem;
    int stageIndex = 0;
    public void SetUp(ItemData.StageItem stageItem, int stageIndex)
    {
        this.stageIndex = stageIndex;
        this.stageItem = stageItem;
        stageItem.GetIcon(sprite => {
            iconImg.sprite = sprite;
            Debug.Log("SPRITE:" + iconImg.sprite.name);
        });

        titleText.text = stageItem.title;
        state = DataManagement.DataManager.Instance.userData.inventory.GetItemState($"{stageIndex}_{stageItem.id}");
        usingObj.SetActive(state == 1);
        usingObj.SetActive(state == 2);
        iconUsing.SetActive(state == 1);
        iconUsing.SetActive(state == 2);
        if (state == 2)
        {
            iconBg.sprite = disableIconBg;
            bg.sprite = disableBg;
            preview.interactable = false;
        }
        adBtn.SetActive(state == 0);
        purchaseBtn.SetActive(state == 0);
        orObj.SetActive(state == 0);
        bonusText.text = $"+{stageItem.bonusEarning}%";

        if (state == 0)
        {
            adText.text = $"{DataManagement.DataManager.Instance.userData.progressData.GetAdProgress($"{stageIndex}_{stageItem.id}")}/{stageItem.adRequire}";
            cashText.text = $"${GameUtility.GameUtility.ShortenNumber(stageItem.cashRequire)}";
        }



        gameObject.SetActive(true);
    }
    public void Equip()
    {
        onStageItemSelected?.Invoke(stageItem);
    }
    public void Purchase()
    {
        if (GameUtility.PurchaseHandler.Purchase(stageItem.cashRequire))
        {
            Unlock();
        }
        else
        {
            UI.PanelManager.Create(typeof(MessagePanel), (panel, op) =>
            {
                ((MessagePanel)panel).SetUp("Not enough cash");
            });
        }
    }
    public void WatchAd()
    {
        AD.Controller.Instance.ShowRewardedAd("stage item", res =>
        {
            if (res)
            {
                int adCount = DataManagement.DataManager.Instance.userData.progressData.GetAdProgress($"{stageIndex}_{stageItem.id}") + 1;
                Debug.Log("ADCOUNT: " + adCount);
                if (adCount >= stageItem.adRequire)
                {
                    Unlock();
                }
                else
                {
                    adText.text = $"{adCount}/{stageItem.adRequire}";
                }
                DataManagement.DataManager.Instance.userData.progressData.SetAdProgress($"{stageIndex}_{stageItem.id}", adCount);
                DataManagement.DataManager.Instance.Save();
            }
            else
            {
#if UNITY_EDITOR
                int adCount = DataManagement.DataManager.Instance.userData.progressData.GetAdProgress($"{stageIndex}_{stageItem.id}") + 1;
                Debug.Log("ADCOUNT: " + adCount);
                if (adCount >= stageItem.adRequire)
                {
                    Unlock();
                }
                else
                {
                    adText.text = $"{adCount}/{stageItem.adRequire}";
                }
                DataManagement.DataManager.Instance.userData.progressData.SetAdProgress($"{stageIndex}_{stageItem.id}", adCount);
                DataManagement.DataManager.Instance.Save();
#endif
            }
        });
    }
    void Unlock()
    {
        ((StageGameController)Game.Controller.Instance.gameController).RestoreStageView();
        Equip();
    }
    public void Preview()
    {
        ((StageGameController)Game.Controller.Instance.gameController).RestoreStageView();
        onStageItemPreview?.Invoke(stageItem);
    }
}
