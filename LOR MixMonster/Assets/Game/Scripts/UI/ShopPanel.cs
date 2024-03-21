using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ShopPanel : UI.Panel
{
    [SerializeField]
    private GameObject special, premium, noads;
    [SerializeField]
    private TMPro.TextMeshProUGUI goldText;
    private int gold; 
    [SerializeField]
    private AudioClip finishSFX;
    bool isProcessing;
    CancellationTokenSource cancellation;
    private void OnEnable()
    {
        cancellation = new CancellationTokenSource();
    }
    public override void PostInit()
    {
    }
    public void SetUp()
    {
        isProcessing = false;
        goldText.text = DataManagement.DataManager.Instance.userData.YourGold.ToString();
        special.SetActive(DataManagement.DataManager.Instance.userData.inventory.GetItemState("SetBundle_1") == 0 && (DataManagement.DataManager.Instance.userData.IsAd || DataManagement.DataManager.Instance.userData.stageListData.isNoAds));
        premium.SetActive(DataManagement.DataManager.Instance.userData.inventory.GetItemState("SetBundle_1") == 0);
        noads.SetActive(DataManagement.DataManager.Instance.userData.IsAd || DataManagement.DataManager.Instance.userData.stageListData.isNoAds);
        Show();
    }
    public async UniTaskVoid ChangeGold(int coin)
    {
        goldText.transform.DOScale(Vector3.one * 1.3f, 0.5f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
        DOTween.To(() => gold, x => gold = x, DataManagement.DataManager.Instance.userData.YourGold + coin, 1f).SetEase(Ease.OutQuad).OnUpdate(() =>
        {
            goldText.text = gold.ToString();
        }).OnComplete(() =>
        {
            DataManagement.DataManager.Instance.userData.YourGold = gold;
            DataManagement.DataManager.Instance.Save();
            Sound.Controller.Instance.PlayOneShot(finishSFX);
        });
        await UniTask.Delay(1300, cancellationToken: cancellation.Token);
    }
    public void Back()
    {
        if (isProcessing) return;
        isProcessing = true;
        LevelLoading.Instance.Active(() =>
        {
            Close();
            AD.Controller.Instance.ShowInterstitial();
            UI.PanelManager.Create(typeof(HomePanel), (panel, op) =>
            {
                ((StageGameController)Game.Controller.Instance.gameController).homePanel = panel as HomePanel;
                ((HomePanel)panel).SetUp();
                ((StageGameController)Game.Controller.Instance.gameController).ShowCurrentStageMonster();
                ((StageGameController)Game.Controller.Instance.gameController).RestoreStageView();
            });
            LevelLoading.Instance.Close();
        });
    }
}
