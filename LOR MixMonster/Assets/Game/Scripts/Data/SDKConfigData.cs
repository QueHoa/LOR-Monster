using UnityEngine;

[CreateAssetMenu(menuName ="SDK/Config")]
public class SDKConfigData : ScriptableObject
{
    public SDKIdConfig sdkIdConfig;

    [System.Serializable]
    public struct SDKIdConfig
    {
        public string admobOpenAdID_Android, admobOpenAdID_Ios;
        public string admobNativeAdID_Android, admobNativeAdID_Ios;
        public string adjustID_Android, adjustID_Ios;
        public com.adjust.sdk.AdjustEnvironment adjustEnvironment;
    }
}