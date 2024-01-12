using Cysharp.Threading.Tasks;
using GameUtility;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public class MakeOverPanel : MakeOverPanelAbstract
{
    public override async UniTask SetUp()
    {
        //prepare pet pool
        Game.Controller.Instance.itemData.GetPack(ItemData.Category.Pet).PrepareItemPool(petReadyPool, excludeItems, null);
        //prepare monster
        Monster monster = ((MakeOverGameController)Game.Controller.Instance.gameController).monster;
        monster.SetUp();
        monster.transform.localPosition = new Vector3(0, -11);
        monster.transform.localScale = new Vector3(1.3f, 1.3f, 1);

        ItemData.Item randomHead;
        do
        {
            randomHead = Game.Controller.Instance.itemData.GetPack(ItemData.Category.Head).GetRandom();
        } while (randomHead.unlockType == ItemData.UnlockType.Ad || (!string.IsNullOrEmpty(randomHead.bundleId) && DataManagement.DataManager.Instance.userData.inventory.GetItemState(randomHead.id) == 0));
        monster.SetItem(randomHead).Forget();
        defaultItems.Clear();
        defaultItems.Add(randomHead);

        await base.SetUp();
    }
    public override async UniTask<bool> OnSelect(ItemSelectButton itemSelectButton)
    {
        if (isProcessing) return false;
        isProcessing = true;
        handTut.SetActive(false);
        selectedItems.Add(itemSelectButton.item);
        mySet.Add(itemSelectButton.item);
        Monster monster = ((MakeOverGameController)Game.Controller.Instance.gameController).monster;

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
        Effect.EffectSpawner.Instance.Get(1, effect =>
        {
            effect.Active(itemSelectButton.transform.position);
        }).Forget();
        Sprite icon = await itemSelectButton.item.GetTextureAsync();
        GameObject spawnPlace = monster.GetItemPlace(itemSelectButton.item);
        ObjectSpawner.Instance.Get(0, obj =>
        {
            ItemOrb itemOrb = obj.GetComponent<ItemOrb>();
            itemOrb.SetUp(icon, itemSelectButton.transform, spawnPlace.transform, 7, res =>
                    {
                        Debug.Log(currentCategory);
                        tabHandler.tabButtons[(int)currentCategory].SetUp(true, true);
                        spawnPlace.transform.Shake(0.15f, 1.5f, 0.2f, cancellationToken: cancellation.Token, defaultScale: spawnPlace.transform.localScale.x).Forget();
                        monster.SetItem(itemSelectButton.item).Forget();

                           //
                        Effect.EffectSpawner.Instance.Get(2, effect =>
                       {
                           effect.Active(spawnPlace.transform.position);
                       }).Forget();
                   });
        }).Forget();
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
}
