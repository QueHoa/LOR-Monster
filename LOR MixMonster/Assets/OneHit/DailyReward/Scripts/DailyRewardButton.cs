using UnityEngine;

namespace DailyReward
{
    public class DailyRewardButton : MonoBehaviour
    {
        public GameObject notificationBadge;

        private DailyRewardsHandler _dailyRewardHandler;

        private void Start()
        {
            _dailyRewardHandler = DailyRewardsHandler.Instance;
        }

        private void FixedUpdate()
        {
            // availableReward is currentDay has unclaim reward
            // availableReward = 0 means not available reward
            notificationBadge!.SetActive(_dailyRewardHandler.availableReward != 0);
        }

        public void OnClickButton()
        {
            // You can find some thing in UIHomeScreen
        }
    }
}