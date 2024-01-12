using System;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;

namespace DailyReward
{
    public class DailyRewardsHandler : DailyRewardsCore<DailyRewardsHandler>
    {
        public List<Reward> rewards; // Rewards list 
        public DateTime LastRewardTime; // The last time the user clicked in a reward
        public int availableReward; // The available reward position the player claim
        public int lastReward; // the last reward the player claimed
        public bool keepOpen = true; // Keep open even when there are no Rewards available?

        // events
        public static Action OnInitialize;
        public static Action<int> OnClaimPrize;

        // Needed Constants
        private const string LastRewardTimeKey = "LastRewardTime";
        private const string LastRewardKey = "LastReward";
        private const string DebugTimeKey = "DebugTime";
        private const string FMT = "O";

        public TimeSpan DebugTime; // For debug purposes only

        private void Start()
        {
            InitializeDate(); // set Now = DateTime.Now
            LoadDebugTime();
            CheckRewards();
            // UpdateUI();

            OnInitialize?.Invoke();
        }

        protected override void OnApplicationPause(bool pauseStatus)
        {
            base.OnApplicationPause(pauseStatus);
            CheckRewards();
        }

        public TimeSpan GetTimeDifference()
        {
            // var difference = (LastRewardTime - Now);
            // difference = difference.Subtract(DebugTime);
            // return difference.Add(new TimeSpan(0, 24, 0, 0));
            return Now.Date.AddDays(1) - Now;
        }

        private void LoadDebugTime()
        {
            var debugHours = PlayerPrefs.GetInt(GetDebugTimeKey(), 0);
            DebugTime = new TimeSpan(debugHours, 0, 0);
        }

        // Check if the player have unclaimed prizes
        public void CheckRewards()
        {
            var lastClaimedTimeStr = PlayerPrefs.GetString(GetLastRewardTimeKey());
            lastReward = PlayerPrefs.GetInt(GetLastRewardKey());

            // It is not the first time the user claimed.
            // We need to know if he can claim another reward or not
            if (!string.IsNullOrEmpty(lastClaimedTimeStr))
            {
                LastRewardTime = DateTime.ParseExact(lastClaimedTimeStr, FMT, CultureInfo.InvariantCulture);

                // if Debug time was added, we use it to check the difference
                var advancedTime = Now.AddHours(DebugTime.TotalHours);

                var diff = advancedTime - LastRewardTime;
                // Debug.Log("Last claim was " + (long)diff.TotalHours + " hours ago.");

                var days = (int)(Math.Abs(diff.TotalHours) / 24);
                // Debug.Log("days: " + days);

                if (days == 0)
                {
                    // No claim for you. Try tomorrow
                    availableReward = 0;
                    return;
                }

                // The player can only claim if he logs between the following day and the next.
                if (days is >= 1 and < 2)
                {
                    // If reached the last reward, resets to the first restarting the cycle
                    if (lastReward == rewards.Count)
                    {
                        availableReward = 1;
                        lastReward = 0;
                        return;
                    }

                    availableReward = lastReward + 1;

                    Debug.Log("Player can claim prize " + availableReward);
                    return;
                }

                if (days >= 2)
                {
                    // The player loses the following day reward and resets the prize
                    availableReward = 1;
                    lastReward = 0;
                    Debug.Log("Prize reset ");
                }
            }
            else
            {
                // Is this the first time? Shows only the first reward
                availableReward = 1;
            }
        }

        // Checks if the player claim the prize and claims it by calling the delegate. Avoids duplicate call
        public void ClaimPrize()
        {
            if (availableReward > 0)
            {
                // SoundManager.PlaySound(SoundName.SPIN_END);
                // Delegate
                OnClaimPrize?.Invoke(availableReward);

                Debug.Log(" Reward [" + rewards[availableReward - 1] + "] Claimed!");
                PlayerPrefs.SetInt(GetLastRewardKey(), availableReward);
                Debug.Log("Last reward claimed: " + availableReward);

                // Remove seconds
                //var timerNoSeconds = now.AddSeconds(-now.Second);
                // If debug time was added then we store it
                //timerNoSeconds = timerNoSeconds.AddHours(debugTime.TotalHours);

                // var lastClaimedStr = Now.AddHours(DebugTime.TotalHours).ToString(FMT);
                var lastClaimedStr = Now.AddHours(DebugTime.TotalHours).ToString(FMT);

                PlayerPrefs.SetString(GetLastRewardTimeKey(), lastClaimedStr);

                PlayerPrefs.SetInt(GetDebugTimeKey(), (int)DebugTime.TotalHours);
            }
            else if (availableReward == 0)
            {
                Debug.LogError("Error! The player is trying to claim the same reward twice.");
            }

            CheckRewards();
        }

        //Returns the lastReward playerPrefs key depending on instanceId
        private static string GetLastRewardKey() => LastRewardKey;

        //Returns the lastRewardTime playerPrefs key depending on instanceId
        private static string GetLastRewardTimeKey() => LastRewardTimeKey;

        //Returns the advanced debug time playerPrefs key depending on instanceId
        private static string GetDebugTimeKey() => DebugTimeKey;

        // Returns the daily Reward of the day
        public Reward GetReward(int day)
        {
            return rewards[day - 1];
        }

        // Resets the Daily Reward for testing purposes
        public void Reset()
        {
            PlayerPrefs.DeleteKey(GetLastRewardKey());
            PlayerPrefs.DeleteKey(GetLastRewardTimeKey());
            PlayerPrefs.DeleteKey(GetDebugTimeKey());
        }
    }
}