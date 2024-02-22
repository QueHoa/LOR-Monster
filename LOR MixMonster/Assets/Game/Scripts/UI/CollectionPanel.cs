using UnityEngine.UI;
using DG.Tweening;
using JetBrains.Annotations;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using CodeStage.AntiCheat.Genuine.CodeHash;
//using static UnityEngine.Rendering.DebugUI;
using System.IO;
using System.Collections;
using Sirenix.OdinInspector;

public class CollectionPanel : UI.Panel
{
    GameUtility.Pooling.PoolHandler poolImage;

    public TextMeshProUGUI viewText, likeText;
    public Image[] iconItem;
    public Image mainImage;
    public CanvasGroup boxView, boxLike;
    public CanvasGroup[] boxItem;
    public ParticleSystem[] boxItemPS;
    public GameObject[] star;
    public Transform monsterPos;

    private bool isProcessing;
    private int numberStar;
    public override void PostInit()
    {
    }
    public async void SetUp(int numberAnim)
    {
        isProcessing = false;
        numberStar = 0;
        if(DataManagement.DataManager.Instance.userData.progressData.collectionDatas.Count > 0)
        {
            StartCoroutine(Effect());

            viewText.text = GameUtility.GameUtility.ShortenNumber(DataManagement.DataManager.Instance.userData.progressData.collectionDatas[0].view);
            likeText.text = GameUtility.GameUtility.ShortenNumber(DataManagement.DataManager.Instance.userData.progressData.collectionDatas[0].like);
            foreach (ItemData.Item item in DataManagement.DataManager.Instance.userData.progressData.collectionDatas[0].collectionItem)
            {
                
                await SetItem(item);
                ((StageGameController)Game.Controller.Instance.gameController).monster.SetItem(item);
            }
            ((StageGameController)Game.Controller.Instance.gameController).monster.Dance(numberAnim);
            poolImage = GetComponentInChildren<GameUtility.Pooling.PoolHandler>();

            foreach (DataManagement.CollectionData collectionData in DataManagement.DataManager.Instance.userData.progressData.collectionDatas)
            {
                PhotoButton photoButton = poolImage.Get().GetComponent<PhotoButton>();
                photoButton.SetUp(this);
            }
            ChangeCollect(1);
        }
        
        Show();
    }
    public async UniTask<GameObject> SetItem(ItemData.Item item)
    {
        switch (item.category)
        {
            case ItemData.Category.Head:
                iconItem[0].sprite = await item.GetIconAsync();
                return iconItem[0].gameObject;
            case ItemData.Category.Accessory:
                iconItem[1].sprite = await item.GetIconAsync();
                return iconItem[1].gameObject;
            case ItemData.Category.Eye:
                iconItem[2].sprite = await item.GetIconAsync();
                return iconItem[2].gameObject;
            case ItemData.Category.Mouth:
                iconItem[3].sprite = await item.GetIconAsync();
                return iconItem[3].gameObject;
            case ItemData.Category.Body:
                iconItem[4].sprite = await item.GetIconAsync();
                return iconItem[4].gameObject;

        }
        return null;
    }
    public IEnumerator Effect()
    {
        star[numberStar].SetActive(true);
        int swap = numberStar;
        if (numberStar == 0)
        {
            numberStar = 1;
        }
        else
        {
            numberStar = 0;
        }
        boxView.alpha = 0;
        boxView.transform.localScale = Vector3.one * 1.3f;
        boxLike.alpha = 0;
        boxLike.transform.localScale = Vector3.one * 1.3f;
        for (int i = 0; i < boxItem.Length; i++)
        {
            boxItem[i].alpha = 0;
            boxItem[i].transform.localScale = Vector3.one * 0.7f;
        }
        DOTween.To(() => boxView.alpha, x => boxView.alpha = x, 1, 0.5f).SetEase(Ease.Linear);
        boxView.transform.DOScale(1, 0.5f).SetEase(Ease.Linear);
        DOTween.To(() => boxLike.alpha, x => boxLike.alpha = x, 1, 0.5f).SetEase(Ease.Linear);
        boxLike.transform.DOScale(1, 0.5f).SetEase(Ease.Linear);
        for (int i = 0; i < boxItem.Length; i++)
        {
            int index = i;
            DOTween.To(() => boxItem[index].alpha, x => boxItem[index].alpha = x, 1, 0.3f).SetEase(Ease.Linear);
            boxItem[index].transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            boxItemPS[index].Play();
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(1.2f);
        star[swap].SetActive(false);
    }
    public void ChangeCollect(int index)
    {
        foreach (Transform button in poolImage.transform)
        {
            button.GetComponent<PhotoButton>().collecting.SetActive(false);
        }
        poolImage.transform.GetChild(index).GetComponent<PhotoButton>().collecting.SetActive(true);
    }
    public void Back()
    {
        if (DataManagement.DataManager.Instance.userData.progressData.playCount >= Game.Controller.Instance.gameConfig.adConfig.adStart)
        {
            AD.Controller.Instance.ShowInterstitial(() =>
            {
                BackHome();

            });
        }
        else
        {
            BackHome();
        }


        void BackHome()
        {
            if (isProcessing) return;
            isProcessing = true;
            LevelLoading.Instance.Active(() =>
            {
                if (DataManagement.DataManager.Instance.userData.progressData.collectionDatas.Count > 0)
                {
                    Game.Controller.Instance.gameController.Destroy();
                }
                ((StageGameController)Game.Controller.Instance.gameController).SetUp();
                Close();
                //LevelLoading.Instance.Close();
            });
        }
    }
}
