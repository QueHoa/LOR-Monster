using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardPopup : UI.Panel
{
    public Sprite[] imgGold;
    public Image icon;
    public TextMeshProUGUI textIcon;
    public GameObject day7, bgIcon;
    public GameObject effect1, effect2;
    [SerializeField]
    private AudioClip rewardSFX;
    bool isProcessing = false;
    public override void PostInit()
    {
    }
    public void SetUp(int day)
    {
        isProcessing = false;
        (Game.Controller.Instance.gameController).updateGold = true;
        Sound.Controller.Instance.PlayOneShot(rewardSFX);
        day7.SetActive(false);
        effect1.SetActive(false);
        effect2.SetActive(false);
        textIcon.text = "";
        if(day == 1)
        {
            Day1();
        }
        if (day == 2)
        {
            Day2();
        }
        if (day == 3)
        {
            Day3();
        }
        if (day == 4)
        {
            Day4();
        }
        if (day == 5)
        {
            Day5();
        }
        if (day == 6)
        {
            Day6();
        }
        if (day == 7)
        {
            Day7();
        }
        Show();
    }
    public void Day1()
    {
        icon.sprite = imgGold[0];
        textIcon.text = "x200";
        DataManagement.DataManager.Instance.userData.YourGold += 200;
        DataManagement.DataManager.Instance.Save();
    }
    public void Day2()
    {
        icon.sprite = imgGold[1];
        textIcon.text = "x500";
        DataManagement.DataManager.Instance.userData.YourGold += 500;
        DataManagement.DataManager.Instance.Save();
    }
    public void Day3()
    {
        icon.sprite = imgGold[2];
        textIcon.text = "";
        effect1.SetActive(true);
        var bundle = Sheet.SheetDataManager.Instance.gameData.itemData.GetBundle("SetBundle_4");
        if (DataManagement.DataManager.Instance.userData.inventory.GetItemState(bundle.modelSets[0].itemIds[0]) == 0)
        {
            /*foreach (ItemData.ModelSet modelSet in bundle.modelSets)
            {
                DataManagement.DataManager.Instance.userData.inventory.SetItemState(modelSet.itemIds[0], 1);
            }*/
            ItemData.ModelSet modelSet = bundle.modelSets[0];
            DataManagement.DataManager.Instance.userData.inventory.SetItemState(modelSet.itemIds[0], 1);
        }
    }
    public void Day4()
    {
        icon.sprite = imgGold[3];
        textIcon.text = "x1000";
        DataManagement.DataManager.Instance.userData.YourGold += 1000;
        DataManagement.DataManager.Instance.Save();
    }
    public void Day5()
    {
        icon.sprite = imgGold[4];
        textIcon.text = "";
        effect1.SetActive(true);
        var bundle = Sheet.SheetDataManager.Instance.gameData.itemData.GetBundle("SetBundle_4");
        if (DataManagement.DataManager.Instance.userData.inventory.GetItemState(bundle.modelSets[1].itemIds[0]) == 0)
        {
            /*foreach (ItemData.ModelSet modelSet in bundle.modelSets)
            {
                DataManagement.DataManager.Instance.userData.inventory.SetItemState(modelSet.itemIds[0], 1);
            }*/
            ItemData.ModelSet modelSet = bundle.modelSets[1];
            DataManagement.DataManager.Instance.userData.inventory.SetItemState(modelSet.itemIds[0], 1);
        }
    }
    public void Day6()
    {
        icon.sprite = imgGold[5];
        textIcon.text = "x2000";
        DataManagement.DataManager.Instance.userData.YourGold += 2000;
        DataManagement.DataManager.Instance.Save();
    }
    public void Day7()
    {
        day7.SetActive(true);
        bgIcon.SetActive(false);
        textIcon.text = "";
        effect2.SetActive(true);
        var bundle = Sheet.SheetDataManager.Instance.gameData.itemData.GetBundle("SetBundle_3");
        for (int i = 0; i < 5; i++)
        {
            //item not yet unlocked
            if (DataManagement.DataManager.Instance.userData.inventory.GetItemState(bundle.modelSets[0].itemIds[i]) == 0)
            {
                foreach (ItemData.ModelSet modelSet in bundle.modelSets)
                {
                    DataManagement.DataManager.Instance.userData.inventory.SetItemState(modelSet.itemIds[i], 1);
                }
            }
        }
    }
    public override void Close()
    {
        if (isProcessing) return;
        isProcessing = true;
        base.Close();
    }
}
