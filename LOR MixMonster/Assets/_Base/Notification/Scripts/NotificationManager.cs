using UnityEngine;
//using Base.Pattern.Singleton;

namespace Base.Notification
{
    public class NotificationManager : SingletonPersistent<NotificationManager>
    {
        [SerializeField]
        private NotificationConfig notificationConfig;

        [SerializeField]
        private AndroidNotificationHandler androidNotificationHandler;

        [SerializeField]
        private IOSNotificationHandler iosNotificationHandler;

        public const string SMALL_ICON = "small_icon"; // small icon to display in the notification area
        public const string LARGE_ICON = "large_icon"; // large icon to display in the notification area

        private bool isInitialized;

        private void Start()
        {
            Debug.Log("[Notification] Initialize...");
            this.isInitialized = true;

#if UNITY_ANDROID
            this.androidNotificationHandler.Initialize(this.notificationConfig);
#elif UNITY_IOS
             this.iosNotificationHandler.Initialize(this.notificationConfig);
#endif
        }

        private void OnApplicationPause(bool paused)
        {
            if (!this.isInitialized)
                return;

#if UNITY_ANDROID
            if (paused)
                this.androidNotificationHandler.ScheduleNotifications();
            else
                this.androidNotificationHandler.CancelAllNotifications();
#elif UNITY_IOS
            if (paused)
                this.iosNotificationHandler.ScheduleNotifications();
            else
                this.iosNotificationHandler.CancelAllNotifications();
#endif
        }
    }
}