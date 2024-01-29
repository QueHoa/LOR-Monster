using CodeStage.AntiCheat.ObscuredTypes;
using System;

namespace GameUtility
{
    public class RewardHandler
    {
        public static void ApplyCash(ObscuredInt cash)
        {
            if (cash == 0) return;
            DataManagement.DataManager.Instance.userData.inventory.Cash += cash;
        }
        public static void ApplyReward(RewardPackage rewardPackage)
        {
           
        }

        public static void ApplyReward(ItemData.Item rewardData)
        {
            //switch (rewardData.category)
            //{                       
            //    case ItemData.Category.Coin:
            //        ApplyReward(rewardData.total);
            //        break;
            //}

            //DataManagement.DataManager.Instance.Save();

        }
    }
}