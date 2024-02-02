using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DataManagement;
using GameUtility;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

public partial class StageGameController : GameController
{
    public static bool FIRST_SECTION;
    public HomePanel homePanel;
    public Monster monster;
    [SerializeField]
    StageEarningHandler stageHandlers;
    public List<IBooster> boosters = new List<IBooster>();
    public int currentStageId;
    public float earnSpeed = 1;
    int clickCount = 0;
    [SerializeField]
    private AudioClip selectCardSFX, removeModelSFX, createModelSFX, moveStageSFX, clickScreenSFX, stageItemSelectSFX, instantCashBoostSFX;
    public ParticleSystem clickPS, instantCashBoostPS;

    int musicThemeIndex = 0;
    bool isReady = false, isMonsterSelected = false;
    public override async UniTask InitializeAsync()
    {
        musicThemeIndex = DataManagement.DataManager.Instance.userData.progressData.playCount == 0 ? 4 : UnityEngine.Random.Range(0, Sound.Controller.Instance.soundData.finalThemes.Length);
        if (DataManagement.DataManager.Instance.userData.stageListData.stageDatas.Count == 0)
        {
            StageData stageData = new StageData();
            stageData.totalMonsterSlot = Sheet.SheetDataManager.Instance.gameData.stageConfig.slotConfigs[0].maxSlot;
            for (int i = 0; i < System.Enum.GetNames(typeof(ItemData.EStageItemCategory)).Length; i++)
            {
                var item = Sheet.SheetDataManager.Instance.gameData.itemData.GetItem($"{(ItemData.EStageItemCategory)i}_{i}");
            }
            stageData.isLocked = false;
            DataManagement.DataManager.Instance.userData.stageListData.AddStage(stageData);

            DataManagement.DataManager.Instance.Save();
        }
        boosters.Add(new InstantMoneyBooster());
        boosters.Add(new SpeedBooster());
        boosters.Add(new AutoClickBooster());
        foreach (IBooster booster in boosters)
        {
            booster.SetActive(false);
        }
    }
    public override async UniTask SetUp()
    {
        cancellation = new CancellationTokenSource();

        homePanel = (HomePanel)await UI.PanelManager.CreateAsync(typeof(HomePanel));
        hideMonster = false;
        Clear();
        await PrepareStage();
        await PrepareCollection();   
        Debug.Log("done");
        MonsterCard.onMonsterSelected += OnMonsterSelected;
        
        ObjectTouchHandler.onMonsterSelected += OnMonsterSelected;
        ObjectTouchHandler.onMonsterReleased += OnMonsterReleased;

        CaculateOfflineEarning();
        homePanel.SetUp();
        isReady = true;

        LevelLoading.Instance.Close();
    }
    public override async UniTask SetUpCollection()
    {
        cancellation = new CancellationTokenSource();

        Sound.Controller.Instance.PlayMusic(Sound.Controller.Instance.soundData.menuTheme[UnityEngine.Random.Range(0, Sound.Controller.Instance.soundData.menuTheme.Length)]);

        if (DataManagement.DataManager.Instance.userData.progressData.collectionDatas.Count > 0)
        {
            monster = (await Addressables.InstantiateAsync("Monster", transform)).GetComponent<Monster>();
        }

        CollectionPanel CollectionPanel = (CollectionPanel)await UI.PanelManager.CreateAsync(typeof(CollectionPanel));
        CollectionPanel.SetUp();

        await UniTask.Delay(300);
        LevelLoading.Instance.Close();

        //

        monster.Dance(musicThemeIndex % Sound.Controller.Instance.soundData.finalThemes.Length);
        await ZoomMonsterCollection(cancellation.Token, CollectionPanel.MonsterPos);

        FirebaseAnalysticController.Instance.LogEvent("CollectionStart");
        DataManagement.DataManager.Instance.userData.progressData.playCount++;
        DataManagement.DataManager.Instance.Save();
    }
    public override void Destroy()
    {
        base.Destroy();
        Addressables.ReleaseInstance(monster.gameObject);
        if (cancellation != null)
        {
            cancellation.Cancel();
        }
        ObjectSpawner.Instance.ClearAll();
        Effect.EffectSpawner.Instance.ClearAll();
    }
    public async UniTask ZoomMonsterCollection(CancellationToken cancellationToken, Transform monsterPos)
    {
        Transform _transform = monster.transform;
        Vector3 scale = Vector3.one * 0.5f;

        _transform.localScale = scale;
        _transform.position = monsterPos.position;
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
    }
    public StageEarningHandler GetStageHandler()
    {
        return stageHandlers;
    }
    public void SetEarnSpeed(float speed)
    {
        this.earnSpeed = speed;
    }
    public void RewardInstantCash(int cash)
    {
        DataManagement.DataManager.Instance.userData.inventory.Cash += cash;
        instantCashBoostPS.Play();
        Sound.Controller.Instance.PlayOneShot(instantCashBoostSFX);
        Effect.EffectSpawner.Instance.Get(8, effect => { effect.Active(Vector3.zero, cash); }).Forget();
    }
    private void OnMonsterSelected(Monster monster)
    {
        homePanel.OnMonsterSelected();
        isMonsterSelected = true;
    }

    private void OnMonsterReleased(Monster monster)
    {
        isMonsterSelected = false;
        if (IsDeleteCheck())
        {
            DeleteMonster(monster);
        }
        else
        {
            homePanel.OnMonsterRelease();
        }
    }

    async UniTask DeleteMonster(Monster monster)
    {
        Effect.EffectSpawner.Instance.Get(1, effect => { effect.Active(monster.transform.position); }).Forget();
        monster.gameObject.SetActive(false);
        homePanel.OnMonsterRelease();

        stageHandlers.RemoveMonster(monster);

        homePanel.OnEarningUpdated(stageHandlers.GetTotalEarning());
        homePanel.OnMonsterSlotUpdated(stageHandlers.stageData.stageCollections.Count, stageHandlers.stageData.totalMonsterSlot);
        currentCardData = null;
    }

    private bool IsDeleteCheck()
    {
        if (Physics.Raycast(CameraController.Instance.GetTouchPosition(), Vector3.forward, int.MaxValue, layerMask: LayerMask.GetMask("RemoveZone")))
        {
            return true;
        }

        return false;
    }

    public override void Clear()
    {
        base.Clear();
        MonsterCard.onMonsterSelected -= OnMonsterSelected;
        ObjectTouchHandler.onMonsterSelected -= OnMonsterSelected;
        ObjectTouchHandler.onMonsterReleased -= OnMonsterReleased;
    }

    async UniTask PrepareStage()
    {
        foreach (DataManagement.StageData stageData in DataManagement.DataManager.Instance.userData.stageListData.stageDatas)
        {
            StageEarningHandler stageEarningHandler = new StageEarningHandler(stageData);

            stageHandlers = stageEarningHandler;
        }
    }

    async UniTask PrepareCollection()
    {
        Debug.Log("************ ----------- ************** 1" );

        await stageHandlers.PrepareCollection();
    }

    CardData currentCardData;
    Monster dragMonster;
    Vector3 startPos;

    public void OnMonsterSelected(CardData cardData)
    {
        Sound.Controller.Instance.PlayOneShot(selectCardSFX);

        List<ItemData.Item> tempMonsterItems = new List<ItemData.Item>();
        foreach (string id in cardData.items)
        {
            if (Game.Controller.Instance.itemData.GetItem(id).category != ItemData.Category.Pet)
            {
                tempMonsterItems.Add(Game.Controller.Instance.itemData.GetItem(id));
            }
        }

        currentCardData = cardData;
        homePanel.OnMonsterSelected();

        ObjectSpawner.Instance.Get(2, obj =>
        {
            // Debug.Break();
            Debug.Log("touch count : " + Input.touchCount.ToString().Color("lime"));

            if (Input.touchCount > 0)
            {
                Debug.Log("drag....");
                isDrag = true;
                dragMonster = obj.GetComponent<Monster>();
                dragMonster.SetUp(tempMonsterItems);
                foreach (ItemData.Item item in tempMonsterItems)
                {
                    dragMonster.SetItem(item);
                }

                dragMonster.transform.localScale = Vector3.one * 0.25f;
                musicThemeIndex = DataManagement.DataManager.Instance.userData.progressData.playCount == 0 ? 4 : UnityEngine.Random.Range(0, Sound.Controller.Instance.soundData.finalThemes.Length);
                dragMonster.Dance(musicThemeIndex % Sound.Controller.Instance.soundData.finalThemes.Length);

                Vector2 worldPosition = CameraController.Instance.GetTouchPosition();
                dragMonster.transform.position = worldPosition;
                startPos = worldPosition;
            }
            else
            {
                Debug.Log("not drag");
                isDrag = false;
                obj.SetActive(false);
            }
        }).Forget();
    }

    public bool ActiveBooster(EBooster booster)
    {
        return boosters[(int)booster].Active(this);
    }

    public bool IsBooserActivated(EBooster booster)
    {
        return boosters[(int)booster].IsActive();
    }

    float checkTime = 0;
    float earnTime = 0;
    bool isDrag = false;

    private void Update()
    {
        if (!isReady) return;
        if (Time.time - earnTime > earnSpeed)
        {
            earnTime = Time.time;
            RewardEarning(false);
        }

        foreach (IBooster booster in boosters)
        {
            booster.OnUpdate(this);
        }

        if (Time.time - checkTime > 1)
        {
            foreach (IBooster booster in boosters)
            {
                booster.CheckRefill();
            }

            checkTime = Time.time;
        }

        if (isDrag && Input.touchCount > 0 && currentCardData != null && dragMonster != null)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 worldPosition = CameraController.Instance.GetTouchPosition();
                dragMonster.transform.position = worldPosition;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDrag = false;
                dragMonster.gameObject.SetActive(false);
                homePanel.OnMonsterRelease();
                if (Vector3.Distance(startPos, dragMonster.transform.position) < 3 || !IsModelOnStage())
                {
                    homePanel.monsterCard.transform.Shake(0.15f, 1, 0.2f);
                }
                else
                {
                    SetModelToStage(dragMonster.transform.position);
                }
            }
        }
    }

    public void ClickScreen(bool auto)
    {
        if (stageHandlers.stageData.stageCollections.Count > 0)
        {
            Sound.Controller.Instance.PlayOneShot(clickScreenSFX);
            clickPS.Play();
        }
        RewardEarning(true);
        if (!auto)
        {
            clickCount++;
            if (clickCount > Game.Controller.Instance.gameConfig.adConfig.clickCountShowInter)
            {
                clickCount = 0;
                AD.Controller.Instance.ShowInterstitial();
            }
        }
    }

    private bool IsModelOnStage()
    {
        if (Physics.Raycast(dragMonster.bottom.position, Vector3.forward, int.MaxValue, layerMask: LayerMask.GetMask("Ground")))
        {
            return true;
        }

        return false;
    }

    async UniTask SetModelToStage(Vector2 position)
    {
        isDrag = false;
        Debug.Log(" PrepareModel: " + currentCardData.id);
        bool result = await stageHandlers.OnMonsterSelected(currentCardData, position);
        if (result)
        {
            Effect.EffectSpawner.Instance.Get(1, effect => { effect.Active(position); }).Forget();
            Sound.Controller.Instance.PlayOneShot(createModelSFX);
            homePanel.OnMonsterSlotUpdated(stageHandlers.stageData.stageCollections.Count, stageHandlers.stageData.totalMonsterSlot);
            homePanel.OnEarningUpdated(stageHandlers.GetTotalEarning());
            homePanel.OnNewModelAdded();

            currentCardData = null;
        }
        else
        {
            homePanel.OnSlotFull();
            UI.PanelManager.Create(typeof(SlotExpandPanel), (panel, op) =>
            {
                ((SlotExpandPanel)panel).SetUp(stageHandlers.stageData, isExpanded =>
                {
                    if (isExpanded)
                    {
                        homePanel.OnMonsterSlotUpdated(stageHandlers.stageData.stageCollections.Count, stageHandlers.stageData.totalMonsterSlot);
                        SetModelToStage(position);
                    }
                    else
                    {
                        currentCardData = null;
                    }
                });
            });
        }
    }
    //call every second
    float lastSaveTime = 0;

    void RewardEarning(bool manual)
    {
        GameUtility.RewardHandler.ApplyCash(GetTotalEarning());
        stageHandlers.ShowEarnEffect(manual);
        if (Time.time - lastSaveTime > 5)
        {
            DataManagement.DataManager.Instance.Save();
            lastSaveTime = Time.time;
        }
    }

    public ObscuredInt GetTotalEarning()
    {
        ObscuredInt totalEarning = 0;
        DataManagement.DataManager.Instance.userData.stageListData.lastEarningDate = System.DateTime.Now.Ticks;

        totalEarning += stageHandlers.RewardEarning();

        return totalEarning;
    }

    public void CaculateOfflineEarning()
    {
        if (DataManagement.DataManager.Instance.userData.stageListData.lastEarningDate == 0) return;

        ObscuredInt totalOfflineEarning = 0;
        int totalOfflineSeconds = (int)System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.stageListData.lastEarningDate)).TotalSeconds;
        if (totalOfflineSeconds >= Game.Controller.Instance.gameConfig.maxOfflineEarningSeconds)
        {
            Debug.Log("EXCEEDED OFFLINE EARNING TIME");
            totalOfflineSeconds = Game.Controller.Instance.gameConfig.maxOfflineEarningSeconds;
        }


        totalOfflineEarning = totalOfflineSeconds * (ObscuredInt)(GetTotalEarning());

        Debug.Log("TOTAL EARNING: " + totalOfflineEarning + " in " + totalOfflineSeconds);
        DataManagement.DataManager.Instance.userData.stageListData.lastEarningDate = System.DateTime.Now.Ticks;


        GameUtility.RewardHandler.ApplyCash(totalOfflineEarning);
        Debug.Log("TOTAL OFFLINE EARNING " + totalOfflineEarning);
        DataManagement.DataManager.Instance.Save();

        if (totalOfflineEarning > 0 && !FIRST_SECTION)
        {
            FIRST_SECTION = true;
            hideMonster = true;
            UI.PanelManager.Create(typeof(OfflineEarnPanel), (panel, op) => { ((OfflineEarnPanel)panel).SetUp(totalOfflineEarning); });
        }
    }

    public void HideCurrentStageMonster()
    {
        stageHandlers.HideMonster();
    }

    public void ShowCurrentStageMonster()
    {
        stageHandlers.ShowMonster();
    }
}