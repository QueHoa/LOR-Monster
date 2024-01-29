using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DailyReward;
using DataManagement;
using GameUtility;
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
    private AudioClip playSFX;
    [SerializeField]
    private TextMeshProUGUI goldText, cashText, totalEarningText, slotText;
    [SerializeField]
    private RectTransform slotTotalRect;
    [SerializeField]
    private GameObject maxSlotNotice, controlPanel, deletePanel, bundleBtn, handTut;
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
    private ParticleSystem unlockPS, newModelPS;
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
        OnModelSlotUpdated(currentStageData.stageCollections.Count, currentStageData.totalModelSlot);
        bundleBtn.SetActive(DataManagement.DataManager.Instance.userData.inventory.GetItemState("SetBundle_1") == 0);
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
            }
        }
        else
        {
            view.sprite = uiOn;
            for (int i = 0; i < uiHome.Length; i++)
            {
                uiHome[i].SetActive(false);
            }
        }

        handTut.SetActive(DataManagement.DataManager.Instance.userData.inventory.GetFirstCollection() != null && (DataManagement.DataManager.Instance.userData.stageListData.stageDatas.Count == 0 || DataManagement.DataManager.Instance.userData.stageListData.stageDatas[0].stageCollections.Count == 0));
        goldText.text = DataManagement.DataManager.Instance.userData.YourGold.ToString();
        Show();
    }
    public void OnEarningUpdated(int totalEarning)
    {
        Debug.Log(totalEarning);
        totalEarningText.text = $"${GameUtility.GameUtility.ShortenNumber((double)totalEarning)}/s";
        totalEarningText.transform.Shake(0.1f, 1, 0.15f);
    }

    public void OnNewModelAdded()
    {
        newModelPS.Play();
        handTut.SetActive(false);
    }

    public void OnModelSlotUpdated(int current, int max)
    {
        slotText.text = $"{current}/{max}";
        maxSlotNotice.SetActive(current >= max);
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
        ((StageGameController)Game.Controller.Instance.gameController).ClickScreen(false);
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
        Sound.Controller.Instance.PlayOneShot(playSFX);
        LevelLoading.Instance.Active(() =>
        {
            Close();
            Game.Controller.Instance.gameController.SetUpCollection();
        });
    }
    public void LeaderBoard()
    {
        if (isProcessing) return;
        isProcessing = true;
        Sound.Controller.Instance.PlayOneShot(playSFX);
        LevelLoading.Instance.Active(() =>
        {
            Close();
            UI.PanelManager.Create(typeof(LeaderBoardPanel), (panel, op) =>
            {
                ((LeaderBoardPanel)panel).SetUp();
            });
            LevelLoading.Instance.Close();
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
            }
            DataManagement.DataManager.Instance.userData.progressData.uiHome = false;
        }
        else
        {
            view.sprite = uiOff;
            for (int i = 0; i < uiHome.Length; i++)
            {
                uiHome[i].SetActive(true);
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
        });
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
