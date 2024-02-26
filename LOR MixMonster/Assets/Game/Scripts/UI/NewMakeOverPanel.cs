using Cysharp.Threading.Tasks;
using GameUtility;
using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public partial class NewMakeOverPanel : MakeOverPanelAbstract
{
  
    [SerializeField]
    private Spine.Unity.SkeletonGraphic sheepAnim;

    public override async UniTask SetUp()
    {
        //prepare pet pool
        Game.Controller.Instance.itemData.GetPack(ItemData.Category.Pet).PrepareItemPool(petReadyPool, excludeItems, null);
        //prepare monster
        Monster monster = ((MakeOverGameController)Game.Controller.Instance.gameController).monster;
        monster.SetUp();
        monster.transform.localPosition = new Vector3(0, -12.5f);
        monster.transform.localScale = new Vector3(1f, 1, 1);
        monster.SetIdle();
        goldText.text = DataManagement.DataManager.Instance.userData.YourGold.ToString();
        defaultItems.Clear();

        for (int i=0;i<4;i++)
        {
            ItemData.Item randomItem ;
            do
            {
                randomItem = Game.Controller.Instance.itemData.GetPack((ItemData.Category)i).GetRandom();
            } while (randomItem.unlockType == ItemData.UnlockType.Ad || (!string.IsNullOrEmpty(randomItem.bundleId) && DataManagement.DataManager.Instance.userData.inventory.GetItemState(randomItem.id) == 0));

            defaultItems.Add(randomItem);
            monster.SetItem(randomItem).Forget();
        }


        ItemData.Item  body = Game.Controller.Instance.itemData.GetItem("Body_12");

        monster.SetItem(body).Forget();

        defaultItems.Add(body);


        await base.SetUp();
    }

    public override async UniTaskVoid SetItem()
    {
        for (int i = 0; i < itemSelectButtons.Length; i++)
        {
            ItemSelectButton button = itemSelectButtons[i];
            button.Hide();
            Effect.EffectSpawner.Instance.Get(2, effect =>
            {
                effect.Active(button.transform.position);
            }).Forget();
        }
        isProcessing = true;
        sheepAnim.AnimationState.SetAnimation(0, "idle_start", false);
        await UniTask.Delay(500, cancellationToken: cancellation.Token);
        sheepAnim.AnimationState.SetAnimation(0, "get_cloth", false);
        sheepAnim.AnimationState.AddAnimation(0, "idle", true, 0);
        await UniTask.Delay(400, cancellationToken: cancellation.Token);

        for (int i = 0; i < itemSelectButtons.Length; i++)
        {
            itemSelectButtons[i].SetUp(readyPool[i], this);
        }
        await UniTask.Delay(500, cancellationToken: cancellation.Token);
        for (int i = 0; i < itemSelectButtons.Length; i++)
        {
            sheepAnim.AnimationState.SetAnimation(0, i == 0 ? "left_start" : "right_start", false);
            sheepAnim.AnimationState.AddAnimation(0, i == 0 ? "left_idle" : "right_idle", false, 0);
            itemSelectButtons[i].HighLight();
            Sound.Controller.Instance.PlayOneShot(leftRightClips[i]);
            await UniTask.Delay(800, cancellationToken: cancellation.Token);
            sheepAnim.AnimationState.SetAnimation(0, "idle", true);

        }
        chooseTut.SetActive(DataManagement.DataManager.Instance.userData.progressData.firstSelect);
        DataManagement.DataManager.Instance.userData.progressData.firstSelect = false;
        DataManagement.DataManager.Instance.Save();
        isProcessing = false;

    }


    public override async UniTask<bool> OnSelect(ItemSelectButton itemSelectButton)
    {
        if (isProcessing) return false;
        isProcessing = true;

        if (Sound.Controller.VibrationEnable)
        {
            MMVibrationManager.Haptic(hapticTypes, true, true, this);
        }
        if (selectedItems.Count == Game.Controller.Instance.gameConfig.adConfig.adBetweenMakeOver)
        {
            bool isAdFinished = false;
            AD.Controller.Instance.ShowInterstitial(() =>
            {
                isAdFinished = true;
            });
            await UniTask.WaitUntil(() => isAdFinished, cancellationToken: cancellation.Token);
        }

        foreach (ItemSelectButton button in itemSelectButtons)
        {
            previousFirstSpawnItems.Add(button.item);
        }

        if (petOfferBtn.activeSelf)
        {
            petReadyPool.RemoveAt(0);
            petOfferBtn.SetActive(false);
        }


        chooseTut.SetActive(false);
        selectedItems.Add(itemSelectButton.item);
        mySet.Add(itemSelectButton.item);
        itemSelectButton.HighLight();
        await ((MakeOverGameController)Game.Controller.Instance.gameController).OnNewItemSelected(itemSelectButton.item, itemSelectButton);

        itemSelectButton.Hide();
        Effect.EffectSpawner.Instance.Get(1, effect =>
        {
            effect.Active(itemSelectButton.transform.position);
        }).Forget();

        Sprite icon = await itemSelectButton.item.GetIconAsync();
        Transform spawnPlace = tabHandler.tabButtons[(int)currentCategory].transform;
        ObjectSpawner.Instance.Get(0, obj =>
        {
            ItemOrb itemOrb = obj.GetComponent<ItemOrb>();
            itemOrb.SetUp(icon, itemSelectButton.transform, spawnPlace, 8, res =>
            {
                tabHandler.tabButtons[(int)currentCategory].SetUp(true, true);
                spawnPlace.Shake(0.15f, 1.5f, 0.2f, cancellationToken: cancellation.Token, defaultScale: spawnPlace.transform.localScale.x).Forget();
                ((MakeOverGameController)Game.Controller.Instance.gameController).monster.SetItem(itemSelectButton.item);
                Effect.EffectSpawner.Instance.Get(2, effect =>
                {
                    effect.Active(((MakeOverGameController)Game.Controller.Instance.gameController).monster.GetItemPlace(itemSelectButton.item).transform.position);
                }).Forget();

                //
                Effect.EffectSpawner.Instance.Get(2, effect =>
                {
                    effect.Active(spawnPlace.position);
                }).Forget();
            });
        }).Forget();

        //
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
            foreach (ItemSelectButton button in itemSelectButtons)
            {
                if (button != itemSelectButton)
                {
                    Effect.EffectSpawner.Instance.Get(1, effect =>
                    {
                        effect.Active(button.transform.position);
                    }).Forget();
                    button.Hide();
                }
            }
            await UniTask.Delay(500, cancellationToken: cancellation.Token);
            SetCategory(currentCategory + 1);
        }
        return true;
    }
    public void OnSelect(int index)
    {
        itemSelectButtons[index].OnSelect();
    }
}
