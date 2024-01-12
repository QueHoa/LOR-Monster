using UnityEngine;

namespace HolidayEvent
{
    public class HolidayEventHandler
    {
        public delegate void OnEventChange(ThemeType holidayEvent);
        public static OnEventChange onEventChange;

        private static ThemeType holiday=ThemeType.NotSet;
        public static ThemeType Holiday
        {
            get
            {
                if (holiday == ThemeType.NotSet)
                {
                    holiday = (ThemeType)PlayerPrefs.GetInt("Event", 0);
                }
                return holiday;
            }
            set
            {
                holiday = value;
                onEventChange?.Invoke(holiday);
            }
        }
    }
}