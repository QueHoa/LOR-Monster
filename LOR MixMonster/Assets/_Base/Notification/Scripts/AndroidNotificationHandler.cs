using UnityEngine;
using UnityEngine.Android;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

namespace Base.Notification
{
    public class AndroidNotificationHandler : MonoBehaviour
    {
#if UNITY_ANDROID
        private static string ChannelId => Application.identifier; // use to identify the channel

        private NotificationConfig notificationConfig;

        public void Initialize(NotificationConfig notificationConfig)
        {
            this.notificationConfig = notificationConfig;
            RequestPermission();
            CreateChanel();
        }

        private void RequestPermission()
        {
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Debug.Log("[Notification] Requesting permission...");
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
        }

        private void CreateChanel()
        {
            // Create the Android Channel to send message through
            var channel = new AndroidNotificationChannel()
            {
                Id = ChannelId,
                Name = Application.productName,
                Importance = Importance.Default,
                Description = "Reminder notifications",
            };

            Debug.Log($"[Notification] Created channel: Id = {channel.Id}, Name = {channel.Name}");
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        public void ScheduleNotifications()
        {
            Debug.Log("[Notification] Schedule notifications...");

            // create notifications to be sent
            for (var id = 0; id < this.notificationConfig.notifications.Count; id++)
            {
                var notificationData = this.notificationConfig.notifications[id];

                var notification = new AndroidNotification()
                {
                    Title = this.notificationConfig.GetRandomIcon() + notificationData.title + this.notificationConfig.GetRandomIcon(),
                    Text = notificationData.message,
                    FireTime = notificationData.GetFireTimeFromNow(),
                    SmallIcon = NotificationManager.SMALL_ICON,
                    LargeIcon = NotificationManager.LARGE_ICON,
                    ShowTimestamp = true,
                    ShowInForeground = true,
                };

                // use explicit id to cancel notification
                AndroidNotificationCenter.SendNotificationWithExplicitID(notification, ChannelId, id);
                Debug.Log($"[Notification] Schedule notification: id: {id}, title: {notification.Text.Substring(0, 10)}..., time: {notification.FireTime}");
            }
        }

        public void CancelAllNotifications()
        {
            Debug.Log("[Notification] Cancel all notifications");
            AndroidNotificationCenter.CancelAllScheduledNotifications();

            // for (var i = 0; i < this.notificationConfig.notifications.Count; i++)
            //     AndroidNotificationCenter.CancelScheduledNotification(i);
        }
#endif
    }
}