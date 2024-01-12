using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace OneHit.Utility
{
    //[CreateAssetMenu(fileName = "BuildLinkData", menuName = "ScriptableObjects/Build Link Data", order = 1)]
    public class GoogleDriveURL : ScriptableObject
    {
        
        [Header("Link up apk")]
        public string apkLink;
        [Header("Link up aab")]
        public string aabLink;

        private const string dir = "Assets/OneHit/Resources";

        [MenuItem("OneHit/Google Drive URL")]
        internal static GoogleDriveURL LoadInstance()
        {
            var instance = Resources.Load<GoogleDriveURL>("GoogleDriveURL");
            if (instance == null)
            {
                Directory.CreateDirectory(dir);
                instance=CreateInstance<GoogleDriveURL>();
                string assetPath = Path.Combine(dir, "GoogleDriveURL.asset");
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
            }
            Selection.activeObject = instance;

            return instance;
        }
        [Button("Open Apk URL")]
        public void OpenApkURL()
        {
            Application.OpenURL(apkLink);
        }
        [Button("Open Aab URL")]
        public void OpenAabURL()
        {
            Application.OpenURL(aabLink);
        }
    }
}
