using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetUp().Forget();
    }
    async UniTaskVoid SetUp()
    {
        await UniTask.WaitUntil(() => DataManagement.DataManager.Instance.IsReady());
        await UniTask.WaitUntil(() => Sound.Controller.Instance.IsReady() && Sheet.SheetDataManager.Instance.isReady );
       
        await UniTask.Delay(150);
        LoadMainScene();

    }

    void LoadMainScene()
    {
        LevelLoading.Instance.Active("MainScene", null,
            () =>
            {

                /*UI.PanelManager.Create(typeof(HomePanel), (panel, op) =>
                {
                    ((HomePanel)panel).SetUp();
                });*/
                Game.Controller.Instance.gameController.SetUp();
                AD.Controller.Instance.ShowBanner();
                if(Game.Controller.Instance.gameConfig.skipAd)
                {
                    AD.Controller.Instance.HideBanner();
                }

                ShowOpenAd();

                LevelLoading.Instance.Close();
            }
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
