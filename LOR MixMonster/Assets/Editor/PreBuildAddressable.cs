using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class PreBuildAddressable
{
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
    }
    private static void BuildPlayerHandler(BuildPlayerOptions buildPlayerOptions)
    {
        Debug.Log("PREBUILD ASSET");
        AddressableAssetSettings.CleanPlayerContent();
        AddressableAssetSettings.BuildPlayerContent();
        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
        Debug.Log("BUILT ASSET");

    }
}
