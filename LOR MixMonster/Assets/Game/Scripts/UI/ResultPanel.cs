using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JetBrains.Annotations;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.IO;
using Spine.Unity.Examples;
using GameUtility;
using DataManagement;
using System.Threading.Tasks;

public class ResultPanel : UI.Panel
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private Image screenShotImg;
    [SerializeField]
    private RectTransform iconGold, boxGold, cusorGameObject;
    [SerializeField]
    private List<Transform> _listTransform;
    bool isProcessing = false;
    [SerializeField]
    private GameObject hand, bestObj;
    [SerializeField]
    private MonsterCard monsterCard;
    [SerializeField]
    private CanvasGroup panelCard;
    [SerializeField]
    private Button claim, thanks, card;
    [SerializeField]
    private AudioClip finishSFX, barMove, coinSFX1, coinSFX2, selectCard, removeCard;
    [SerializeField]
    private TMPro.TextMeshProUGUI likeText, goldText, goldReceivedText, claimWithAdsText;

    private int gold, changeGold, max, xLucky;
    bool isDone;
    private RectTransform[] coin;
    CancellationTokenSource cancellation;
    DataManagement.CollectionData data;
    
    private void OnEnable()
    {
        cancellation = new CancellationTokenSource();
    }
    public override void OnDestroy()
    {
        DataManager.Instance.userData.inventory.onUpdate -= OnCollectionUpdated;
        base.OnDestroy();
        
    }
    private void OnDisable()
    {
        DataManager.Instance.userData.inventory.onUpdate -= OnCollectionUpdated;
    }
    public override void PostInit()
    {
    }

    public void SetUp(int changeGold,int totalViewPoint, int totalLikePoint, Sprite screenShot, List<ItemData.Item> mySets)
    {
        isDone = true;
        xLucky = 5;
        screenShotImg.sprite = screenShot;
        Texture2D rawImageTexture = screenShot.texture;
        byte[] bytes = rawImageTexture.EncodeToJPG(50); // Chuyển texture thành dãy byte JPG
        string filePath = Application.persistentDataPath + "/" + (DataManagement.DataManager.Instance.userData.progressData.collectionDatas.Count + 1).ToString() + ".jpg";
        File.WriteAllBytes(filePath, bytes);
        //screenShotImg.SetNativeSize();
        //DOTween.To(() => panelCard.alpha, x => panelCard.alpha = x, 1, 0.3f);
        //hand.SetActive(DataManagement.DataManager.Instance.userData.stageListData.stageDatas.Count == 0);
        panelCard.interactable = true;
        claim.interactable = false;
        thanks.interactable = false;
        card.interactable = false;
        this.changeGold = changeGold;
        if (totalViewPoint > DataManagement.DataManager.Instance.userData.progressData.bestViewPoint)
        {
            bestObj.SetActive(true);
            DataManagement.DataManager.Instance.userData.progressData.bestViewPoint = totalViewPoint;
            DataManagement.DataManager.Instance.Save();
        }
        else
        {
            bestObj.SetActive(false);
        }
        hand.SetActive(DataManager.Instance.userData.stageListData.stageDatas.Count == 0 || DataManager.Instance.userData.stageListData.stageDatas[0].stageCollections.Count == 0);
        data = new DataManagement.CollectionData(totalViewPoint, totalLikePoint, DataManagement.DataManager.Instance.userData.progressData.collectionDatas.Count + 1, mySets);
        DataManagement.DataManager.Instance.userData.progressData.collectionDatas.Add(data);
        DataManagement.DataManager.Instance.Save();
        gold = DataManagement.DataManager.Instance.userData.YourGold;
        max = Sheet.SheetDataManager.Instance.gameData.rewardBarConfig.views[Sheet.SheetDataManager.Instance.gameData.rewardBarConfig.views.Length - 1];
        likeText.text = "0m";
        Effect(totalLikePoint);
        goldText.text = gold.ToString();
        likeText.text = GameUtility.GameUtility.ShortenNumber(totalLikePoint);
        monsterCard.gameObject.SetActive(true);
        DataManager.Instance.userData.inventory.onUpdate += OnCollectionUpdated;
        OnCollectionUpdated(DataManager.Instance.userData.inventory);
        Show();
    }
    public async UniTaskVoid Effect(int totalLikePoint)
    {
        await UniTask.Delay(660, cancellationToken: cancellation.Token);
        Sound.Controller.Instance.PlayOneShot(barMove);
        /*for (int i = 0; i < Sheet.SheetDataManager.Instance.gameData.rewardGold.item.Count; i++)
        {
            if (totalLikePoint >= Sheet.SheetDataManager.Instance.gameData.rewardGold.item[i].like && totalLikePoint < Sheet.SheetDataManager.Instance.gameData.rewardGold.item[i + 1].like)
            {
                changeGold = (int)(Sheet.SheetDataManager.Instance.gameData.rewardGold.item[i].gold.GetRandomInt() * UnityEngine.Random.Range(1.7f, 2f));
            }
        }*/
        int a = 0;
        goldReceivedText.transform.DOScale(Vector3.one * 1.3f, 1f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
        DOTween.To(() => a, x => a = x, changeGold, 2f).SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                goldReceivedText.text = a.ToString();
            });
        await UniTask.Delay(2000, cancellationToken: cancellation.Token);
        DataManager.Instance.userData.inventory.onUpdate += OnCollectionUpdated;
        coin = CoinPooler.instance.GetPoolCoin();
        isDone = false;
        claim.interactable = true;
        thanks.interactable = true;
        card.interactable = true;
    }
    private void OnCollectionUpdated(Inventory inventory)
    {
        Debug.Log("SET COLLECTION");
        CardData cardData = DataManager.Instance.userData.inventory.GetFirstCollection();
        if (cardData != null)
        {
            Debug.Log("SET COLLECTION " + cardData.id);
            monsterCard.SetUp(cardData);
        }
        else
        {
            hand.SetActive(false);
            monsterCard.monster.gameObject.SetActive(false);
            monsterCard.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (isDone) return;

        cusorGameObject.Translate(new Vector3(speed, 0f, 0f) * Time.deltaTime);
        if (cusorGameObject.position.x <= _listTransform[0].position.x)
        {
            speed = System.Math.Abs(speed);
        }
        else if (cusorGameObject.position.x >= _listTransform[_listTransform.Count - 1].position.x)
        {
            speed = -System.Math.Abs(speed);
        }

        for (int i = _listTransform.Count - 2; i >= 0; i--)
        {
            if (cusorGameObject.position.x >= _listTransform[i].position.x)
            {
                switch (i)
                {
                    case 0:
                        xLucky = 2;
                        break;
                    case 1:
                        xLucky = 3;
                        break;
                    case 2:
                        xLucky = 4;
                        break;
                    case 3:
                        xLucky = 5;
                        break;
                    case 4:
                        xLucky = 4;
                        break;
                    case 5:
                        xLucky = 3;
                        break;
                    case 6:
                        xLucky = 2;
                        break;
                    default:
                        xLucky = 10;
                        break;
                }

                ;
                break;
            }
        }

        goldReceivedText.text = (changeGold * xLucky).ToString();
        claimWithAdsText.text = "CLAIM X" + xLucky.ToString();
    }
    public async void Claim()
    {
        if (isProcessing) return;
        isProcessing = true;
        isDone = true;
        AD.Controller.Instance.ShowRewardedAd("ClaimGold", res =>
        {
            if (res)
            {
                isDone = true;
                ClaimGold();
            }
            else
            {
                isProcessing = false;
            }
        });

        async void ClaimGold()
        {
            for (int i = 0; i < 5; i++)
            {
                if (coin[i] != null)
                {
                    coin[i].localScale = Vector3.zero;
                    coin[i].position = boxGold.position;
                    coin[i].gameObject.SetActive(true);
                    coin[i].DOAnchorPos(new Vector3(boxGold.position.x + UnityEngine.Random.Range(-80f, 80f), boxGold.position.y + UnityEngine.Random.Range(-100f, 60f), 1), 0.3f).SetEase(Ease.InOutQuad);
                    Sound.Controller.Instance.PlayOneShot(coinSFX1);
                    coin[i].transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad);
                }
            }
            await UniTask.Delay(600, cancellationToken: cancellation.Token);
            for (int i = 0; i < 5; i++)
            {
                if (coin[i] != null)
                {
                    coin[i].transform.DOMove(iconGold.position, 0.8f).SetEase(Ease.OutSine).OnComplete(() =>
                    {
                        Sound.Controller.Instance.PlayOneShot(coinSFX2);
                    });
                }
            }
            await UniTask.Delay(800, cancellationToken: cancellation.Token);
            for (int i = 0; i < 5; i++)
            {
                if (coin[i] != null)
                {
                    coin[i].gameObject.SetActive(false);
                }
            }
            goldText.transform.DOScale(Vector3.one * 1.3f, 0.5f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
            DOTween.To(() => gold, x => gold = x, DataManagement.DataManager.Instance.userData.YourGold + changeGold * xLucky, 1f).SetEase(Ease.OutQuad).OnUpdate(() =>
            {
                goldText.text = gold.ToString();
            }).OnComplete(() =>
            {
                DataManagement.DataManager.Instance.userData.YourGold = gold;
                DataManagement.DataManager.Instance.Save();
                Sound.Controller.Instance.PlayOneShot(finishSFX);
            });
            await UniTask.Delay(1800, cancellationToken: cancellation.Token);
            LoadLevel();
        }
    }
    public void Thank()
    {
        if (isProcessing) return;
        isProcessing = true;

        if (DataManagement.DataManager.Instance.userData.progressData.playCount >= Game.Controller.Instance.gameConfig.adConfig.adStart)
        {
            AD.Controller.Instance.ShowInterstitial(() =>
            {
                ReceiveGold();

            });
        }
        else
        {
            ReceiveGold();
        }
        async void ReceiveGold()
        {
            for (int i = 0; i < 5; i++)
            {
                if (coin[i] != null)
                {
                    coin[i].localScale = Vector3.zero;
                    coin[i].position = boxGold.position;
                    coin[i].gameObject.SetActive(true);
                    coin[i].DOAnchorPos(new Vector3(boxGold.position.x + UnityEngine.Random.Range(-80f, 80f), boxGold.position.y + UnityEngine.Random.Range(-100f, 60f), 1), 0.3f).SetEase(Ease.InOutQuad);
                    Sound.Controller.Instance.PlayOneShot(coinSFX1);
                    coin[i].transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad);
                }
            }
            await UniTask.Delay(600, cancellationToken: cancellation.Token);
            for (int i = 0; i < 5; i++)
            {
                if (coin[i] != null)
                {
                    coin[i].transform.DOMove(iconGold.position, 0.8f).SetEase(Ease.OutSine).OnComplete(() =>
                    {
                        Sound.Controller.Instance.PlayOneShot(coinSFX2);
                    });
                }
            }
            await UniTask.Delay(800, cancellationToken: cancellation.Token);
            for (int i = 0; i < 5; i++)
            {
                if (coin[i] != null)
                {
                    coin[i].gameObject.SetActive(false);
                }
            }
            goldText.transform.DOScale(Vector3.one * 1.3f, 0.5f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
            DOTween.To(() => gold, x => gold = x, DataManagement.DataManager.Instance.userData.YourGold + changeGold, 1f).SetEase(Ease.OutQuad).OnUpdate(() =>
            {
                goldText.text = gold.ToString();
            }).OnComplete(() =>
            {
                DataManagement.DataManager.Instance.userData.YourGold = gold;
                DataManagement.DataManager.Instance.Save();
                Sound.Controller.Instance.PlayOneShot(finishSFX);
            });
            await UniTask.Delay(1800, cancellationToken: cancellation.Token);
            LoadLevel();
        }
    }
    void LoadLevel()
    {
        LevelLoading.Instance.Active(() =>
        {
            Close();
            if (DataManagement.DataManager.Instance.userData.progressData.firstDaily < 2)
            {
                DataManagement.DataManager.Instance.userData.progressData.firstDaily += 1;
                DataManagement.DataManager.Instance.Save();
            }
            monsterCard.monster.gameObject.SetActive(false);
            Game.Controller.Instance.gameController.SetUp();
        });
    }
    public void Home()
    {
        bool isTut = hand.activeSelf;
        if (isProcessing) return;
        isProcessing = true;
        if (!isTut)
        {
            AD.Controller.Instance.ShowInterstitial(() =>
            {
                LoadSceneAsync();
            });
        }
        else
        {
            LoadSceneAsync();
        }


        async Task LoadSceneAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                if (coin[i] != null)
                {
                    coin[i].localScale = Vector3.zero;
                    coin[i].position = boxGold.position;
                    coin[i].gameObject.SetActive(true);
                    coin[i].DOAnchorPos(new Vector3(boxGold.position.x + UnityEngine.Random.Range(-80f, 80f), boxGold.position.y + UnityEngine.Random.Range(-100f, 60f), 1), 0.3f).SetEase(Ease.InOutQuad);
                    Sound.Controller.Instance.PlayOneShot(coinSFX1);
                    coin[i].transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad);
                }
            }
            await UniTask.Delay(600, cancellationToken: cancellation.Token);
            for (int i = 0; i < 5; i++)
            {
                if (coin[i] != null)
                {
                    coin[i].transform.DOMove(iconGold.position, 0.8f).SetEase(Ease.OutSine).OnComplete(() =>
                    {
                        Sound.Controller.Instance.PlayOneShot(coinSFX2);
                    });
                }
            }
            await UniTask.Delay(800, cancellationToken: cancellation.Token);
            for (int i = 0; i < 5; i++)
            {
                if (coin[i] != null)
                {
                    coin[i].gameObject.SetActive(false);
                }
            }
            goldText.transform.DOScale(Vector3.one * 1.3f, 0.5f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
            DOTween.To(() => gold, x => gold = x, DataManagement.DataManager.Instance.userData.YourGold + changeGold, 1f).SetEase(Ease.OutQuad).OnUpdate(() =>
            {
                goldText.text = gold.ToString();
            }).OnComplete(() =>
            {
                DataManagement.DataManager.Instance.userData.YourGold = gold;
                DataManagement.DataManager.Instance.Save();
                Sound.Controller.Instance.PlayOneShot(finishSFX);
            });
            await UniTask.Delay(1800, cancellationToken: cancellation.Token);
            LevelLoading.Instance.Active("HomeScene", null
             ,async () =>
             {
                 await Game.Controller.Instance.gameController.SetUp();

             }
         , closeOverride: true);
        }
    }
}
