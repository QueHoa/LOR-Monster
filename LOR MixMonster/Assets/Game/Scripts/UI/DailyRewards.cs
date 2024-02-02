using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewards : UI.Panel
{
    public CanvasGroup close;
    public AudioClip sfx;
    bool isProcessing = false;
    public override void PostInit()
    {
    }
    public void SetUp()
    {
        StartCoroutine(afterShow());
    }
    IEnumerator afterShow()
    {
        close.interactable = false;
        close.alpha = 0.0f;
        DataManagement.DataManager.Instance.userData.progressData.firstDaily = 2;
        DataManagement.DataManager.Instance.Save();
        Show();
        yield return new WaitForSeconds(2.5f);
        close.interactable = true;
        DOTween.To(() => close.alpha, x => close.alpha = x, 1, 0.5f).SetEase(Ease.Linear);
    }
    public void BackHome()
    {
        if (DataManagement.DataManager.Instance.userData.progressData.playCount >= Game.Controller.Instance.gameConfig.adConfig.adStart)
        {
            AD.Controller.Instance.ShowInterstitial(() =>
            {
                Back();

            });
        }
        else
        {
            Back();
        }

        void Back()
        {
            if (isProcessing) return;
            isProcessing = true;
            Sound.Controller.Instance.PlayOneShot(sfx);
            (Game.Controller.Instance.gameController).hideMonster = false;
            Close();
        }

    }
}
