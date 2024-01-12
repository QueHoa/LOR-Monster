using System;
using UnityEngine;

namespace DailyReward
{
    public abstract class DailyRewardsCore<T> : Singleton<T> where T : MonoBehaviour
    {
        protected DateTime Now; // The actual date. Either returned by the using the world clock or the player device clock

        [HideInInspector]
        public bool isInitialized;

        protected void InitializeDate()
        {
            Now = DateTime.Now;
            isInitialized = true;
        }

        protected void RefreshTime()
        {
            Now = DateTime.Now;
        }

        //Updates the current time
        public virtual void TickTime()
        {
            if (!isInitialized) return;
            Now = Now.AddSeconds(Time.unscaledDeltaTime);
        }

        public string GetFormattedTime(TimeSpan span)
        {
            return $"{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}";
        }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                RefreshTime();
            }
        }
    }
}