using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotExpandPanel : UI.Panel
{
    public delegate void OnSlotExpanded();
    public static OnSlotExpanded onSlotExpanded;
    DataManagement.StageData stageData;
    [SerializeField]
    private TMPro.TextMeshProUGUI expandPriceText, adCountText, totalNewSlotText1, totalNewSlotText2;
    [SerializeField]
    private AudioClip unlockSFX;

    System.Action<bool> onResult;

    [SerializeField]
    private GameObject lockObj, normalObj;

    int nextSlotLevel = 0;

    public override void PostInit()
    {
    }
    public void SetUp(DataManagement.StageData stageData, System.Action<bool> onResult)
    {
        this.stageData = stageData;
        this.onResult = onResult;
        if (stageData.totalModelSlot < 22 /*Sheet.SheetDataManager.Instance.gameData.stageConfig.stageConfigs[stageData.index].maxExpandSlot*/)
        {
            normalObj.SetActive(true);
            lockObj.SetActive(false);
            expandPriceText.text = $"${GameUtility.GameUtility.ShortenNumber(Sheet.SheetDataManager.Instance.gameData.stageConfig.expandPrice)}";

            for (int i = 0; i < Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs.Count; i++)
            {
                if (Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[i].maxSlot > stageData.totalModelSlot)
                {
                    nextSlotLevel = i;
                    break;
                }
            }

            expandPriceText.text = $"${GameUtility.GameUtility.ShortenNumber(Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[nextSlotLevel].cashRequire)}";
            adCountText.text = $"{DataManagement.DataManager.Instance.userData.progressData.GetAdProgress($"Slot_{stageData.index}_{nextSlotLevel}")}/{Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[nextSlotLevel].adRequire}";

            int maxSlot = 22; //Sheet.SheetDataManager.Instance.gameData.stageConfig.stageConfigs[stageData.index].maxExpandSlot;
            int totalNewSlot = Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[nextSlotLevel].maxSlot;


            totalNewSlotText1.text = totalNewSlotText2.text = $"+{Mathf.Min(totalNewSlot, maxSlot) - stageData.totalModelSlot}";
        }
        else
        {
            normalObj.SetActive(false);
            lockObj.SetActive(true);
        }

        Show();
    }

    public void Purchase()
    {
        int price = Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[nextSlotLevel].cashRequire;

        /*if (GameUtility.PurchaseHandler.Purchase(price))
        {
            Unlock();
            base.Close();
        }
        else
        {
            UI.PanelManager.Create(typeof(MessagePanel), (panel, op) =>
            {
                ((MessagePanel)panel).SetUp("Not enough cash");
            });
        }*/
    }
    public void WatchAd()
    {
        AD.Controller.Instance.ShowRewardedAd("ExpandStage", res =>
        {
            if (res)
            {
                int adCount = DataManagement.DataManager.Instance.userData.progressData.GetAdProgress($"Slot_{stageData.index}_{nextSlotLevel}");
                adCount++;
                if (adCount >= Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[nextSlotLevel].adRequire)
                {
                    Unlock();
                    base.Close();
                }
                adCountText.text = $"{adCount}/{Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[nextSlotLevel].adRequire}";


                DataManagement.DataManager.Instance.userData.progressData.SetAdProgress($"Slot_{stageData.index}_{nextSlotLevel}", adCount);
            }
        });
    }

    void Unlock()
    {
        Sound.Controller.Instance.PlayOneShot(unlockSFX);

        int maxSlot = 22; //Sheet.SheetDataManager.Instance.gameData.stageConfig.stageConfigs[stageData.index].maxExpandSlot;
        int totalNewSlot = Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[nextSlotLevel].maxSlot;

        stageData.totalModelSlot = Mathf.Min(totalNewSlot, maxSlot);
        DataManagement.DataManager.Instance.Save();
        onSlotExpanded?.Invoke();
        onResult?.Invoke(true);
    }
    public override void Close()
    {
        base.Close();
        onResult?.Invoke(false);
    }
}
