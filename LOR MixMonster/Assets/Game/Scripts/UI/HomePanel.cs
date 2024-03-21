using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DailyReward;
using DataManagement;
using GameUtility;
using Spine.Unity.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static DataManagement.Inventory;

public class HomePanel : UI.Panel
{
    bool isProcessing = false;
    [SerializeField]
    private AudioClip playSFX, cameraClip;
    [SerializeField]
    private TextMeshProUGUI cashText, totalEarningText, slotText;
    public TextMeshProUGUI goldText, timeNoAds;
    [SerializeField]
    private RectTransform slotTotalRect;
    [SerializeField]
    private GameObject maxSlotNotice, controlPanel, deletePanel, bundleBtn, viewBtn, handTut, boxBanner;
    public GameObject removeAds, notifyOffline;
    public Button blockAds;
    [SerializeField]
    private GameObject[] uiHome;
    [SerializeField]
    private Image view;
    [SerializeField]
    private Sprite uiOff, uiOn;
    public MonsterCard monsterCard;
    [SerializeField]
    private BoosterButton[] boosterButtons;
    [SerializeField]
    private ParticleSystem unlockPS, newMonsterPS;
    private bool hideHand;
    float time = 0;
    StageData stageData;
    public override void PostInit()
    {
    }
    Effect.EffectAbstract effect;
    public void SetUp()
    {
        Sound.Controller.Instance.PlayMusic(Sound.Controller.Instance.soundData.menuTheme[UnityEngine.Random.Range(0, Sound.Controller.Instance.soundData.menuTheme.Length)]);

        isProcessing = false;
        DataManagement.DataManager.Instance.userData.inventory.onUpdate += OnCollectionUpdated;
        DataManagement.DataManager.Instance.userData.inventory.onCashUpdated += OnCashUpdated;
        OnCollectionUpdated(DataManager.Instance.userData.inventory);
        OnEarningUpdated(((StageGameController)Game.Controller.Instance.gameController).GetTotalEarning());
        StageData currentStageData = ((StageGameController)Game.Controller.Instance.gameController).GetStageHandler().stageData;
        OnMonsterSlotUpdated(currentStageData.stageCollections.Count, currentStageData.totalMonsterSlot);
        bundleBtn.SetActive(DataManagement.DataManager.Instance.userData.inventory.GetItemState("SetBundle_1") == 0);
        removeAds.SetActive(DataManagement.DataManager.Instance.userData.IsAd || DataManagement.DataManager.Instance.userData.stageListData.isNoAds);
        blockAds.gameObject.SetActive(DataManagement.DataManager.Instance.userData.IsAd || DataManagement.DataManager.Instance.userData.stageListData.isNoAds);
        for (int i = 0; i < boosterButtons.Length; i++)
        {
            boosterButtons[i].SetUp(((StageGameController)Game.Controller.Instance.gameController).boosters[i]);
        }
        if (DataManagement.DataManager.Instance.userData.progressData.uiHome)
        {
            view.sprite = uiOff;
            for (int i = 0; i < uiHome.Length; i++)
            {
                uiHome[i].SetActive(true);
                boxBanner.SetActive(false);
            }
            if (DataManagement.DataManager.Instance.userData.inventory.GetItemState("SetBundle_1") == 0)
            {
                bundleBtn.SetActive(true);
            }
        }
        else
        {
            view.sprite = uiOn;
            for (int i = 0; i < uiHome.Length; i++)
            {
                uiHome[i].SetActive(false);
                boxBanner.SetActive(true);
            }
            if (DataManagement.DataManager.Instance.userData.inventory.GetItemState("SetBundle_1") == 0)
            {
                bundleBtn.SetActive(false);
            }
        }
        if (((StageGameController)Game.Controller.Instance.gameController).FIRST_NOADS == 1 && DataManagement.DataManager.Instance.userData.IsAd)
        {
            UI.PanelManager.Create(typeof(NoAdsPanel), (panel, op) =>
            {
                ((NoAdsPanel)panel).SetUp(onUnlock: () =>
                {
                    blockAds.interactable = false;
                    timeNoAds.gameObject.SetActive(true);
                });
                ((StageGameController)Game.Controller.Instance.gameController).hideMonster = true;
            });
        }
        if (((StageGameController)Game.Controller.Instance.gameController).FIRST_NOADS < 2)
        {
            ((StageGameController)Game.Controller.Instance.gameController).FIRST_NOADS++;
        }
        if (DataManagement.DataManager.Instance.userData.stageListData.isNoAds)
        {
            SetCoolDown(() => { ReloadAds(); });
        }
        else
        {
            blockAds.interactable = true;
            timeNoAds.gameObject.SetActive(false);
        }
        hideHand = DataManagement.DataManager.Instance.userData.inventory.GetFirstCollection() != null && (DataManagement.DataManager.Instance.userData.stageListData.stageDatas.Count == 0 || DataManagement.DataManager.Instance.userData.stageListData.stageDatas[0].stageCollections.Count == 0);
        handTut.SetActive(DataManagement.DataManager.Instance.userData.inventory.GetFirstCollection() != null && (DataManagement.DataManager.Instance.userData.stageListData.stageDatas.Count == 0 || DataManagement.DataManager.Instance.userData.stageListData.stageDatas[0].stageCollections.Count == 0));
        goldText.text = DataManagement.DataManager.Instance.userData.YourGold.ToString();
        cashText.text = "$" + GameUtility.GameUtility.ShortenNumber(DataManagement.DataManager.Instance.userData.inventory.cash);
        Show();
        DataManager.Instance.Save();
    }
    void Update()
    {
        if (Time.time - time > 1 && DataManagement.DataManager.Instance.userData.stageListData.isNoAds && System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.progressData.timeNoAds)).TotalSeconds < DataManagement.DataManager.Instance.userData.stageListData.noAdsTime)
        {
            var span = System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.progressData.timeNoAds));
            span = TimeSpan.FromSeconds(DataManagement.DataManager.Instance.userData.stageListData.noAdsTime).Subtract(span);
            timeNoAds.text = $"{span.Minutes}:{span.Seconds}";
            RemoveAd();
            time = Time.time;
        }
    }
    public void SetCoolDown(System.Action onDone)
    {
        blockAds.interactable = false;
        timeNoAds.gameObject.SetActive(true);
        StartCoroutine(DoCoolDown(onDone));
    }
    IEnumerator DoCoolDown(System.Action onDone)
    {
        double time = DataManagement.DataManager.Instance.userData.stageListData.noAdsTime - System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.progressData.timeNoAds)).TotalSeconds;
        double t = 0;
        while (t <= time)
        {
            t += Time.deltaTime;
            yield return null;
        }
        blockAds.interactable = true;
        timeNoAds.gameObject.SetActive(false);
        onDone?.Invoke();
    }
    void RemoveAd()
    {
        DataManagement.DataManager.Instance.userData.IsAd = false;
        DataManagement.DataManager.Instance.Save();
        AD.Controller.Instance.RemoveAd();
    }
    void ReloadAds()
    {
        DataManagement.DataManager.Instance.userData.stageListData.isNoAds = false;
        DataManagement.DataManager.Instance.userData.IsAd = true;
        DataManagement.DataManager.Instance.Save();
        AD.Controller.Instance.ReloadAd();
    }
    public void OnEarningUpdated(int totalEarning)
    {
        Debug.Log(totalEarning);
        totalEarningText.text = $"${GameUtility.GameUtility.ShortenNumber((double)totalEarning)}/s";
        totalEarningText.transform.Shake(0.1f, 1, 0.15f);
    }

    public void OnNewModelAdded()
    {
        newMonsterPS.Play();
        handTut.SetActive(false);
    }

    public void OnMonsterSlotUpdated(int current, int max)
    {
        slotText.text = $"{current}/{max}";
        slotText.transform.Shake(0.1f, 1, 0.15f);
        maxSlotNotice.transform.Shake(0.2f, 2, 0.2f);
    }

    private void OnCashUpdated(Inventory inventory, ObscuredInt cash)
    {
        cashText.text = $"${GameUtility.GameUtility.ShortenNumber((double)cash)}";
        cashText.transform.Shake(0.1f, 1, 0.15f);
    }
    private void OnDisable()
    {
        DataManagement.DataManager.Instance.userData.inventory.onUpdate -= OnCollectionUpdated;
        DataManagement.DataManager.Instance.userData.inventory.onCashUpdated -= OnCashUpdated;
        DataManagement.DataManager.Instance.userData.YourMoney = (double)DataManagement.DataManager.Instance.userData.inventory.cash;
    }
    private void OnCollectionUpdated(Inventory inventory)
    {
        Debug.Log("SET COLLECTION");
        DataManagement.CardData cardData = DataManagement.DataManager.Instance.userData.inventory.GetFirstCollection();
        if (cardData != null)
        {
            Debug.Log("SET COLLECTION " + cardData.id);
            monsterCard.SetUp(cardData);
        }
        else
        {
            handTut.SetActive(false);
            monsterCard.gameObject.SetActive(false);
        }
    }

    public void OnSlotFull()
    {
        monsterCard.transform.Shake(0.1f, 1, 0.15f);
        slotTotalRect.transform.Shake(0.1f, 1, 0.15f);
    }


    public override void Deactive()
    {
        base.Deactive();
    }
    public void ClickScreen()
    {
        if (!((StageGameController)Game.Controller.Instance.gameController).isSelected && !((StageGameController)Game.Controller.Instance.gameController).isMoveStage)
        {
            ((StageGameController)Game.Controller.Instance.gameController).ClickScreen(false);
        }
        else
        {
            ((StageGameController)Game.Controller.Instance.gameController).isSelected = false;
            ((StageGameController)Game.Controller.Instance.gameController).isMoveStage = false;
        }
    }
    public void Play()
    {
        if (isProcessing) return;
        isProcessing = true;
        Sound.Controller.Instance.PlayOneShot(playSFX);
        LevelLoading.Instance.Active("MainScene",
                () => { Game.Controller.Instance.gameController.Clear(); },
                async () =>
                {
                    await Game.Controller.Instance.gameController.SetUp();
                    LevelLoading.Instance.Close();
                }
                , closeOverride: true);
    }
    public void Collection()
    {
        if (isProcessing) return;
        isProcessing = true;
        LevelLoading.Instance.Active(() =>
        {
            ((StageGameController)Game.Controller.Instance.gameController).HideCurrentStageMonster();
            Close();
            ((StageGameController)Game.Controller.Instance.gameController).SetUpCollection();
        });
    }
    public void Decoration()
    {
        if (isProcessing) return;
        isProcessing = true;
        UI.PanelManager.Create(typeof(DecorPanel), (panel, op) =>
        {
            Close();
            ((DecorPanel)panel).SetUp();

            ((StageGameController)Game.Controller.Instance.gameController).HideCurrentStageMonster();
        });
    }
    public void LeaderBoard()
    {
        if (isProcessing) return;
        isProcessing = true;
        LevelLoading.Instance.Active(() =>
        {
            ((StageGameController)Game.Controller.Instance.gameController).HideCurrentStageMonster();
            UI.PanelManager.Create(typeof(LeaderBoardPanel), (panel, op) =>
            {
                ((LeaderBoardPanel)panel).SetUp();
            });
            Close();
            LevelLoading.Instance.Close();
        });
    }
    public void Shop()
    {
        if (isProcessing) return;
        isProcessing = true;
        LevelLoading.Instance.Active(() =>
        {
            ((StageGameController)Game.Controller.Instance.gameController).HideCurrentStageMonster();
            UI.PanelManager.Create(typeof(ShopPanel), (panel, op) =>
            {
                ((ShopPanel)panel).SetUp();
            });
            Close();
            LevelLoading.Instance.Close();
        });
    }
    public void OfflineReward()
    {
        if (isProcessing) return;
        isProcessing = true;
        UI.PanelManager.Create(typeof(OfflineRewardPanel), (panel, op) =>
        {
            ((OfflineRewardPanel)panel).SetUp(DataManagement.DataManager.Instance.userData.stageListData.totalOfflineSeconds, DataManagement.DataManager.Instance.userData.stageListData.totalOfflineCash, DataManagement.DataManager.Instance.userData.stageListData.totalOfflineGold);
            ((StageGameController)Game.Controller.Instance.gameController).hideMonster = true;
            isProcessing = false;
        });
    }
    public void View()
    {
        if (DataManagement.DataManager.Instance.userData.progressData.uiHome)
        {
            view.sprite = uiOn;
            for (int i = 0; i < uiHome.Length; i++)
            {
                uiHome[i].SetActive(false);
                boxBanner.SetActive(true);
            }
            if (DataManagement.DataManager.Instance.userData.inventory.GetItemState("SetBundle_1") == 0)
            {
                bundleBtn.SetActive(false);
            }
            DataManagement.DataManager.Instance.userData.progressData.uiHome = false;
        }
        else
        {
            view.sprite = uiOff;
            for (int i = 0; i < uiHome.Length; i++)
            {
                uiHome[i].SetActive(true);
                boxBanner.SetActive(false);
            }
            if (DataManagement.DataManager.Instance.userData.inventory.GetItemState("SetBundle_1") == 0)
            {
                bundleBtn.SetActive(true);
            }
            DataManagement.DataManager.Instance.userData.progressData.uiHome = true;
        }
    }
    public void DailyReward()
    {
        if (isProcessing) return;
        isProcessing = true;
        UI.PanelManager.Create(typeof(DailyRewards), (panel, op) =>
        {
            ((DailyRewards)panel).SetUp();
            ((StageGameController)Game.Controller.Instance.gameController).hideMonster = true;
            isProcessing = false;
        });
    }
    public void Setting()
    {
        if (isProcessing) return;
        isProcessing = true;
        UI.PanelManager.Create(typeof(SettingPopup), (panel, op) =>
        {
            ((SettingPopup)panel).SetUp();
            isProcessing = false;
            ((StageGameController)Game.Controller.Instance.gameController).hideMonster = true;
        });
    }
    public void NoAds()
    {
        if (isProcessing) return;
        isProcessing = true;
        UI.PanelManager.Create(typeof(NoAdsPanel), (panel, op) =>
        {
            ((NoAdsPanel)panel).SetUp(onUnlock: () =>
            {
                blockAds.interactable = false;
                timeNoAds.gameObject.SetActive(true);
            });
            ((StageGameController)Game.Controller.Instance.gameController).hideMonster = true;
            isProcessing = false;
        });
    }
    public void ShowBundle()
    {
        if (isProcessing) return;
        isProcessing = true;
        UI.PanelManager.Create(typeof(SetBundlePanel), (panel, op) =>
        {
            isProcessing = false;
            ((SetBundlePanel)panel).SetUp("SetBundle_1", onUnlock: () =>
            {
                bundleBtn.SetActive(false);
            });
            ((StageGameController)Game.Controller.Instance.gameController).hideMonster = true;
        });
    }
    public void ShowStageUpgrade()
    {
        OnSlotFull();
        UI.PanelManager.Create(typeof(SlotExpandPanel), (panel, op) =>
        {
            StageGameController ctr = ((StageGameController)Game.Controller.Instance.gameController);
            ((SlotExpandPanel)panel).SetUp(ctr.GetStageHandler().stageData, isExpanded => { OnMonsterSlotUpdated(ctr.GetStageHandler().stageData.stageCollections.Count, ctr.GetStageHandler().stageData.totalMonsterSlot); });
            ((StageGameController)Game.Controller.Instance.gameController).hideMonster = true;
            panel.onClose = () => { AD.Controller.Instance.ShowInterstitial(); };
        });
    }
    public void Share()
    {
        if (isProcessing) return;
        isProcessing = true;
        boxBanner.SetActive(false);
        viewBtn.SetActive(false);
        Texture2D capturedScreenShot = null;
        StartCoroutine(DoCapture(res =>
        {
            capturedScreenShot = res;

            Effect.EffectSpawner.Instance.Get(0, effect =>
            {
                effect.Active(Vector3.zero);
            }).Forget();

        }));
        UI.PanelManager.Create(typeof(SharePanel), (panel, op) =>
        {
            isProcessing = false;
            ((SharePanel)panel).SetUp(Sprite.Create(capturedScreenShot, new Rect(capturedScreenShot.width / 16, capturedScreenShot.height / 6, capturedScreenShot.width * 7 / 8, capturedScreenShot.height - capturedScreenShot.height * 2 / 6), Vector2.zero));
            ((StageGameController)Game.Controller.Instance.gameController).hideMonster = true;
            boxBanner.SetActive(true);
            viewBtn.SetActive(true);
        });
    }
    IEnumerator DoCapture(System.Action<Texture2D> result)
    {
        Sound.Controller.Instance.PlayOneShot(cameraClip);
        yield return new WaitForEndOfFrame();
        Texture2D capturedScreenShot = GameUtility.ScreenCapture.Capture();
        result.Invoke(capturedScreenShot);
    }
    public void OnMonsterSelected()
    {
        controlPanel.SetActive(false);
        deletePanel.SetActive(true);
    }

    public void OnMonsterRelease()
    {
        controlPanel.SetActive(true);
        deletePanel.SetActive(false);
        handTut.SetActive(DataManagement.DataManager.Instance.userData.inventory.GetFirstCollection() != null &&
                          (DataManagement.DataManager.Instance.userData.stageListData.stageDatas.Count == 0 || DataManagement.DataManager.Instance.userData.stageListData.stageDatas[0].stageCollections.Count == 0));
    }

    public void UseBooster(int booster)
    {
        if (((StageGameController)Game.Controller.Instance.gameController).IsBooserActivated((EBooster)booster))
        {
            boosterButtons[booster].transform.Shake(0.15f, 1, 0.2f);
            UI.PanelManager.Create(typeof(MessagePanel), (panel, op) => { ((MessagePanel)panel).SetUp("This booster is in use"); });
            return;
        }

        BoosterData boosterData = DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)booster];

        if (boosterData.Amount > 0)
        {
            ActiveBooster();
            boosterData.Amount--;
            DataManagement.DataManager.Instance.Save();
        }
        else
        {
            AD.Controller.Instance.ShowRewardedAd("booster", res =>
            {
                if (res)
                {
                    ActiveBooster();
                }
            });
        }

        bool ActiveBooster()
        {
            return ((StageGameController)Game.Controller.Instance.gameController).ActiveBooster((EBooster)booster);
        }
    }

    public void UpdateBooster()
    {
    }

}

public class InstantMoneyBooster : IBooster
{
    private const EBooster type = EBooster.InstantMoney;
    Action onBoosterActive;
    Action onBoosterRefilled;

    bool isActivated = false;
    float duration;
    float startTime = 0;

    public bool Active(StageGameController gameController)
    {
        if (isActivated) return false;
        isActivated = true;
        startTime = Time.time;
        duration = 5;
        int totalCash = (int)Sheet.SheetDataManager.Instance.gameData.stageConfig.GetBooster(EBooster.InstantMoney).GetStat(BoosterStatKey.INSTANTCASH).value;
        gameController.RewardInstantCash(totalCash);
        DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)EBooster.InstantMoney].lastUseBooster = System.DateTime.Now.Ticks;

        onBoosterActive?.Invoke();
        Debug.Log(type + " " + DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].Amount);
        if (DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].Amount > 0)
        {
            Debug.Log(type + "  => " + DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].Amount);
            DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseFree = System.DateTime.Now.Ticks;
        }

        return true;
    }

    float time = 0;

    public void OnUpdate(StageGameController gameController)
    {
        if (isActivated)
        {
            if (Time.time - startTime >= duration)
            {
                Remove(gameController);
            }
        }
    }

    public void Remove(StageGameController gameController)
    {
        isActivated = false;
        DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)EBooster.InstantMoney].lastUseBooster = System.DateTime.Now.Ticks;
    }

    public bool IsActive()
    {
        return isActivated;
    }

    public void CheckRefill()
    {
        if (DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)EBooster.InstantMoney].Amount < 1 &&
            (float)System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseFree)).TotalSeconds >= Sheet.SheetDataManager.Instance.gameData.stageConfig.boosterRecoverTime)
        {
            DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)EBooster.InstantMoney].Amount++;
            onBoosterRefilled?.Invoke();
        }
    }

    public void AddOnBoosterActive(Action onBoosterActive)
    {
        this.onBoosterActive = onBoosterActive;
    }

    public float GetDuration()
    {
        return duration;
    }

    public void AddOnBoosterRefilled(Action onBoosterRefilled)
    {
        this.onBoosterRefilled = onBoosterRefilled;
    }

    public float GetCurrentCoolDown()
    {
        return (float)System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseBooster)).TotalSeconds;
    }

    public void SetActive(bool isActivated)
    {
        this.isActivated = isActivated;
    }
}
public class SpeedBooster : IBooster
{
    private const EBooster type = EBooster.SpeedBoost;
    Action onBoosterRefilled;

    Action onBoosterActive;
    bool isActivated = false;
    float clickRate = 0, duration;
    float startTime = 0;

    public bool Active(StageGameController gameController)
    {
        if (isActivated) return false;
        isActivated = true;
        Sound.Controller.Instance.PlayBoosterSfx();
        startTime = Time.time;
        float speed = Sheet.SheetDataManager.Instance.gameData.stageConfig.GetBooster(type).GetStat(BoosterStatKey.EARNSPEED).value;

        Debug.Log("SPEED:" + speed);
        duration = Sheet.SheetDataManager.Instance.gameData.stageConfig.GetBooster(type).GetStat(BoosterStatKey.DURATION).value;
        gameController.SetEarnSpeed(speed);

        DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseBooster = System.DateTime.Now.Ticks;
        onBoosterActive?.Invoke();
        if (DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].Amount > 0)
        {
            DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseFree = System.DateTime.Now.Ticks;
        }

        return true;
    }

    float time = 0;

    public void OnUpdate(StageGameController gameController)
    {
        if (isActivated)
        {
            if (Time.time - startTime >= duration)
            {
                Remove(gameController);
            }
        }
    }

    public void Remove(StageGameController gameController)
    {
        isActivated = false;
        DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)EBooster.AutoClick].lastUseBooster = System.DateTime.Now.Ticks;
        gameController.SetEarnSpeed(1);
    }

    public bool IsActive()
    {
        return isActivated;
    }

    public void CheckRefill()
    {
        if (DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].Amount < 1 &&
            (float)System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseFree)).TotalSeconds >= Sheet.SheetDataManager.Instance.gameData.stageConfig.boosterRecoverTime)
        {
            DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].Amount++;
            onBoosterRefilled?.Invoke();
        }
    }

    public void AddOnBoosterActive(Action onBoosterActive)
    {
        this.onBoosterActive = onBoosterActive;
    }

    public float GetDuration()
    {
        return duration;
    }

    public void AddOnBoosterRefilled(Action onBoosterRefilled)
    {
        this.onBoosterRefilled = onBoosterRefilled;
    }

    public float GetCurrentCoolDown()
    {
        return (float)System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseBooster)).TotalSeconds;
    }

    public void SetActive(bool isActivated)
    {
        this.isActivated = isActivated;
    }
}
public class AutoClickBooster : IBooster
{
    private const EBooster type = EBooster.AutoClick;
    Action onBoosterActive;
    Action onBoosterRefilled;
    bool isActivated = false;
    float clickRate = 0, duration;
    float startTime = 0;

    public bool Active(StageGameController gameController)
    {
        if (isActivated) return false;
        isActivated = true;
        Sound.Controller.Instance.PlayBoosterSfx();
        startTime = Time.time;
        clickRate = Sheet.SheetDataManager.Instance.gameData.stageConfig.GetBooster(type).GetStat(BoosterStatKey.AUTOCLICKRATE).value;
        duration = Sheet.SheetDataManager.Instance.gameData.stageConfig.GetBooster(type).GetStat(BoosterStatKey.DURATION).value;
        DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseBooster = System.DateTime.Now.Ticks;
        if (DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].Amount > 0)
        {
            DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseFree = System.DateTime.Now.Ticks;
        }

        onBoosterActive?.Invoke();
        return true;
    }

    float time = 0;

    public void OnUpdate(StageGameController gameController)
    {
        if (isActivated)
        {
            if (Time.time - time < clickRate) return;
            time = Time.time;
            gameController.ClickScreen(true);

            if (Time.time - startTime >= duration)
            {
                Remove(gameController);
            }
        }
    }

    public void Remove(StageGameController gameController)
    {
        isActivated = false;
        DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseBooster = System.DateTime.Now.Ticks;
    }

    public bool IsActive()
    {
        return isActivated;
    }

    public void CheckRefill()
    {
        if (DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].Amount < 1 &&
            (float)System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseFree)).TotalSeconds >= Sheet.SheetDataManager.Instance.gameData.stageConfig.boosterRecoverTime)
        {
            DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].Amount++;
            onBoosterRefilled?.Invoke();
        }
    }

    public void AddOnBoosterActive(Action onBoosterActive)
    {
        this.onBoosterActive = onBoosterActive;
    }

    public float GetDuration()
    {
        return duration;
    }

    public void AddOnBoosterRefilled(Action onBoosterRefilled)
    {
        this.onBoosterRefilled = onBoosterRefilled;
    }

    public float GetCurrentCoolDown()
    {
        return (float)System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.stageListData.boosters[(int)type].lastUseBooster)).TotalSeconds;
    }

    public void SetActive(bool isActivated)
    {
        this.isActivated = isActivated;
    }
}
public interface IBooster
{
    bool Active(StageGameController gameController);
    void Remove(StageGameController gameController);
    void OnUpdate(StageGameController gameController);
    bool IsActive();
    void CheckRefill();
    float GetDuration();
    float GetCurrentCoolDown();
    void AddOnBoosterActive(System.Action onBoosterActive);
    void AddOnBoosterRefilled(System.Action onBoosterRefilled);
    void SetActive(bool isActivated);
}
public enum EBooster
{
    InstantMoney,
    SpeedBoost,
    AutoClick
}
