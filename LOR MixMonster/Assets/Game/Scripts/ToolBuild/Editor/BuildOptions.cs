using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks;

namespace OneHit.Utility
{
    public class BuildOptions
    {
        [MenuItem("OneHit/Build/Apk Mono")]
        public static void BuildApkMono()
        {
            EditorUserBuildSettings.buildAppBundle = false;

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            OpenBuildWindow();
        }
        [MenuItem("OneHit/Build/Apk IL2CPP")]
        public static void BuildApkIL2CPP()
        {
            EditorUserBuildSettings.buildAppBundle = false;

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            OpenBuildWindow();
        }
        [MenuItem("OneHit/Build/AAB up vercode")]
        public static void BuildAAB_upvercode()
        {
            EditorUserBuildSettings.buildAppBundle = true;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.bundleVersionCode++;
            OpenBuildWindow();
        }
        [MenuItem("OneHit/Build/AAB non up vercode")]
        public static void BuildAAB_nonUpvercode()
        {
            EditorUserBuildSettings.buildAppBundle = true;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            OpenBuildWindow();
        }

        public static void OpenBuildWindow()
        {
            EnterPassword();
            //Editor.MonetizationData.CreateInstance().LoadFromFileCsv();
            EditorApplication.ExecuteMenuItem("File/Build Settings...");
        }
        public static string pw = "dangle";
        public static void EnterPassword()
        {
            // Set the keystore and key alias passwords when Unity asks for them during the build process.
            PlayerSettings.Android.keystorePass = pw;
            PlayerSettings.Android.keyaliasPass = pw;
        }

    }
}