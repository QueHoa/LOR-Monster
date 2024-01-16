using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Notification
{
    [CreateAssetMenu(fileName = "NotificationConfig", menuName = "Base/NotificationConfig")]
    public class NotificationConfig : ScriptableObject
    {
        public List<NotificationData> notifications = new List<NotificationData>();

        private readonly List<string> symbolIcons = new List<string>
        {
            "🎁", "⏰", "🎉", "🎊", "🎀", "🎯", "🎮", "🏆",
        };

        public string GetRandomIcon()
        {
            try
            {
                return this.symbolIcons[UnityEngine.Random.Range(0, this.symbolIcons.Count)];
            }
            catch (Exception e)
            {
                Debug.LogError("[Notification] Error " + e);
                return "";
            }
        }
    }

    [Serializable]
    public class NotificationData
    {
        public string title;
        public string message;
        public NotificationType type;
        public NotificationTime time;

        public DateTime GetFireTimeFromNow()
        {
            // Nếu muốn bắn thông báo sau khi thoát game: add thời gian tính từ Dâtetime.Now
            // Nếu muốn bắn thông báo vào 1 thời điểm cố định trong ngày: add thời gian tính từ DateTime.Today
            var fireTime = this.type switch
            {
                NotificationType.AfterExitingGame => DateTime.Now.AddDays(this.time.day).AddHours(this.time.hour).AddMinutes(this.time.minute).AddSeconds(this.time.second),
                NotificationType.DailyTime => DateTime.Today.AddHours(this.time.hour).AddMinutes(this.time.minute).AddSeconds(this.time.second),
                _ => DateTime.Now.AddDays(1)
            };

            // Tránh notify từ 23h đêm đến 9h sáng (nếu không phải notification ngay sau khi thoát game vài 1 vài phút)
            if ((fireTime.Hour >= 23 || fireTime.Hour <= 9) && (fireTime - DateTime.Now).TotalMinutes >= 5)
                fireTime = fireTime.AddHours(10);

            // Nếu fireTime dã qua, tại thời điểm schedule notifications nó sẽ bị bắn ngay lập tức
            // Vì vậy thêm 1 ngày để bắn vào khung giờ đó ngày hôm sau
            if (fireTime < DateTime.Now)
                fireTime = fireTime.AddDays(1);

            return fireTime;
        }

        public enum NotificationType
        {
            AfterExitingGame,
            DailyTime,
        }

        [Serializable]
        public class NotificationTime
        {
            public int day;
            public int hour;
            public int minute;
            public int second;
        }
    }
}

// "🤩", "🥳", "🤗", "🤭", "🤫", "🤔", "🤐", "🤨", "😐", "😑", "😶", "😏", "😒", "🙄", "😬", "🤥",
// "🤫", "🤭", "🧐", "🤓", "😎", "🥸", "🤡", "🤠", "🤗", "🤩", "🥳", "😏", "😒", "😞", "😔", "😟",
// "😕", "🙁", "☹️", "😣", "😖", "😫", "😩", "🥺", "😢", "😭", "😤", "😠", "😡", "🤬", "🤯", "😳",
// "🥵", "🥶", "😱", "😨", "😰", "😥", "😓", "🤗", "🤔", "🤭", "🤫", "🤥", "😶", "😐", "😑", "😬",
// "🙄", "😯", "😦", "😧", "😮", "😲", "🥱", "😴", "🤤", "😪", "😵", "🤐", "🥴", "🤢", "🤮", "🤧",