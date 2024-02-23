using Cysharp.Threading.Tasks;
using DG.Tweening;
using ItemData;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using MoreMountains.NiceVibrations;
using UnityEngine.Networking.Types;
using UnityEngine.UI;
using static DataManagement.MergeSlotData;

public abstract class MakeOverPanelAbstract : UI.Panel
{
    public bool isProcessing = false;
    private int _monsterId;

    [SerializeField]
    protected ItemSelectButton[] itemSelectButtons;

    protected int currentCategory;
    [SerializeField]
    protected ItemData.Category[] categoryOrder;
    protected TabHandler tabHandler;
    [SerializeField]
    protected GameUtility.Pooling.PoolHandler petSlotPools;
    protected List<ItemData.Item> defaultItems = new List<ItemData.Item>();

    protected List<ItemData.Item> selectedItems = new List<ItemData.Item>();
    protected List<ItemData.Item> readyPool = new List<ItemData.Item>();
    protected List<ItemData.Item> petReadyPool = new List<ItemData.Item>();
    
    [SerializeField]
    protected AudioClip[] leftRightClips;
    [SerializeField]
    protected AudioClip dailyShow, sfx;
    [SerializeField]
    protected HapticTypes hapticTypes = HapticTypes.Warning;
    [SerializeField]
    protected GameObject handTut, chooseTut, homeBtn, bundleBtn, petOfferBtn, petOfferAdObj, newOptionBtn;
    public GameObject removeAds;
    [SerializeField]
    protected Image tryPetIcon, petIcon, optionIcon;
    [SerializeField]
    protected Sprite[] categoryIcon;
    [SerializeField]
    protected Button buyPet, buyOption;
    [SerializeField]
    protected Animator animTryPet, animTryOption;
    [SerializeField]
    protected TextMeshProUGUI goldText, tryPetText;
    protected int gold;
    protected CancellationTokenSource cancellation;

    private bool hapticsAllowed = true;
    private string bundleId;
    System.Action onUnlock;

    public static List<ItemData.Item> previousFirstSpawnItems = new List<ItemData.Item>();
    public static List<ItemData.Item> excludeItems = new List<ItemData.Item>();
    public List<ItemData.Item> excludeItems2 = new List<ItemData.Item>();
    public List<ItemData.Item> mySet = new List<ItemData.Item>();
    [SerializeField]
    protected Sprite[] backGroundSprites;
    [SerializeField]
    protected Image backGroundImg;
    protected void OnEnable()
    {
        cancellation = new CancellationTokenSource();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
        }
    }
    private void OnDisable()
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
        }
    }
    public override void PostInit()
    {
        tabHandler = GetComponentInChildren<TabHandler>();
    }
    public void SetCategory(int index)
    {
        tabHandler.SetTab(index);
        currentCategory = index;
        if (ads.Contains((int)currentCategory))
        {
            Game.Controller.Instance.itemData.GetPack(categoryOrder[index]).PrepareItemPool_MixedAd(readyPool, excludeItems,  defaultItems);
        }
        else
        {
            Game.Controller.Instance.itemData.GetPack(categoryOrder[index]).PrepareItemPool(readyPool, excludeItems,  defaultItems);
        }
        if (readyPool.Count % 2 != 0)
        {
            readyPool.RemoveAt(readyPool.Count - 1);
        }
        SetItem();

        switch ((ItemData.Category)currentCategory)
        {
            case ItemData.Category.Accessory:
            case ItemData.Category.Eye:
                {
                    petReadyPool[0].GetIcon(sprite =>
                    {
                        petIcon.sprite = sprite;
                    });
                    tryPetText.text = petReadyPool[0].cost.ToString();
                    petOfferAdObj.SetActive(!DataManagement.DataManager.Instance.userData.progressData.firstPet);
                    petOfferBtn.SetActive(true);
                    break;
                }
            default:
                petOfferBtn.SetActive(false);
                break;
        }

        newOptionBtn.SetActive(true);
        getNewPairCount = 0;
    }
    List<int> ads = new List<int>();

    public virtual async UniTask SetUp()
    {
        backGroundImg.sprite = backGroundSprites[DataManagement.DataManager.Instance.userData.progressData.playCount % backGroundSprites.Length];
        bundleBtn.SetActive(DataManagement.DataManager.Instance.userData.inventory.GetItemState("SetBundle_1") == 0);
        homeBtn.SetActive(DataManagement.DataManager.Instance.userData.inventory.cards.Count != 0);
        removeAds.SetActive(DataManagement.DataManager.Instance.userData.IsAd);
        excludeItems.Clear();
        excludeItems.AddRange(previousFirstSpawnItems);
        previousFirstSpawnItems.Clear();
        MMVibrationManager.SetHapticsActive(hapticsAllowed);

#if UNITY_EDITOR
        excludeItems2 = excludeItems;
#endif

        //
        if (DataManagement.DataManager.Instance.userData.progressData.firstSelect)
        {
            ads.Clear();
            ads.Add(3);
            ads.Add(4);
        }
        else
        {
            int adCount = 3;
            ads.Clear();
            while (adCount > 0)
            {
                int rd = 0;
                do
                {
                    rd = UnityEngine.Random.Range(0, 5);
                } while (ads.Contains(rd));
                ads.Add(rd);
                adCount--;
            }
        }

        isProcessing = true;
        selectedItems.Clear();
        mySet.Clear();
        foreach (ItemSelectButton button in itemSelectButtons)
        {
            button.gameObject.SetActive(false);
        }

        
        Show();
        await UniTask.Delay(1000, cancellationToken: cancellation.Token);
        SetCategory(0);
        if (DataManagement.DataManager.Instance.userData.progressData.firstDaily == 1)
        {
            UI.PanelManager.Create(typeof(DailyRewards), (panel, op) =>
            {
                Sound.Controller.Instance.PlayOneShot(dailyShow);
                ((DailyRewards)panel).SetUp();
            });
        }

    }
    private void Update()
    {
        if (DataManagement.DataManager.Instance.userData.IsAd)
        {
            removeAds.SetActive(true);
        }
        else
        {
            removeAds.SetActive(false);
        }
        if ((Game.Controller.Instance.gameController).updateGold)
        {
            goldText.text = DataManagement.DataManager.Instance.userData.YourGold.ToString();
            (Game.Controller.Instance.gameController).updateGold = false;
        }
    }
    async UniTaskVoid Introduce(int count)
    {
        var bundleSet = Sheet.SheetDataManager.Instance.gameData.itemData.GetBundle("SetBundle_3");

        /*foreach (ModelSet modelSet in bundleSet.modelSets)
        {
            foreach (string itemId in modelSet.itemIds)
            {
                DataManagement.DataManager.Instance.userData.inventory.SetItemState(itemId, 1);
            }
        }*/
        ModelSet modelSet = bundleSet.modelSets[count];

        for (int j = 0; j < modelSet.itemIds.Count; j++)
        {
            string itemId = modelSet.itemIds[j];
            DataManagement.DataManager.Instance.userData.inventory.SetItemState(itemId, 1);
        }
        //DataManagement.DataManager.Instance.userData.inventory.SetItemState(bundleId, 3);
        onUnlock?.Invoke();


        List<ItemData.Item> rewardItems = new List<ItemData.Item>();
        /*foreach (ItemData.ModelSet modelSet in bundleSet.modelSets)
        {
            rewardItems.Clear();
            foreach (string itemId in modelSet.itemIds)
            {
                rewardItems.Add(Sheet.SheetDataManager.Instance.gameData.itemData.GetItem(itemId));
            }
            IntroducMonsterPanel introducMonsterPanel = (IntroducMonsterPanel)await UI.PanelManager.CreateAsync(typeof(IntroducMonsterPanel));
            introducMonsterPanel.SetUp(rewardItems);

            await UniTask.WaitUntil(() => introducMonsterPanel == null);
        }*/
        rewardItems.Clear();

        for (int j = 0; j < modelSet.itemIds.Count; j++)
        {
            string itemId = modelSet.itemIds[j];
            rewardItems.Add(Sheet.SheetDataManager.Instance.gameData.itemData.GetItem(itemId));
        }
            
        IntroducMonsterPanel introducMonsterPanel = (IntroducMonsterPanel)await UI.PanelManager.CreateAsync(typeof(IntroducMonsterPanel));
        introducMonsterPanel.SetUp(rewardItems);

        await UniTask.WaitUntil(() => introducMonsterPanel == null);

    }
    List<UniTask> tasks = new List<UniTask>();
    public virtual async UniTaskVoid SetItem()
    {
        isProcessing = true;
        for (int i = 0; i < itemSelectButtons.Length; i++)
        {
            ItemSelectButton button = itemSelectButtons[i];
            button.Hide();
            Effect.EffectSpawner.Instance.Get(2, effect =>
            {
                effect.Active(button.transform.position);
            }).Forget();
        }
        await UniTask.Delay(500, cancellationToken: cancellation.Token);
        tasks.Clear();
        for (int i = 0; i < itemSelectButtons.Length; i++)
        {
            tasks.Add(itemSelectButtons[i].SetUp(readyPool[i], this));
        }
        await UniTask.WhenAll(tasks);

        for (int i = 0; i < itemSelectButtons.Length; i++)
        {
            itemSelectButtons[i].HighLight();
            Sound.Controller.Instance.PlayOneShot(leftRightClips[i]);
            await UniTask.Delay(400, cancellationToken: cancellation.Token);
        }
        handTut.SetActive(DataManagement.DataManager.Instance.userData.progressData.firstSelect);
        DataManagement.DataManager.Instance.userData.progressData.firstSelect = false;

        DataManagement.DataManager.Instance.Save();
        isProcessing = false;

    }
    public void ShowBundle()
    {
        if (isProcessing) return;
        isProcessing = true;
        Sound.Controller.Instance.PlayOneShot(sfx);
        UI.PanelManager.Create(typeof(SetBundlePanel), (panel, op) =>
        {
            isProcessing = false;
            ((SetBundlePanel)panel).SetUp("SetBundle_1", onUnlock: () =>
            {
                bundleBtn.SetActive(false);
            });
        });
    }
    public void ShowPetOffer()
    {
        if (isProcessing) return;
        isProcessing = true;
        if (DataManagement.DataManager.Instance.userData.progressData.firstPet)
        {
            ItemData.Item offererPet = petReadyPool[0];
            if (!DataManagement.DataManager.Instance.userData.progressData.firstPet)
            {
                FirebaseAnalysticController.Instance.LogEvent($"CollectPet_{offererPet.id}");
                FirebaseAnalysticController.Instance.LogEvent($"ADS_REWARD_START_COLLECTPET");

            }

            DataManagement.DataManager.Instance.userData.progressData.firstPet = false;
            DataManagement.DataManager.Instance.Save();

            petOfferBtn.SetActive(false);

            selectedItems.Add(offererPet);
            petReadyPool.RemoveAt(0);
            ObjectSpawner.Instance.Get(0, obj =>
            {
                ItemOrb itemOrb = obj.GetComponent<ItemOrb>();
                itemOrb.SetUp(petIcon.sprite, petIcon.transform, petSlotPools.transform, 8, res =>
                {
                    petSlotPools.Get().GetComponent<PetSlot>().SetUp(offererPet);
                    //
                    Effect.EffectSpawner.Instance.Get(2, effect =>
                    {
                        effect.Active(petSlotPools.transform.position);
                    }).Forget();
                });
            }).Forget();
        }
        else
        {
            tryPetIcon.sprite = petIcon.sprite;
            if (gold >= 1500)
            {
                buyPet.interactable = true;
            }
            else
            {
                buyPet.interactable = false;
            }
            animTryPet.gameObject.SetActive(true);
        }
        isProcessing = false;
    }
    public void AdsPet()
    {
        AD.Controller.Instance.ShowRewardedAd("Pet", res =>
        {
            if (res)
            {
                ItemData.Item offererPet = petReadyPool[0];
                if (!DataManagement.DataManager.Instance.userData.progressData.firstPet)
                {
                    FirebaseAnalysticController.Instance.LogEvent($"CollectPet_{offererPet.id}");
                    FirebaseAnalysticController.Instance.LogEvent($"ADS_REWARD_START_COLLECTPET");

                }

                petOfferBtn.SetActive(false);

                selectedItems.Add(offererPet);
                petReadyPool.RemoveAt(0);
                ObjectSpawner.Instance.Get(0, obj =>
                {
                    ItemOrb itemOrb = obj.GetComponent<ItemOrb>();
                    itemOrb.SetUp(petIcon.sprite, petIcon.transform, petSlotPools.transform, 8, res =>
                    {
                        petSlotPools.Get().GetComponent<PetSlot>().SetUp(offererPet);
                        //
                        Effect.EffectSpawner.Instance.Get(2, effect =>
                        {
                            effect.Active(petSlotPools.transform.position);
                        }).Forget();
                    });
                }).Forget();

                animTryPet.SetTrigger("close");

            }
        }, canSkip: DataManagement.DataManager.Instance.userData.progressData.firstPet);
    }
    public void BuyPet()
    {
        ItemData.Item offererPet = petReadyPool[0];
        if (!DataManagement.DataManager.Instance.userData.progressData.firstPet)
        {
            FirebaseAnalysticController.Instance.LogEvent($"CollectPet_{offererPet.id}");
        }

        DataManagement.DataManager.Instance.userData.progressData.firstPet = false;
        DataManagement.DataManager.Instance.Save();

        petOfferBtn.SetActive(false);

        selectedItems.Add(offererPet);
        petReadyPool.RemoveAt(0);
        ObjectSpawner.Instance.Get(0, obj =>
        {
            ItemOrb itemOrb = obj.GetComponent<ItemOrb>();
            itemOrb.SetUp(petIcon.sprite, petIcon.transform, petSlotPools.transform, 8, res =>
            {
                petSlotPools.Get().GetComponent<PetSlot>().SetUp(offererPet);
                //
                Effect.EffectSpawner.Instance.Get(2, effect =>
                {
                    effect.Active(petSlotPools.transform.position);
                }).Forget();
            });
        }).Forget();
        goldText.transform.DOScale(Vector3.one * 1.3f, 0.3f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
        DOTween.To(() => gold, x => gold = x, DataManagement.DataManager.Instance.userData.YourGold - 1500, 1f).SetEase(Ease.OutQuad).OnUpdate(() =>
        {
            goldText.text = gold.ToString();
        }).OnComplete(() =>
        {
            DataManagement.DataManager.Instance.userData.YourGold = gold;
            DataManagement.DataManager.Instance.Save();
        });
        animTryPet.SetTrigger("close");
    }

    //watch ad and set a new pair of item
    float adTime = 0;
    int getNewPairCount = 0;
    public void GetNewPair()
    {
        if (isProcessing) return;
        isProcessing = true;
        if (gold >= 500)
        {
            buyOption.interactable = true;
        }
        else
        {
            buyOption.interactable=false;
        }
        optionIcon.sprite = categoryIcon[(int)currentCategory];
        animTryOption.gameObject.SetActive(true);
        isProcessing = false;
    }
    public void OptionGold()
    {
        if (Time.time - adTime < 2) return;
        adTime = Time.time;
        getNewPairCount++;
        if (getNewPairCount >= Game.Controller.Instance.gameConfig.newItemMax)
        {
            newOptionBtn.SetActive(false);
        }
        readyPool.RemoveRange(0, 2);

        if (readyPool.Count == 0)
        {
            if (ads.Contains((int)currentCategory))
            {
                Game.Controller.Instance.itemData.GetPack((ItemData.Category)currentCategory).PrepareItemPool_MixedAd(readyPool, excludeItems, null);
            }
            else
            {
                Game.Controller.Instance.itemData.GetPack((ItemData.Category)currentCategory).PrepareItemPool(readyPool, excludeItems, null);
            }
        }
        if (readyPool.Count % 2 != 0)
        {
            readyPool.RemoveAt(readyPool.Count - 1);
        }
        SetItem();
        goldText.transform.DOScale(Vector3.one * 1.3f, 0.3f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
        DOTween.To(() => gold, x => gold = x, DataManagement.DataManager.Instance.userData.YourGold - 500, 1f).SetEase(Ease.OutQuad).OnUpdate(() =>
        {
            goldText.text = gold.ToString();
        }).OnComplete(() =>
        {
            DataManagement.DataManager.Instance.userData.YourGold = gold;
            DataManagement.DataManager.Instance.Save();
        });
        animTryOption.SetTrigger("close");
    }

    public void OptionAds()
    {
        if (isProcessing || Time.time - adTime < 2) return;
        isProcessing = true;
        adTime = Time.time;
        AD.Controller.Instance.ShowRewardedAd("NewOptions", res =>
        {
            if (res)
            {
                getNewPairCount++;
                if (getNewPairCount >= Game.Controller.Instance.gameConfig.newItemMax)
                {
                    newOptionBtn.SetActive(false);
                }
                readyPool.RemoveRange(0, 2);

                if (readyPool.Count == 0)
                {
                    if (ads.Contains((int)currentCategory))
                    {
                        Game.Controller.Instance.itemData.GetPack((ItemData.Category)currentCategory).PrepareItemPool_MixedAd(readyPool, excludeItems, null);
                    }
                    else
                    {
                        Game.Controller.Instance.itemData.GetPack((ItemData.Category)currentCategory).PrepareItemPool(readyPool, excludeItems, null);
                    }
                }
                if (readyPool.Count % 2 != 0)
                {
                    readyPool.RemoveAt(readyPool.Count - 1);
                }
                SetItem();
                animTryOption.SetTrigger("close");
            }
            else
            {
                isProcessing = false;
            }
        });
    }
    public virtual async UniTask<bool> OnSelect(ItemSelectButton itemSelectButton)
    {
        if (isProcessing) return false;
        isProcessing = true;
        handTut.SetActive(false);
        selectedItems.Add(itemSelectButton.item);
        mySet.Add(itemSelectButton.item);

        foreach (ItemSelectButton button in itemSelectButtons)
        {
            previousFirstSpawnItems.Add(button.item);
        }

        if (petOfferBtn.activeSelf)
        {
            petReadyPool.RemoveAt(0);
            petOfferBtn.SetActive(false);
        }
        foreach (ItemSelectButton button in itemSelectButtons)
        {
            button.Hide();
        }
        ((MakeOverGameController)Game.Controller.Instance.gameController).OnNewItemSelected(itemSelectButton.item, itemSelectButton);
        await UniTask.Delay(1000, cancellationToken: cancellation.Token);
        //finish the last part, show live stream page
        if (currentCategory >= categoryOrder.Length - 1)
        {
            Close();
            ((MakeOverGameController)Game.Controller.Instance.gameController).FinishMakeOver(selectedItems, mySet);
        }
        //roll the the next part
        else
        {
            SetCategory((currentCategory + 1));
        }
        return true;
    }

    public void Home()
    {
        if (isProcessing) return;
        isProcessing = true;
        AD.Controller.Instance.ShowInterstitial(() =>
        {
            Sound.Controller.Instance.PlayOneShot(sfx);
            LevelLoading.Instance.Active("HomeScene",
                 () =>
                 {
                     Game.Controller.Instance.gameController.Destroy();
                 },
                 async () =>
                 {
                     await Game.Controller.Instance.gameController.SetUp();

                 }
             , closeOverride: true);
        });

    }
}
