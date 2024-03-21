using Cysharp.Threading.Tasks;
using DG.Tweening;
using ItemData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class NoAdsPanel : UI.Panel
{
    [SerializeField]
    private GameObject[] adsCount;
    [SerializeField]
    private CanvasGroup close;
    System.Action onUnlock;
    bool isProcessing;
    public override void PostInit()
    {
    }
    public void SetUp(System.Action onUnlock)
    {
        isProcessing = false;
        if (((StageGameController)Game.Controller.Instance.gameController).FIRST_NOADS < 2)
        {
            ((StageGameController)Game.Controller.Instance.gameController).FIRST_NOADS++;
        }
        for (int i = 0; i < adsCount.Length; i++)
        {
            if(i < DataManagement.DataManager.Instance.userData.progressData.adsCount)
            {
                adsCount[i].SetActive(true);
            }
            else
            {
                adsCount[i].SetActive(false);
            }
        }
        this.onUnlock = onUnlock;
        StartCoroutine(afterShow());
    }
    IEnumerator afterShow()
    {
        close.interactable = false;
        close.alpha = 0.0f;
        Show();
        yield return new WaitForSeconds(2.5f);
        close.interactable = true;
        DOTween.To(() => close.alpha, x => close.alpha = x, 1, 0.5f).SetEase(Ease.Linear);
    }
    public void Watch1()
    {
        if (isProcessing) return;
        isProcessing = true;
        AD.Controller.Instance.ShowRewardedAd("NoAds", res =>
        {
            if (res)
            {
                Unlock(3);
            }
            else
            {
#if UNITY_EDITOR
                Unlock(3);
#endif
            }
        });

    }
    public void Watch2()
    {
        if (isProcessing) return;
        isProcessing = true;
        AD.Controller.Instance.ShowRewardedAd("NoAds", res =>
        {
            if (res)
            {
                DataManagement.DataManager.Instance.userData.progressData.adsCount++;
                for (int i = 0; i < adsCount.Length; i++)
                {
                    if (i < DataManagement.DataManager.Instance.userData.progressData.adsCount)
                    {
                        adsCount[i].SetActive(true);
                    }
                    else
                    {
                        adsCount[i].SetActive(false);
                    }
                }
                if (DataManagement.DataManager.Instance.userData.progressData.adsCount == 3)
                {
                    Unlock(9);
                    DataManagement.DataManager.Instance.userData.progressData.adsCount = 0;
                }
                DataManagement.DataManager.Instance.Save();
                isProcessing = false;
            }
            else
            {
#if UNITY_EDITOR
                DataManagement.DataManager.Instance.userData.progressData.adsCount++;
                for (int i = 0; i < adsCount.Length; i++)
                {
                    if (i < DataManagement.DataManager.Instance.userData.progressData.adsCount)
                    {
                        adsCount[i].SetActive(true);
                    }
                    else
                    {
                        adsCount[i].SetActive(false);
                    }
                }
                if (DataManagement.DataManager.Instance.userData.progressData.adsCount == 3)
                {
                    Unlock(9);
                    DataManagement.DataManager.Instance.userData.progressData.adsCount = 0;
                }
                DataManagement.DataManager.Instance.Save();
                isProcessing = false;
#endif
            }
        });

    }
    public void Unlock(int type)
    {
        onUnlock?.Invoke();
        UI.PanelManager.Create(typeof(MessengerPanel), (panel, op) =>
        {
            ((MessengerPanel)panel).SetUp(type);
            isProcessing = false;
        });
        Close();
    }
    public override void Close()
    {
        base.Close();
        (Game.Controller.Instance.gameController).hideMonster = false;
    }
}
