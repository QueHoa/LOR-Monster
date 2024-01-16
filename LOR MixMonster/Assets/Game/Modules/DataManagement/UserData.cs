using CodeStage.AntiCheat.ObscuredTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;

namespace DataManagement
{
    [System.Serializable]
    public class UserData
    {
        public delegate void OnUpdate(UserData userData);
        [System.NonSerialized]
        public OnUpdate onUpdate;
        protected ObscuredBool isAd = true;
        public ObscuredBool IsAd
        {
            get
            {
                return isAd;
            }
            set
            {
                isAd = value;
                onUpdate?.Invoke(this);
            }
        }
        public ProgressData progressData;
        public CollectionData collectionData;
        public Inventory inventory;
        public AchievementData achievementData;
        public UserStageData stageListData;

        public int BestView { get => bestView; set => bestView = Mathf.Max(value, bestView); }
        private int bestView;
        public int YourGold { get => yourGold; set => yourGold = value; }
        private int yourGold;
        public UserData()
        {

        }
        public void Init()
        {
            progressData = new ProgressData();
            collectionData = new CollectionData();
            inventory = new Inventory();
            achievementData = new AchievementData();
            stageListData = new UserStageData();
        }
      

    }

    [System.Serializable]
    public class AchievementData
    {
        public Dictionary<string, Achievement> achievements=new Dictionary<string, Achievement>();

        public AchievementData()
        {
            //List<CollectionData> collectionDatas = Game.Controller.Instance.gameConfig.collectionData.collectionDatas;
            //foreach (CollectionData collectionData in collectionDatas)
            //{
            //    achievements.Add(collectionData.id, new Achievement(collectionData.id,false,false));
            //}
        }
        public Achievement GetAchievement(string id)
        {
            return achievements[id];
        }
        public bool IsAnythingAvailableToCollect()
        {
            foreach(Achievement achievement in achievements.Values)
            {
                if(achievement.isFinished && !achievement.isClaimed)
                {
                    return true;
                }
            }
            return false;
        }
    }
    [System.Serializable]
    public class BoosterData
    {
        public delegate void OnUpdate(BoosterData boosterData);
        [System.NonSerialized]
        public OnUpdate onUpdate;
        public EBooster boosterType;
        public long lastUseBooster, lastUseFree;
        public int amount;

        public BoosterData()
        {
        }

        public BoosterData(EBooster boosterType, int amount)
        {
            this.boosterType = boosterType;
            this.amount = amount;
        }

        public int Amount
        {
            get => amount;
            set
            {
                amount = value;
                onUpdate?.Invoke(this);
            }
        }
    }
    [System.Serializable]
    public class UserStageData
    {
        public List<StageData> stageDatas;
        public long lastEarningDate;
        public int offlineEarningLevel = 0;

        public List<BoosterData> boosters;

        public UserStageData()
        {
            stageDatas = new List<StageData>();
            boosters = new List<BoosterData>();
        }

        public StageData GetStage(int stageId)
        {
            return stageDatas[stageId];
        }

        public void AddStage(StageData stageData)
        {
            stageData.index = stageDatas.Count;
            stageDatas.Add(stageData);
        }
    }
    [System.Serializable]
    public class StageData
    {
        public int index = 0;
        public List<StageCollectionData> stageCollections;
        public string[] stageItems;

        public int totalModelSlot = 5;
        public bool isLocked = true;

        public StageData()
        {
            stageCollections = new List<StageCollectionData>();
            stageItems = new string[System.Enum.GetNames(typeof(ItemData.EStageItemCategory)).Length];
        }

        public void RemoveCollection(StageCollectionData stageCollectionData)
        {
            stageCollections.Remove(stageCollectionData);
        }
    }
    [System.Serializable]
    public class StageCollectionData
    {
        public string collectionId;
        public long createDate;
        public Vector position;
        public List<Vector> petPositions = new List<Vector>();
        private static Vector2[] defaultPositions = new Vector2[3] { new Vector2(-10, -5), new Vector2(0, -13), new Vector2(10, -5) };

        public StageCollectionData(string collectionId, Vector2 position)
        {
            this.collectionId = collectionId;
            createDate = System.DateTime.Now.Ticks;
            this.position = new Vector(position);
            for (int i = 0; i < defaultPositions.Length; i++)
            {
                petPositions.Add(new Vector(position + defaultPositions[i]));
            }
        }

        public StageCollectionData()
        {
        }
    }

    [System.Serializable]
    public class Achievement
    {
        public string id;
        public bool isFinished,isClaimed;
        public Achievement()
        {

        }

        public Achievement(string id, bool isFinished, bool isClaimed)
        {
            this.id = id;
            this.isFinished = isFinished;
            this.isClaimed = isClaimed;
        }
    }
    
    [System.Serializable]
    public class ProgressData
    {
        public delegate void OnUpdate(ProgressData progressData);
        [System.NonSerialized]
        public OnUpdate onUpdate;
        [JsonProperty("iprog")]
        public Dictionary<string, int> itemAdProgress = new Dictionary<string, int>();
      
        public ObscuredInt  collectionCollectAdCount = 0, comboRewardTrack=0,totalSessionOfToday=0;
        public System.DateTime lastLoggedIn;
        public int bestViewPoint,playCount;
        public bool firstSelect = true;
        public bool firstPet = true, hideCard = false;
        public bool music = true, sound = true, vibration = true;
        public int firstDaily = 0;
        public List<CollectionData> collectionDatas = new List<CollectionData>();

        public int GetAdProgress(string id)
        {
            int result = 0;
            itemAdProgress.TryGetValue(id, out result);
            return result;

        }
        public void SetAdProgress(string id,int value)
        {
            if (itemAdProgress.ContainsKey(id))
            {
                itemAdProgress[id] = value;
            }
            else
            {
                itemAdProgress.Add(id, value);
            }
        }
        public bool IsNewDay()
        {
            return System.DateTime.Now.Date.CompareTo(lastLoggedIn) > 0;
        }
        public void CheckTotalSessionOfDay()
        {
            if (IsNewDay())
            {
                totalSessionOfToday = 1;
                lastLoggedIn = System.DateTime.Now.Date;
            }
            else
            {
                totalSessionOfToday++;
            }
        }

        public ProgressData()
        {
        }
    }
    [System.Serializable]
    public class CardData
    {
        public ECollectionState state;
        public string id;
        public int model;
        public string eventId;
        public string[] items;

        public CardData()
        {
        }

        /*public CardData(int modelId, List<ItemData.ModelItem> currentItems)
        {
            this.id = Guid.NewGuid().ToString();
            this.model = modelId;
            items = new string[currentItems.Count];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = currentItems[i].id;
            }
        }*/
    }
    public enum ECollectionState
    {
        Available,
        InUse
    }
    [System.Serializable]
    public class CollectionData
    {
        public delegate void OnUpdate(CollectionData collectionData);
        [System.NonSerialized]
        public OnUpdate onUpdate;

        public int view, like, numberPhoto;
        public List<ItemData.Item> collectionItem = new List<ItemData.Item>();

        public CollectionData(int view, int like, int numberPhoto, List<ItemData.Item> collectionItem)
        {
            this.view = view;
            this.like = like;
            this.numberPhoto = numberPhoto;
            this.collectionItem = collectionItem;
        }
        public CollectionData()
        {

        }
    }
    [System.Serializable]
    public class Inventory
    {
        public delegate void OnUpdate(Inventory inventory);
        [System.NonSerialized]
        public OnUpdate onUpdate;

        public delegate void OnCashUpdated(Inventory inventory, ObscuredInt cash);
        [System.NonSerialized]
        public OnCashUpdated onCashUpdated;

        public ObscuredInt cash;
        public ObscuredInt Cash
        {
            get => cash;
            set
            {
                cash = value;
                onCashUpdated?.Invoke(this, cash);
            }
        }

        public ObscuredInt cake;
        public ObscuredInt Cake
        {
            get => cake;
            set { cake = value; }
        }

        [JsonProperty("iprog")]
        public Dictionary<string, int> itemStates = new Dictionary<string, int>();
        [JsonProperty("collection")]
        public Dictionary<string, CollectionData> collections = new Dictionary<string, CollectionData>();


        public int GetItemState(string id)
        {
            int result = 0;
            itemStates.TryGetValue(id, out result);
            return result;
        }

        public void SetItemState(string id, int value)
        {
            if (itemStates.ContainsKey(id))
            {
                itemStates[id] = value;
            }
            else
            {
                itemStates.Add(id, value);
            }
        }

        public void AddCollection(CollectionData modelData)
        {
            //collections.Add(modelData.id, modelData);
            Update();
        }

        public void RemoveCollection(string id)
        {
            collections.Remove(id);
            Update();
        }

        public CollectionData GetCollection(string id)
        {
            if (collections.Count == 0 || !collections.ContainsKey(id)) return null;
            return collections[id];
        }

        public CollectionData GetFirstCollection()
        {
            CollectionData[] temp = new CollectionData[collections.Values.Count];
            collections.Values.CopyTo(temp, 0);
            for (int i = temp.Length - 1; i >= 0; i--)
            {
                CollectionData collection = temp[i];
                //if (collection.state == ECollectionState.Available)
                    return collection;
            }

            return null;
        }

        public int GetTotalCollection()
        {
            int total = 0;
            foreach (var collection in collections.Values)
            {
                /*if (collection.state == ECollectionState.Available)
                {
                    total++;
                }*/
            }

            return total;
        }

        public void Update()
        {
            onUpdate?.Invoke(this);
        }

        public Inventory()
        {
        }
    }

    [System.Serializable]
    public class Item
    {
        public ObscuredString id;
        public ObscuredInt total;

        public Item(ObscuredString id, ObscuredInt total)
        {
            this.id = id;
            this.total = total;
        }

        public Item()
        {
        }
        public void Set(ObscuredString id, ObscuredInt total)
        {
            this.id = id;
            this.total = total;
        }
        public ItemData.Item GetItemMetadata()
        {
            return Game.Controller.Instance.itemData.GetItem(id);
        }
    }

    [System.Serializable]
    public class MergeTableData
    {
        public delegate void OnUpdate(MergeTableData mergeTable);
        public OnUpdate onUpdate;
        public MergeSlotData[] mergeSlotDatas;

        public MergeTableData()
        {
        }

        internal void ClearEquip()
        {
            foreach(MergeSlotData slotData in mergeSlotDatas)
            {
                slotData.isEquiped = false;
            }
        }
    }
    [System.Serializable]
    public class MergeSlotData
    {
        public delegate void OnUnlock(MergeSlotData slot);
        public static OnUnlock onUnlock;
        public int id;
        public string itemId;
        public int level;
        public bool isCoinLock, isAdLock, isAvailable,isEquiped;

        public MergeSlotData()
        {
        }

        public MergeSlotData(int id, string itemId, bool isAdLock, bool isCoinLock, bool isAvailable)
        {
            this.id = id;
            this.itemId = itemId;
            this.isCoinLock = isCoinLock;
            this.isAdLock = isAdLock;
            this.isAvailable = isAvailable;
        }
        public bool IsEquiped()
        {
            return isEquiped;
        }
        //does slot contain any item? 
        public bool IsUsing()
        {
            return !string.IsNullOrEmpty(itemId);
        }
        //does slot ready for setting up new item? 
        public bool IsReady()
        {
            return isAvailable && !isAdLock && !isCoinLock && string.IsNullOrEmpty(itemId);
        }
        // is slot available for any action
        public bool IsAvailable()
        {
            return isAvailable && !isAdLock && !isCoinLock;
        }
        // check if slot could unlock
        public bool CanUnlock()
        {
            return isAvailable && (isAdLock ||isCoinLock);
        }
        public void ApplyItem(string itemId, int level)
        {
            this.itemId = itemId;
            this.level = level;
        }
        public void ClearItem()
        {
            this.itemId = null;
        }
        public void Unlock()
        {
            isAvailable = true;
            isAdLock = false;
            isCoinLock = false;
            onUnlock?.Invoke(this);
        }

        public void Equip()
        {
            isEquiped = true;
        }


    }


}
