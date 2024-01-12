using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace DailyReward
{
    public class DailyRewardsScreen : MonoBehaviour
    {
        [Title("Daily Rewards Panel")]
        public Button buttonClaim;
        // public ScrollRect scrollRect;
        public TextMeshProUGUI textTimeDue;
        public string color = "red";
        public DailyRewardPopup dailyRewardPopup;

        [Space(20)]
        public List<UIDailyReward> dailyRewardsUI = new();

        private DailyRewardsHandler DailyRewardsHandler => DailyRewardsHandler.Instance;
        private bool _readyToClaim;
        private void Start()
        {
            InitializeDailyRewardsUI();
            UpdateUI();
        }

        private void InitializeDailyRewardsUI()
        {
            for (var i = 0; i < DailyRewardsHandler.rewards.Count; i++)
            {
                var day = i + 1;
                var reward = DailyRewardsHandler.GetReward(day);
                var uiDailyReward = dailyRewardsUI[i];

                uiDailyReward.day = day;
                uiDailyReward.reward = reward;
                uiDailyReward.Initialize();
            }
        }

        private void UpdateUI()
        {
            DailyRewardsHandler.CheckRewards();

            var isRewardAvailableNow = false;
            var lastReward = DailyRewardsHandler.lastReward;
            var availableReward = DailyRewardsHandler.availableReward;

            foreach (var dailyRewardUI in dailyRewardsUI)
            {
                var day = dailyRewardUI.day;

                if (day == availableReward)
                {
                    dailyRewardUI.state = DailyRewardState.UnclaimAvailable;
                    isRewardAvailableNow = true;
                }
                else if (day <= lastReward)
                {
                    dailyRewardUI.state = DailyRewardState.Claimed;
                }
                else
                {
                    dailyRewardUI.state = DailyRewardState.UnclaimUnavailable;
                }

                dailyRewardUI.Refresh();
            }

            buttonClaim.interactable = isRewardAvailableNow;

            if (isRewardAvailableNow)
            {
                SnapToReward();
                textTimeDue.text = "You can claim your reward in day " + availableReward + "!";
            }

            _readyToClaim = isRewardAvailableNow;
        }

        private void SnapToReward()
        {
            Canvas.ForceUpdateCanvases();

            var lastRewardIdx = DailyRewardsHandler.lastReward;

            // Scrolls to the last reward element
            if (dailyRewardsUI.Count - 1 < lastRewardIdx)
                lastRewardIdx++;

            if (lastRewardIdx > dailyRewardsUI.Count - 1)
                lastRewardIdx = dailyRewardsUI.Count - 1;

            var target = dailyRewardsUI[lastRewardIdx].GetComponent<RectTransform>();

            // var content = scrollRect.content;

            //content.anchoredPosition = (Vector2)scrollRect.transform.InverseTransformPoint(content.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

            // var normalizePosition = (float)target.GetSiblingIndex() / content.transform.childCount;
            // scrollRect.verticalNormalizedPosition = normalizePosition;
        }

        private void Update()
        {
            // * add unscaleDeltaTime instead get DateTime.Now
            DailyRewardsHandler.TickTime();

            // Updates the time due
            CheckTimeDifference();
        }

        private void CheckTimeDifference()
        {
            if (!_readyToClaim)
            {
                var difference = DailyRewardsHandler.GetTimeDifference();

                // If the counter below 0 it means there is a new reward to claim
                if (difference.TotalSeconds <= 0)
                {
                    _readyToClaim = true;
                    UpdateUI();
                    SnapToReward();
                    return;
                }

                var formattedTs = DailyRewardsHandler.GetFormattedTime(difference);

                textTimeDue.text = $"Come back in <color={color}>{formattedTs}</color> for your next reward";
            }
        }

        //--------------------------------------------------//

        private void Awake()
        {
            DailyRewardsHandler.OnInitialize += OnInitialize;
            DailyRewardsHandler.OnClaimPrize += OnClaimPrize;
        }

        private void OnDestroy()
        {
            DailyRewardsHandler.OnInitialize -= OnInitialize;
            DailyRewardsHandler.OnClaimPrize -= OnClaimPrize;
        }

        private void OnInitialize()
        {
            UpdateUI();
            SnapToReward();
            CheckTimeDifference();
        }

        private void OnClaimPrize(int day)
        {
            //FirebaseManager.Instance?.LogEvent("DailyReward"+day);
            // Show reward info popup
        }

        public Vector3 GetPosOfRewardDay(int day)
        {
            return dailyRewardsUI[day - 1].transform.position;
        }

        public void OnClickClaimButton()
        {
            dailyRewardPopup.SetUp(DailyRewardsHandler.availableReward);
            DailyRewardsHandler.ClaimPrize();
            _readyToClaim = false;
            UpdateUI();
        }
        [Button]
        public void Open()
        {
            gameObject.SetActive(true);
        }
        #region ========== DEBUG ==========

        public void OnClickAdvanceDayButton()
        {
            DailyRewardsHandler.DebugTime = DailyRewardsHandler.DebugTime.Add(new TimeSpan(1, 0, 0, 0));
            UpdateUI();
        }


        public void OnClickAdvanceHourButton()
        {
            DailyRewardsHandler.DebugTime = DailyRewardsHandler.DebugTime.Add(new TimeSpan(1, 0, 0));
            UpdateUI();
        }

        public void OnClickResetButton()
        {
            DailyRewardsHandler.Reset();
            DailyRewardsHandler.DebugTime = new TimeSpan();
            DailyRewardsHandler.LastRewardTime = DateTime.MinValue;
            _readyToClaim = false;
        }

        #endregion
    }
}