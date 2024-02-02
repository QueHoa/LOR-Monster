using Cysharp.Threading.Tasks;
using GameUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class MakeOverGameController : GameController
{
    public Monster monster;
    public int monsterId;
    CancellationTokenSource cancellation;
    [SerializeField]
    private Transform[] petSpawnPlaces;
    private int changeGold;
    private void OnEnable()
    {
    }
    private void OnDestroy()
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
        }
    }
    private void OnDisable()
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
        }
    }
    private void Start()
    {
        Game.Controller.Instance.OnGameLoaded(this);
    }
    int musicThemeIndex = 0;
    public override async UniTask InitializeAsync()
    {
        musicThemeIndex = DataManagement.DataManager.Instance.userData.progressData.playCount == 0 ? 4 : UnityEngine.Random.Range(0, Sound.Controller.Instance.soundData.finalThemes.Length);

    }
    public override async UniTask SetUp()
    {
        cancellation = new CancellationTokenSource();
        hideMonster = false;
        Sound.Controller.Instance.PlayMusic(Sound.Controller.Instance.soundData.menuTheme[UnityEngine.Random.Range(0, Sound.Controller.Instance.soundData.menuTheme.Length)]);


        monster = (await Addressables.InstantiateAsync("Monster", transform)).GetComponent<Monster>();

        MakeOverPanelAbstract makeOverPanel = (MakeOverPanelAbstract)await UI.PanelManager.CreateAsync(Game.Controller.Instance.gameConfig.gameType == 0 ? typeof(MakeOverPanel) : typeof(NewMakeOverPanel));
        makeOverPanel.SetUp();


        LevelLoading.Instance.Close();

        //


        FirebaseAnalysticController.Instance.LogEvent("MakeOverStart");
        DataManagement.DataManager.Instance.userData.progressData.playCount++;
        DataManagement.DataManager.Instance.Save();
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
    public async UniTask OnNewItemSelected(ItemData.Item selectedItem, ItemSelectButton itemSelectButton)
    {

        switch (itemSelectButton.index)
        {
            case 0:
                monster.PickLeft();
                break;
            case 1:
                monster.PickRight();
                break;
        }
        await UniTask.Delay(600, cancellationToken: cancellation.Token);
    }

    List<int[]> petSpawnOrder = new List<int[]>(new int[][] { new int[] { 0 }, new int[] { 0, 1 }, new int[] { 0, 1, 2 } });
    async UniTask PreparePet(List<ItemData.Item> selectedItems)
    {
        int totalPet = 0;
        foreach (ItemData.Item item in selectedItems)
        {
            totalPet += item.category == ItemData.Category.Pet ? 1 : 0;
        }
        int index = 0;
        foreach (ItemData.Item item in selectedItems)
        {
            if (item.category != ItemData.Category.Pet) continue;
            Pet pet = (await ObjectSpawner.Instance.GetAsync(1)).GetComponent<Pet>();
            pet.SetUp(item.skin, petSpawnPlaces[index].position);
            index++;
        }

    }
    [SerializeField]
    private AnimationCurve monsterZoomCurve, monsterMoveCurve;
    [SerializeField]
    private AudioClip cameraClip;
    public async UniTaskVoid FinishMakeOver(List<ItemData.Item> selectedItems, List<ItemData.Item> mySets)
    {

        int totalViewPoint = 0;
        int totalLikePoint = 0;
        foreach (ItemData.Item item in selectedItems)
        {
            totalViewPoint += item.viewPoint.GetRandomInt();
        }
        float rd = UnityEngine.Random.Range(60, 80) / 100f;
        totalLikePoint = (int)(totalViewPoint * rd);

        PreparePet(selectedItems);

        monster.Dance(musicThemeIndex % Sound.Controller.Instance.soundData.finalThemes.Length);
        await ZoomMonster(cancellation.Token);
        Sound.Controller.Instance.PlayMusic(Sound.Controller.Instance.soundData.finalThemes[musicThemeIndex++ % Sound.Controller.Instance.soundData.finalThemes.Length]);
        Effect.EffectSpawner.Instance.Get(3, effect =>
        {
            effect.Active(Vector3.zero);
        }).Forget();

        LiveStreamPanel liveStreamPanel = (LiveStreamPanel)await UI.PanelManager.CreateAsync(typeof(LiveStreamPanel));
        await liveStreamPanel.SetUp(totalViewPoint, totalLikePoint);
        await UniTask.Delay(3000);
        liveStreamPanel.DeactiveUI();

        int bonusView = 0;
        int bonusLike = 0;
        Texture2D capturedScreenShot = null;
        StartCoroutine(DoCapture(res =>
        {
            capturedScreenShot = res;

            Effect.EffectSpawner.Instance.Get(0, effect =>
            {
                effect.Active(Vector3.zero);
            }).Forget();
            bonusView = liveStreamPanel.bonusView;
            bonusLike = liveStreamPanel.bonusLike;
            liveStreamPanel.Close();

        }));


        await UniTask.Delay(1000, cancellationToken: cancellation.Token);
        await UniTask.WaitUntil(() => capturedScreenShot != null, cancellationToken: cancellation.Token);
        changeGold = 0;
        for (int i = 0; i < Sheet.SheetDataManager.Instance.gameData.rewardGold.item.Count; i++)
        {
            if (totalLikePoint + bonusLike >= Sheet.SheetDataManager.Instance.gameData.rewardGold.item[i].like && totalLikePoint + bonusLike < Sheet.SheetDataManager.Instance.gameData.rewardGold.item[i + 1].like)
            {
                changeGold = (int)(Sheet.SheetDataManager.Instance.gameData.rewardGold.item[i].gold.GetRandomInt() * UnityEngine.Random.Range(1.7f, 2f));
            }
        }
        //lưu dữ liệu con mới mix lại ở cardData
        DataManagement.CardData cardData = new DataManagement.CardData(DataManagement.DataManager.Instance.userData.progressData.collectionDatas.Count + 1, changeGold, selectedItems);
        DataManagement.DataManager.Instance.userData.inventory.AddCollection(cardData);
        Game.Controller.Instance.gameController.Destroy();
        ResultPanel resultPanel = (ResultPanel)await UI.PanelManager.CreateAsync(typeof(ResultPanel));
        resultPanel.SetUp(changeGold / 10, totalViewPoint + bonusView, totalLikePoint + bonusLike, Sprite.Create(capturedScreenShot, new Rect(0, capturedScreenShot.height / 20, capturedScreenShot.width, capturedScreenShot.height - capturedScreenShot.height * 2 / 20), Vector2.zero), mySets);

        Debug.Log($"BEST VIEW {DataManagement.DataManager.Instance.userData.BestView}+ {totalViewPoint + bonusView}");
        DataManagement.DataManager.Instance.userData.BestView = totalViewPoint + bonusView;
        DataManagement.DataManager.Instance.Save();

        await UniTask.Delay(2800, cancellationToken: cancellation.Token);
        CheckReward(totalViewPoint + bonusView);
        FirebaseAnalysticController.Instance.LogEvent("MakeOverEnd");

    }

    public async UniTask ZoomMonster(CancellationToken cancellationToken)
    {
        float t = 0;
        Transform _transform = monster.transform;
        Vector3 scale = Vector3.one * 1.3f;
        Vector3 pos = Vector3.zero;

        while (t < monsterZoomCurve.keys[monsterZoomCurve.length - 1].time)
        {
            scale.x = scale.y = monsterZoomCurve.Evaluate(t);
            _transform.localScale = scale;

            pos.y = monsterMoveCurve.Evaluate(t);
            _transform.localPosition = pos;

            t += Time.fixedDeltaTime;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
        }
    }
    public async UniTask ZoomMonsterCollection(CancellationToken cancellationToken, Transform monsterPos)
    {
        Transform _transform = monster.transform;
        Vector3 scale = Vector3.one * 0.5f;

        _transform.localScale = scale;
        _transform.position = monsterPos.position;
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
    }
    IEnumerator DoCapture(System.Action<Texture2D> result)
    {
        Sound.Controller.Instance.PlayOneShot(cameraClip);
        yield return new WaitForEndOfFrame();
        Texture2D capturedScreenShot = GameUtility.ScreenCapture.Capture();
        result.Invoke(capturedScreenShot);
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus && Game.Controller.Instance.gameConfig.adConfig.openAd)
        {
            DelayShowOpenAd();
        }
    }
    void DelayShowOpenAd()
    {
        AD.Controller.Instance.ShowOpenAd();

    }
    void CheckReward(int view)
    {
        List<ItemData.Item> rewardItems = new List<ItemData.Item>();
        int[] views = Sheet.SheetDataManager.Instance.gameData.rewardBarConfig.views;
        var bundle = Sheet.SheetDataManager.Instance.gameData.itemData.GetBundle("SetBundle_2");
        for (int i = 0; i < views.Length; i++)
        {
            //item not yet unlocked
            if (view >= views[i] && DataManagement.DataManager.Instance.userData.inventory.GetItemState(bundle.modelSets[0].itemIds[i]) == 0)
            {
                rewardItems.Add(Sheet.SheetDataManager.Instance.gameData.itemData.GetItem(bundle.modelSets[0].itemIds[i]));

                foreach (ItemData.ModelSet modelSet in bundle.modelSets)
                {
                    DataManagement.DataManager.Instance.userData.inventory.SetItemState(modelSet.itemIds[i], 1);
                    //Debug.Log("SET STATE: " + modelSet.itemIds[i]+":" +1);
                }

            }
        }
        if (rewardItems.Count > 0)
        {
            UI.PanelManager.Create(typeof(RewardPanel), (panel, op) =>
            {
                ((RewardPanel)panel).SetUp(rewardItems);
            });
        }
    }
}
