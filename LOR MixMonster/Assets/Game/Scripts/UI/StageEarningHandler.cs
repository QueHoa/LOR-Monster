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
        public StageViewHandler stageView;
        public StageData stageData;
        public ObscuredInt totalEarning;
        public ObscuredFloat totalEarningBonus;
        public Dictionary<Monster, int> monsterItemDict = new Dictionary<Monster, int>();
        public List<ItemData.StageItem> stageItems = new List<ItemData.StageItem>();
        int musicThemeIndex = 0;

        public StageEarningHandler(StageData stageData)
        {
            this.stageData = stageData;
        }
        public async UniTask PrepareStageView()
        {
            stageView = (await GameObjectSpawner.Instance.GetAsync("StagePlatform")).GetComponent<StageViewHandler>();
            stageItems.Clear();
            for (int i = 0; i < stageData.stageItems.Length; i++)
            {
                string itemId = stageData.stageItems[i];
                if (string.IsNullOrEmpty(itemId))
                {
                    stageItems.Add(new ItemData.StageItem() { category = (ItemData.EStageItemCategory)i });
                    continue;
                }

                ItemData.StageItem item = (ItemData.StageItem)Sheet.SheetDataManager.Instance.gameData.itemData.GetStageItem(itemId);
                stageItems.Add(item);
            }

            stageView.SetUp(stageData, stageItems);
        }
        public async UniTask<Monster> PrepareMonster(CardData cardData, StageCollectionData stageCollection, Vector2 position)
        {
            ItemData.ItemPack itemPack = new ItemData.ItemPack();
            List<ItemData.Item> items = new List<ItemData.Item>();
            ObscuredInt totalEarning = cardData.money;
            Monster monster = (await ObjectSpawner.Instance.GetAsync(2)).GetComponent<Monster>();
            foreach (string id in cardData.items)
            {
                if (Game.Controller.Instance.itemData.GetItem(id).category != ItemData.Category.Pet)
                {
                    items.Add(Game.Controller.Instance.itemData.GetItem(id));
                }
            }
            
            await monster.SetUp(items);
            musicThemeIndex = DataManagement.DataManager.Instance.userData.progressData.playCount == 0 ? 4 : UnityEngine.Random.Range(0, Sound.Controller.Instance.soundData.finalThemes.Length);
            monster.Dance(musicThemeIndex % Sound.Controller.Instance.soundData.finalThemes.Length);
            monster.transform.position = position;
            monster.stageCollectionData = stageCollection;
            monster.transform.localScale = Vector3.one * 0.25f;
            monster.GetComponent<BoxCollider2D>().enabled = true;
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
            foreach (ItemData.StageItem item in stageItems)
            {
                if (item != null)
                    totalBonus += item.bonusEarning;
            }

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
        public void OnStageItemSelected(ItemData.StageItem stageItem)
        {
            string currentEquipedItem = stageData.stageItems[(int)stageItem.category];
            if (!string.IsNullOrEmpty(currentEquipedItem))
            {
                DataManagement.DataManager.Instance.userData.inventory.SetItemState($"{stageData.index}_{currentEquipedItem}", 1);
            }

            stageData.stageItems[(int)stageItem.category] = stageItem.id;
            stageItems[(int)stageItem.category] = stageItem;

            DataManagement.DataManager.Instance.userData.inventory.SetItemState($"{stageData.index}_{stageItem.id}", 2);
            DataManagement.DataManager.Instance.Save();
            if (lastEquipedItems.ContainsKey(stageItem.category))
            {
                lastEquipedItems[stageItem.category] = stageItem;
            }

            stageView.SetItem(stageItem, stageData.index);
        }

        Dictionary<ItemData.EStageItemCategory, ItemData.StageItem> lastEquipedItems = new Dictionary<ItemData.EStageItemCategory, ItemData.StageItem>();

        public void OnStageItemPreview(ItemData.StageItem stageItem)
        {
            if (!lastEquipedItems.ContainsKey(stageItem.category))
            {
                Debug.Log("SET EQUIP " + stageItem.category + " " + (stageItems[(int)stageItem.category] == null));

                lastEquipedItems.Add(stageItem.category, stageItems[(int)stageItem.category]);
            }

            stageView.PreviewItem(stageItem, stageData.index);
        }
        public void RestorePreview()
        {
            if (lastEquipedItems.Count == 0) return;
            foreach (var item in lastEquipedItems)
            {
                Debug.Log("REMOVE" + item.Key + " " + (item.Value == null));
                stageView.PreviewItem(item.Value, stageIndex: stageData.index);
            }

            lastEquipedItems.Clear();
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
        public void ShowEarnEffectSelect(Monster monster)
        {
            if (isMonsterHidden) return;
            Effect.EffectSpawner.Instance.Get(7, result => { (result).Active(monster.cashEffectPlace.position + (Vector3)UnityEngine.Random.insideUnitCircle * 0.3f, monsterItemDict[monster]).SetColor(Color.green).SetParent(monster.cashEffectPlace); }).Forget();
            
        }
    }
}