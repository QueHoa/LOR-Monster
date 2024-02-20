using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitLoader : UnityEngine.MonoBehaviour
{
    // Start is called before the first frame update
    async UniTaskVoid Start()
    {
        Application.targetFrameRate = 60;
        AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> op;
        op = Addressables.LoadSceneAsync(SceneHandle.GetScene(1), loadMode: LoadSceneMode.Single, false);
        await op;
       
        PlayerPrefs.SetInt("LoginCount", PlayerPrefs.GetInt("LoginCount", 0) + 1);

        DataManagement.DataManager.Instance.IsReady();
        //AD.Controller.Instance.Init(DataManagement.DataManager.Instance.userData.IsAd);
        //AD.Controller.Instance.LoadOpenAd();
        //AD.Controller.Instance.LoadBanner();
        //AD.Controller.Instance.LoadInterstitial();

        IAP.Controller.Instance.InitProductAsync();
        Localization.LocalizationHandler.Init();

        await UniTask.Delay(2500);
        await op.Result.ActivateAsync();
    }

}
public static class Extensions
{
    public static string Color(this string str, string color)
    {
        return $"<color={color}>{str}</color>";
    }
}
