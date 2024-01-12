using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Firebase.RemoteConfig;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.AddressableAssets;

public class RemoteConfigHandler : UnityEngine.MonoBehaviour
{
    public static RemoteConfigHandler Instance;
    public bool isReady = false;
    public string defaultAssetBundle,defaultLevelPack;
    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            FirebaseManager.onInit -= GetDataAndActive;
            FirebaseManager.onInit += GetDataAndActive;
            if (FirebaseManager.Instance!=null&& FirebaseManager.Instance.isReady)
            {
                try
                {
                    GetDataAndActive();
                }
                catch (System.Exception e)
                {
                    GameUtility.GameUtility.LogError(e);
                    isReady = true;
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void GetDataAndActive()
    {
        //set giá trị default cho remote
        try
        {
            //UniTask setDefaultTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).AsUniTask();
            //// đợi khi nào task set giá trị default thành công
            //await setDefaultTask;
            // đợi firebase lấy dữ liệu remote xong
            await Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).AsUniTask();
        }
        catch (System.Exception e)
        {
            GameUtility.GameUtility.LogError(e);
        }
        //lấy dữ liệu từ remote
        FetchComplete().Forget();
    }

    private async UniTaskVoid FetchComplete()
    {
        await UniTask.WaitUntil(() => FirebaseManager.Instance.isReady);

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;

        string AssetBundle = null;

        GameUtility.GameUtility.Log("LAST: " + info.LastFetchStatus);
        //fetching configs from server
        try
        {
            UniTask<bool> activeTask = FirebaseRemoteConfig.DefaultInstance.ActivateAsync().AsUniTask();
            await activeTask;
            AssetBundle = FirebaseRemoteConfig.DefaultInstance.GetValue("Data_GameConfig").StringValue;

        }
        catch (System.Exception e)
        {
            GameUtility.GameUtility.LogError(e);
        }

        if (!Game.Controller.Instance.gameConfig.editMode && !string.IsNullOrEmpty(AssetBundle))
        {
            Debug.Log("REMOTE : SETUP game config ");
            Game.Controller.Instance.gameConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<GameConfig>(AssetBundle, new ObscuredValueConverter());
            Debug.Log("REMOTE : FINISH SETUP game config " + Game.Controller.Instance.gameConfig.adConfig.ToString());

        }



        FirebaseManager.onInit -= GetDataAndActive;
        isReady = true;


    }

    private void OnDestroy()
    {
        FirebaseManager.onInit -= GetDataAndActive;
    }

}
