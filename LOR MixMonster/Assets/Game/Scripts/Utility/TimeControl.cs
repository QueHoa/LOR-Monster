using UnityEngine;


namespace TimeHandler
{
    public static class TimeControl
    {
        public delegate void OnTimeChanged(float timeScale);
        public static OnTimeChanged onTimeChanged;

        public static void SetBulletTime(float ratio)
        {
            Time.timeScale = 1f/ ratio;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            onTimeChanged?.Invoke(Time.timeScale);
        }
        public static void Freeze()
        {
            Time.timeScale =0;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            onTimeChanged?.Invoke(Time.timeScale);
        }
        public static void Continue()
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            onTimeChanged?.Invoke(Time.timeScale);
        }
    }
}