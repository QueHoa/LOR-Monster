using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Drawing;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ItemData;
using static DataManagement.MergeSlotData;

/*
 * This is just a snippet of code to integrate Daily Rewards into your project
 * Copy / Paste the code below
 */
namespace DailyReward
{
    public class IntegrationDailyRewards : MonoBehaviour
    {
        public DailyRewardsScreen dailyRewardsScreen;
        // public Transform spawnCoinHolder;
        // public CanvasGroup coinCanvasGroup;
        // public Transform coinHole;
        // public Image backpack;
        public ParticleSystem particle;
        private void Awake() => DailyRewardsHandler.OnClaimPrize += ClaimDailyRewards;

        private void OnDestroy() => DailyRewardsHandler.OnClaimPrize -= ClaimDailyRewards;


        private void TemplateCodeClaimReward(int day)
        {
          //var reward =   DailyRewardsHandler.Instance.GetReward(day);


          //  PlayerPrefs.SetInt("MY_REWARD_KEY", rewardsCount);
          //  PlayerPrefs.Save();
        }

        private async void ClaimDailyRewards(int day)
        {
            // This returns a Reward object
            var myReward = DailyRewardsHandler.Instance.GetReward(day);
            Debug.Log("Collect coin in day " + day);
            //CoinManager.AddCoin(GetValue(day));
            //particle.Play();
        }

        public int GetValue(int index)
        {
            if (index <= 3) return 300;
            if (index <= 6) return 500;
            if (index == 7) return 1000;
            return 0;
        }
    }
}