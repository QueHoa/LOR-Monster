using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DataManagement;
//using Game.Pool;
using System.Collections.Generic;
using UnityEngine;

public partial class StageGameController
{
    [System.Serializable]
    public class StageEarningHandler
    {
        /*public StageViewHandler stageView;
        public StageData stageData;
        public ObscuredInt totalEarning;
        public ObscuredFloat totalEarningBonus;
        public Dictionary<Model, (ItemData.ModelItemPack, int)> modelItemDict = new Dictionary<Model, (ItemData.ModelItemPack, int)>();
        public Dictionary<Model, List<Pet>> modelPetDict = new Dictionary<Model, List<Pet>>();
        public List<ItemData.StageItem> stageItems = new List<ItemData.StageItem>();

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

                ItemData.StageItem item = (ItemData.StageItem)Sheet.SheetDataManager.Instance.gameData.itemData.GetItem(itemId);
                stageItems.Add(item);
            }

            stageView.SetUp(stageData, stageItems);
        }

        public async UniTask<Model> PrepareModel(CollectionData collectionData, StageCollectionData stageCollection, Vector2 position)
        {
            ItemData.ModelItemPack itemPack = new ItemData.ModelItemPack();
            List<Pet> pets = new List<Pet>();
            ObscuredInt totalEarning = 0;
            foreach (string itemId in collectionData.items)
            {
                ItemData.ModelItem item = (ItemData.ModelItem)Sheet.SheetDataManager.Instance.gameData.itemData.GetItem(itemId);
                itemPack.items.Add(item);

                totalEarning += (item.unlockType == ItemData.UnlockType.None && item.category != ItemData.EModelItemCategory.Pet) ? Sheet.SheetDataManager.Instance.gameData.stageConfig.cashEarningForNormalItem : Sheet.SheetDataManager.Instance.gameData.stageConfig.cashEarningForAdItem;

                if (item.category == ItemData.EModelItemCategory.Pet && stageCollection.petPositions.Count != 0)
                {
                    Pet pet = (await ObjectSpawner.Instance.GetAsync(1)).GetComponent<Pet>();
                    //Debug.Log("pet:" + stageCollection.petPositions.Count+ " "+pets.Count+" "+item.category+" "+item.id);
                    Vector3 petPos = stageCollection.petPositions[pets.Count].Vector3();
                    pet.SetUp(item.skin, petPos);
                    pet.index = pets.Count;
                    pet.stageCollectionData = stageCollection;
                    pet.transform.localScale = Vector3.one * 1.2f;
                    pet.GetComponent<PetTouchHandler>().enabled = true;

                    pets.Add(pet);
                }
            }

            Model model = (await ObjectSpawner.Instance.GetAsync(string.IsNullOrEmpty(collectionData.eventId) ? 2 : 4)).GetComponent<Model>();
            if (string.IsNullOrEmpty(collectionData.eventId))
            {
                model.SetUp(collectionData.model);
                model.SetItem(itemPack.items);
            }
            else
            {
                model.SetSkin(collectionData.eventId);
            }

            model.Pose();
            model.transform.position = position;
            model.stageCollectionData = stageCollection;
            model.transform.localScale = Vector3.one * 1.2f;
            model.GetComponent<ObjectTouchHandler>().enabled = true;
            modelItemDict.Add(model, (itemPack, totalEarning));
            modelPetDict.Add(model, pets);
            return model;
            //stageView.AddModel(model,models.Count-1);
        }

        public async UniTask PrepareCollection()
        {
            Debug.Log("PREPARE COLLECTION: " + stageData.index);
            foreach (var stageCollection in stageData.stageCollections)
            {
                CollectionData collectionData = DataManager.Instance.userData.inventory.GetCollection(stageCollection.collectionId);
                if (collectionData != null)
                {
                    //Debug.Log("= >>>>>>>  COLLECTION: " + collectionData.id);
                    await PrepareModel(collectionData, stageCollection, stageCollection.position.Vector3());
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
            foreach ((ItemData.ModelItemPack, int) pack in modelItemDict.Values)
            {
                totalEarning += pack.Item2;
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

        public async UniTask<bool> OnModelSelected(CollectionData collectionData, Vector2 position)
        {
            if (stageData.isLocked)
            {
                UI.PanelManager.Create(typeof(MessagePanel), (panel, op) => { ((MessagePanel)panel).SetUp("This stage is locked"); });
                return false;
            }
            else if (stageData.stageCollections.Count >= stageData.totalModelSlot)
            {
                Debug.Log("EXCEED MAX SLOT");


                return false;
            }

            // * cập nhật lại vị trí hiện tại của model, lưu lại vào trong stateData
            StageCollectionData stageCollection = new StageCollectionData(collectionData.id, position);
            stageData.stageCollections.Add(stageCollection);

            collectionData.state = ECollectionState.InUse;

            DataManagement.DataManager.Instance.Save();
            DataManagement.DataManager.Instance.userData.inventory.Update();
            await PrepareModel(collectionData, stageCollection, position);
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

            stageView.SetItem(stageItem, stageData.index);
        }

        public void RestorePreview()
        {
            if (lastEquipedItems.Count == 0) return;
            foreach (var item in lastEquipedItems)
            {
                Debug.Log("REMOVE" + item.Key + " " + (item.Value == null));
                stageView.SetItem(item.Value, stageIndex: stageData.index);
            }

            lastEquipedItems.Clear();
        }

        public void RemoveModel(Model model)
        {
            stageData.RemoveCollection(model.stageCollectionData);
            modelItemDict.Remove(model);

            foreach (var pet in modelPetDict[model])
            {
                pet.gameObject.SetActive(false);
            }

            DataManagement.DataManager.Instance.Save();
        }

        bool isModelHidden = false;

        public void HideModel()
        {
            isModelHidden = true;
            foreach (Model model in modelItemDict.Keys)
            {
                model.gameObject.SetActive(false);
                foreach (Pet pet in modelPetDict[model])
                {
                    pet.gameObject.SetActive(false);
                }
            }
        }

        public void ShowModel()
        {
            isModelHidden = false;
            foreach (Model model in modelItemDict.Keys)
            {
                model.gameObject.SetActive(true);
                foreach (Pet pet in modelPetDict[model])
                {
                    pet.gameObject.SetActive(true);
                }
            }
        }

        public void ShowEarnEffect(bool manual)
        {
            if (isModelHidden) return;
            foreach (var model in modelItemDict.Keys)
            {
                Effect.EffectSpawner.Instance.Get(7, result => { (result).Active(model.cashEffectPlace.position + (Vector3)UnityEngine.Random.insideUnitCircle * 3f, modelItemDict[model].Item2).SetColor(manual ? Color.yellow : Color.white).SetParent(model.cashEffectPlace); }).Forget();
            }
        }*/
    }
}