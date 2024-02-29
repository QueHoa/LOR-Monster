using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineRewardPanel : UI.Panel
{
    [SerializeField]
    private TMPro.TextMeshProUGUI totalCashText, totalGoldText, totalOfflineTimeText;
    private int totalCash, totalGold;
    [SerializeField]
    private ParticleSystem cashPS;
    [SerializeField]
    private AudioClip rewardSFX;
    private bool isProcessing;
    public override void PostInit()
    {
    }
    public void SetUp(int time, int totalCash, int totalGold)
    {
        Sound.Controller.Instance.PlayOneShot(rewardSFX);
        isProcessing = false;
        totalOfflineTimeText.text = $"{time/3600}H{(time%3600)/60}M{time%60}S";
        this.totalCash = totalCash;
        totalCashText.text = $"{GameUtility.GameUtility.ShortenNumber(totalCash)}";
        this.totalGold = totalGold;
        totalGoldText.text = $"{GameUtility.GameUtility.ShortenNumber(totalGold)}";
        Show();
        cashPS.Play();
    }
    public void Claim(bool ads)
    {
        if (isProcessing) return;

        if (ads)
        {
            AD.Controller.Instance.ShowRewardedAd("ClaimGold", res =>
            {
                if (res)
                {
                    ClaimReward(2);
                }
                else
                {
                    isProcessing = false;
                }
            });
        }
        else
        {
            ClaimReward(1);
        }
        void ClaimReward(int x)
        {
            DataManagement.DataManager.Instance.userData.stageListData.lastEarningDate = System.DateTime.Now.Ticks;

            GameUtility.RewardHandler.ApplyCash(totalCash * x);
            Debug.Log("TOTAL OFFLINE EARNING " + totalCash * x);
            DataManagement.DataManager.Instance.userData.YourGold += totalGold * x;
            ((StageGameController)Game.Controller.Instance.gameController).homePanel.goldText.text = DataManagement.DataManager.Instance.userData.YourGold.ToString();
            DataManagement.DataManager.Instance.Save();
            Close();
        }
    }
    public override void Close()
    {
        base.Close();
        (Game.Controller.Instance.gameController).hideMonster = false;
    }
}
