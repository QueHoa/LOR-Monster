using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace Base.Notification
{
    public class IOSNotificationHandler : MonoBehaviour
    {
#if UNITY_IOS
        private NotificationConfig notificationConfig;

        public void Initialize(NotificationConfig notificationConfig)
        {
            this.notificationConfig = notificationConfig;
            RequestAuthorization().Forget();
        }

        private async UniTask RequestAuthorization()
        {
            const AuthorizationOption authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
            using var req = new AuthorizationRequest(authorizationOption, true);
            await UniTask.WaitUntil(() => req.IsFinished);

            var res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log("[Notification] Requested authorization: " + res);
        }

        public void ScheduleNotifications()
        {
            Debug.Log("[Notification] Schedule notifications");

            // create notifications to be sent
            for (var id = 0; id < this.notificationConfig.notifications.Count; id++)
            {
                var notificationData = this.notificationConfig.notifications[id];

                var notification = new iOSNotification()
                {
                    Identifier = id.ToString(),
                    Title = Application.productName,
                    Subtitle = this.notificationConfig.GetRandomIcon() + notificationData.title + this.notificationConfig.GetRandomIcon(),
                    Body = notificationData.message,
                    ShowInForeground = true,
                    ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                    CategoryIdentifier = "category_a",
                    ThreadIdentifier = "thread1",
                    Trigger = CreateTimeTrigger(notificationData),
                };

                iOSNotificationCenter.ScheduleNotification(notification);
            }
        }

        private iOSNotificationTrigger CreateTimeTrigger(NotificationData notificationData)
        {
            iOSNotificationTrigger timeTrigger = null;
            if (notificationData.type == NotificationData.NotificationType.AfterExitingGame)
            {
                var fireTime = notificationData.GetFireTimeFromNow();
                timeTrigger = new iOSNotificationCalendarTrigger()
                {
                    Year = fireTime.Year,
                    Month = fireTime.Month,
                    Day = fireTime.Day,
                    Hour = fireTime.Hour,
                    Minute = fireTime.Minute,
                    Second = fireTime.Second,
                    Repeats = false
                };
            }
            else if (notificationData.type == NotificationData.NotificationType.DailyTime)
            {
                timeTrigger = new iOSNotificationCalendarTrigger()
                {
                    Hour = notificationData.time.hour,
                    Minute = notificationData.time.minute,
                    Second = notificationData.time.second,
                    Repeats = true
                };
            }

            return timeTrigger;
        }

        public void CancelAllNotifications()
        {
            Debug.Log("[Notification] Cancel all notifications");
            foreach (var notification in iOSNotificationCenter.GetDeliveredNotifications())
            {
                iOSNotificationCenter.RemoveDeliveredNotification(notification.Identifier);
            }
        }
#endif
    }
}