using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // TODO: show consent
        GDPRHandler.Instance.RequestConsent();
        SetUp().Forget();
    }
    async UniTaskVoid SetUp()
    {
        await UniTask.WaitUntil(() => DataManagement.DataManager.Instance.IsReady() && Sound.Controller.Instance.IsReady() && Sheet.SheetDataManager.Instance.isReady );
        
        await UniTask.Delay(150);
        LoadMainScene();

    }

    void LoadMainScene()
    {
        var sceneKey = (DataManagement.DataManager.Instance.userData.inventory.GetFirstCollection() == null
                            && DataManagement.DataManager.Instance.userData.stageListData.stageDatas.Count == 0)
                ? "MainScene"
                : "HomeScene";

        LevelLoading.Instance.Active(sceneKey, null,
            UniTask.Action(async () =>
            {
                Game.Controller.Instance.gameController.SetUp();
                //AD.Controller.Instance.ShowBanner();
                CollapsibleBanner.ShowBanner();
                if(Game.Controller.Instance.gameConfig.skipAd)
                {
                    //AD.Controller.Instance.HideBanner();
                    CollapsibleBanner.HideBanner();
                }
                await UniTask.Delay(150);
                ShowOpenAd();
            })
            , closeOverride: true);
    }

    async UniTask ShowOpenAd()
    {
        float startTime = Time.time;
        float waitOpenAdTimeOut = 4;
        await UniTask.WaitUntil(() => (!AD.Controller.Instance.isAd || AD.Controller.Instance.IsOpenAdAvailable()) || (Time.time - startTime >= waitOpenAdTimeOut));
        AD.Controller.Instance.ShowOpenAd();
    }
}
