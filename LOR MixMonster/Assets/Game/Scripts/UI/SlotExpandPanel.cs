using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotExpandPanel : UI.Panel
{
    public delegate void OnSlotExpanded();
    public static OnSlotExpanded onSlotExpanded;
    DataManagement.StageData stageData;
    [SerializeField]
    private TMPro.TextMeshProUGUI slotPresent, slotUpgrade;
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
        if (stageData.totalMonsterSlot < 22)
        {
            normalObj.SetActive(true);
            lockObj.SetActive(false);
            
            for (int i = 0; i < Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs.Count; i++)
            {
                if (Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[i].maxSlot > stageData.totalMonsterSlot)
                {
                    nextSlotLevel = i;
                    break;
                }
            }
            slotPresent.text = stageData.totalMonsterSlot.ToString();
            slotUpgrade.text = Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[nextSlotLevel].maxSlot.ToString();

        }
        else
        {
            normalObj.SetActive(false);
            lockObj.SetActive(true);
        }

        Show();
    }
    public void WatchAd()
    {
        AD.Controller.Instance.ShowRewardedAd("ExpandStage", res =>
        {
            if (res)
            {
                Unlock();
                base.Close();
            }
        });
    }

    void Unlock()
    {
        Sound.Controller.Instance.PlayOneShot(unlockSFX);

        int maxSlot = 22;
        int totalNewSlot = Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[nextSlotLevel].maxSlot;

        stageData.totalMonsterSlot = Mathf.Min(totalNewSlot, maxSlot);
        DataManagement.DataManager.Instance.Save();
        onSlotExpanded?.Invoke();
        onResult?.Invoke(true);
    }
    public override void Close()
    {
        base.Close();
        ((StageGameController)Game.Controller.Instance.gameController).hideMonster = false;
        onResult?.Invoke(false);
    }
}
