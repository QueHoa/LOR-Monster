using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DataManagement;
using Game.Pool;
using System.Collections.Generic;
using UnityEngine;

public partial class StageGameController
{
    [System.Serializable]
    public class StageEarningHandler
    {
        public StageData stageData;
        public ObscuredInt totalEarning;
        public ObscuredFloat totalEarningBonus;
        public Dictionary<Monster, int> monsterItemDict = new Dictionary<Monster, int>();
        int musicThemeIndex = 0;

        public StageEarningHandler(StageData stageData)
        {
            this.stageData = stageData;
        }

        public async UniTask<Monster> PrepareMonster(CardData cardData, StageCollectionData stageCollection, Vector2 position)
        {
            ItemData.ItemPack itemPack = new ItemData.ItemPack();
            List<ItemData.Item> items = new List<ItemData.Item>();
            ObscuredInt totalEarning = cardData.money;
            /*foreach (string itemId in cardData.items)
            {
                ItemData.Item item = Sheet.SheetDataManager.Instance.gameData.itemData.GetItem(itemId);
                itemPack.items.Add(item);

                totalEarning += (item.unlockType == ItemData.UnlockType.None) ? Sheet.SheetDataManager.Instance.gameData.stageConfig.cashEarningForNormalItem : Sheet.SheetDataManager.Instance.gameData.stageConfig.cashEarningForAdItem;

            }*/
            Monster monster = (await ObjectSpawner.Instance.GetAsync(2)).GetComponent<Monster>();
            foreach (string id in cardData.items)
            {
                if (Game.Controller.Instance.itemData.GetItem(id).category != ItemData.Category.Pet)
                {
                    items.Add(Game.Controller.Instance.itemData.GetItem(id));
                }
            }
            
            await monster.SetUp(items);
            /*foreach (ItemData.Item item in items)
            {
                await monster.SetItem(item);
            }*/
            musicThemeIndex = DataManagement.DataManager.Instance.userData.progressData.playCount == 0 ? 4 : UnityEngine.Random.Range(0, Sound.Controller.Instance.soundData.finalThemes.Length);
            monster.Dance(musicThemeIndex % Sound.Controller.Instance.soundData.finalThemes.Length);
            monster.transform.position = position;
            monster.stageCollectionData = stageCollection;
            monster.transform.localScale = Vector3.one * 0.25f;
            monster.GetComponent<ObjectTouchHandler>().enabled = true;

            monsterItemDict.Add(monster, totalEarning);
            return monster;
        }

        public async UniTask PrepareCollection()
        {
            Debug.Log("PREPARE COLLECTION: " + stageData.index);
            foreach (var stageCollection in stageData.stageCollections)
            {
                CardData cardData = DataManager.Instance.userData.inventory.GetCollection(stageCollection.collectionId);
                if (cardData != null)
                {
                    //Debug.Log("= >>>>>>>  COLLECTION: " + collectionData.id);
                    await PrepareMonster(cardData, stageCollection, stageCollection.position.Vector3());
                }
            }
        }

        public ObscuredInt GetTotalEarning()
        {
            totalEarning = CaculateEarning();
            totalEarningBonus = CaculateEarningBonus();
            return (ObscuredInt)(totalEarning * (100 + totalEarningBonus) / 100);
        }

        private ObscuredInt CaculateEarning()
        {
            ObscuredInt totalEarning = 0;
            foreach (int pack in monsterItemDict.Values)
            {
                totalEarning += pack;
            }

            return totalEarning;
        }

        private ObscuredFloat CaculateEarningBonus()
        {
            ObscuredFloat totalBonus = 0;

            return totalBonus;
        }

        public ObscuredInt RewardEarning()
        {
            return GetTotalEarning();
        }
        //public int CaculateOfflineEarning()
        //{
        //    if (DataManagement.DataManager.Instance.userData.stageListData.lastEarningDate == 0) return 0;
        //    int totalOfflineSeconds = (int)System.DateTime.Now.Subtract(new System.DateTime(DataManagement.DataManager.Instance.userData.stageListData.lastEarningDate)).TotalSeconds;
        //    if (totalOfflineSeconds >= Game.Controller.Instance.gameConfig.maxOfflineEarningSeconds)
        //    {
        //        Debug.Log("EXCEEDED OFFLINE EARNING TIME");
        //        totalOfflineSeconds = Game.Controller.Instance.gameConfig.maxOfflineEarningSeconds;
        //    }


        //    int totalOfflineEarning = totalOfflineSeconds * (ObscuredInt)(GetTotalEarning() );

        //    Debug.Log("TOTAL EARNING: " + totalOfflineEarning + " in " + totalOfflineSeconds);
        //    stageData.lastEarningDate = System.DateTime.Now.Ticks;

        //    return totalOfflineEarning;
        //}

        public async UniTask<bool> OnMonsterSelected(CardData cardData, Vector2 position)
        {
            if (stageData.isLocked)
            {
                UI.PanelManager.Create(typeof(MessagePanel), (panel, op) => { ((MessagePanel)panel).SetUp("This stage is locked"); });
                return false;
            }
            else if (stageData.stageCollections.Count >= stageData.totalMonsterSlot)
            {
                Debug.Log("EXCEED MAX SLOT");


                return false;
            }

            // * cập nhật lại vị trí hiện tại của model, lưu lại vào trong stateData
            StageCollectionData stageCollection = new StageCollectionData(cardData.id, position);
            stageData.stageCollections.Add(stageCollection);

            cardData.state = ECollectionState.InUse;

            DataManagement.DataManager.Instance.Save();
            DataManagement.DataManager.Instance.userData.inventory.Update();
            await PrepareMonster(cardData, stageCollection, position);
            return true;
        }

        public void RemoveMonster(Monster monster)
        {
            stageData.RemoveCollection(monster.stageCollectionData);
            monsterItemDict.Remove(monster);

            DataManagement.DataManager.Instance.Save();
        }

        bool isMonsterHidden = false;

        public void HideMonster()
        {
            isMonsterHidden = true;
            foreach (Monster monster in monsterItemDict.Keys)
            {
                monster.gameObject.SetActive(false);
            }
        }

        public void ShowMonster()
        {
            isMonsterHidden = false;
            foreach (Monster monster in monsterItemDict.Keys)
            {
                monster.gameObject.SetActive(true);
            }
        }

        public void ShowEarnEffect(bool manual)
        {
            if (isMonsterHidden) return;
            foreach (var monster in monsterItemDict.Keys)
            {
                Effect.EffectSpawner.Instance.Get(7, result => { (result).Active(monster.cashEffectPlace.position + (Vector3)UnityEngine.Random.insideUnitCircle * 0.3f, monsterItemDict[monster]).SetColor(manual ? Color.yellow : Color.white).SetParent(monster.cashEffectPlace); }).Forget();
            }
        }
    }
}