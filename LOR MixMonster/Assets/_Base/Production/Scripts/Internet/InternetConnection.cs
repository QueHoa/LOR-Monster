using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Base.Production.Internet
{
    public class InternetConnection : Singleton<InternetConnection>
    {
        [Title("Panel")]
        public DarkBgPanel noInternetPanel;

        [Title("Config")]
        public bool requireInternet;

        [ShowIf("@requireInternet")]
        public float timePerInternetCheck = 3f;

        private void Start()
        {
            LogInternetStatus();
            CheckInternetAsync();
        }

        private static void LogInternetStatus()
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    Debug.Log("[Internet] Network is available through wifi!".Color("lime"));
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    Debug.Log("[Internet] Network is available through mobile data!".Color("lime"));
                    break;
                case NetworkReachability.NotReachable:
                    Debug.LogError("[Internet] Network not available!".Color("red"));
                    break;
            }
        }

        private async void CheckInternetAsync()
        {
            // don't check internet if not require
            if (!requireInternet) return;

            // wait scene LOADING complete, check internet in scene MAIN
            await UniTask.WaitUntil(() => SceneManager.GetActiveScene().buildIndex != 0);

            Debug.LogWarning("[Internet] Start check internet...".Color("orange"));

            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(timePerInternetCheck));

                if (!HasInternet())
                {
                    noInternetPanel.Appear();
                    await UniTask.WaitUntil(HasInternet);
                    noInternetPanel.Disappear();
                }
            }
        }

        public void OpenWifiSetting()
        {
#if UNITY_ANDROID
            try
            {
                var intent = new AndroidJavaObject("android.content.Intent");
                intent.Call<AndroidJavaObject>("setAction", "android.settings.WIFI_SETTINGS");

                var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intent);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
#endif
        }

        public static bool HasInternet()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}