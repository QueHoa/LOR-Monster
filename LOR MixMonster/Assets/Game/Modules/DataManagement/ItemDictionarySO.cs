using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Sheet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ItemData {
    [CreateAssetMenu(menuName = "Item/ItemList")]
    public class ItemDictionarySO : ScriptableObject
    {
        public ItemPack[] packs;
        [HideInInspector]
        public Dictionary<ObscuredString, Item> itemDicts = new Dictionary<ObscuredString, Item>();
        public List<BundleSet> bundleSets = new List<BundleSet>();
        public Item GetItem(string id)
        {
            if(itemDicts==null || itemDicts.Count == 0)
            {
                foreach (ItemPack pack in packs)
                {
                    foreach (Item item in pack.items)
                    {
                        itemDicts.Add(item.id, item);
                    }
                }
            }
            if(itemDicts.ContainsKey(id))
            return itemDicts[id];

            return new Item();
        }
        //public void Populate()
        //{
        //    List<ItemPack> packs = new List<ItemPack>();

        //    for(int i = 0; i < 5;i++)
        //    {
        //        ItemPack pack = new ItemPack();
        //        List<Item> items = new List<Item>();
        //        Category category = (Category)i;
        //        for (int j = 0; j < (category==Category.Body?12:20); j++)
        //        {
        //            Item item = new Item() {
        //                id = category + "_" + j,
        //                category = category,
        //                icon = category!=Category.Body?($"itemicon/{category}_{j}.png").ToLower():$"{category}/{category}_{j}.png".ToLower(),
        //                mainTexture = $"{category}/{category}_{j}.png".ToLower(),
        //                title = category + "_" + j,
        //                likePoint = new RangeValue(),
        //                viewPoint = new RangeValue(),
        //                adRequire = (category == Category.Body && j >= 6) ? 1 : 0,
        //                skin = (category != Category.Body ? 0:j).ToString(),


        //            };
        //            items.Add(item);
        //        }
        //        pack.items = items.ToArray();
        //        packs.Add(pack);
        //    }
        //    this.packs = packs.ToArray();
        //}

        public ItemPack GetPack(Category category)
        {
            return packs[(int)category];
        }

     
        public BundleSet GetBundle(string bundleId)
        {
            foreach (BundleSet bundleSet in bundleSets)
            {
                if (bundleSet.bundleId.Equals(bundleId))
                {
                    return bundleSet;
                }
            }
            return null;
        }
        public BundleSet GetBundleByItemID(string itemId)
        {
            foreach (BundleSet bundleSet in bundleSets)
            {
                foreach (ModelSet modelSet in bundleSet.modelSets)
                {
                    if (modelSet.itemIds.Contains(itemId))
                    {
                        return bundleSet;
                    }
                }
            }
            return null;
        }
        public void ApplyData(GSheetData[] sheets)
        {
            Debug.Log("ITEM");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif

            //  data bundle set
            List<RowData> setRowDatas = GameUtility.GameUtility.ConvertSheetToList(sheets[6].GoogleSheetData);

            bundleSets.Clear();
            int row = 1;
            while (row < setRowDatas.Count)
            {
                BundleSet bundleSet = new BundleSet(bundleId: setRowDatas[row].list[0], setIcon: setRowDatas[row].list[1]);

               
                int setId = 0;
                if (row <= 15)
                {
                    while (setId + 2 < setRowDatas[row].list.Count && !string.IsNullOrEmpty(setRowDatas[row].list[setId + 2]))
                    {
                        ModelSet set = new ModelSet();
                        for (int part = 0; part < 5; part++)
                        {
                            set.itemIds.Add(setRowDatas[row + part].list[setId + 2]);
                        }
                        bundleSet.modelSets.Add(set);
                        setId++;
                    }
                }
                else
                {
                    while (setId + 2 < setRowDatas[row].list.Count && !string.IsNullOrEmpty(setRowDatas[row].list[setId + 2]))
                    {
                        ModelSet set = new ModelSet();
                        for (int part = 0; part < 1; part++)
                        {
                            set.itemIds.Add(setRowDatas[row + part].list[setId + 2]);
                        }
                        bundleSet.modelSets.Add(set);
                        setId++;
                    }
                }
                bundleSets.Add(bundleSet);
                if (row <= 15)
                {
                    row += 5;
                }
                else
                {
                    row += 1;
                }
                
            }
            //prepare item

            // fetch item for all category
            packs = new ItemPack[System.Enum.GetNames(typeof(Category)).Length];
            for (int i = 0; i <packs.Length ; i++)
            {
                ItemPack itemPack = new ItemPack();
                List<RowData> rowDatas = GameUtility.GameUtility.ConvertSheetToList(sheets[i].GoogleSheetData);
                row = 1;
                while (row < rowDatas.Count)
                {
                    Debug.Log(i + " " + (row + 1));
                    if (string.IsNullOrEmpty(rowDatas[row].list[0])) continue;
                    BundleSet bundleSet = GetBundleByItemID((Category)i + "_" + rowDatas[row].list[0]);
                    Item item = new Item()
                    {
                        id = (Category)i + "_" + rowDatas[row].list[0],
                        category = (Category)i,
                        icon = (rowDatas[row].list[7] + ".png").ToLower(),
                        mainTexture = (Category)i == Category.Body ? (rowDatas[row].list[7] + ".png").ToLower() : (rowDatas[row].list[6] + ".png").ToLower(),
                        viewPoint = new RangeValue(int.Parse(rowDatas[row].list[3]), int.Parse(rowDatas[row].list[4])),
                        cost = int.Parse(rowDatas[row].list[8]),
                        unlockType = (UnlockType)(int.Parse(rowDatas[row].list[5])),
                        skin = rowDatas[row].list[6],
                        bundleId = bundleSet == null ? string.Empty : bundleSet.bundleId
                    };

                    itemPack.items.Add(item);

                    row++;
                }
                packs[i] = itemPack;

            }



        }
        public static List<RowData> ConvertSheetToList(object[,] data)
        {
            List<RowData> list = new List<RowData>();
            for (int i = 0; i < data.GetLength(0); i++)
            {
                RowData row = new RowData();
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    string value = data[i, j].ToString();
                    row.list.Add(value);
                }
                list.Add(row);
            }
            return list;
        }
    }
    [System.Serializable]
    public class BundleSet
    {
        public string setIcon;
        public string bundleId;
        public List<ModelSet> modelSets = new List<ModelSet>();

        public BundleSet(string setIcon, string bundleId)
        {
            this.setIcon = setIcon;
            this.bundleId = bundleId;
        }
        public void GetIcon(System.Action<Sprite> onLoad)
        {
            Addressables.LoadAsset<Sprite>(setIcon).Completed += op =>
            {
                Sprite sprite = op.Result;

                onLoad?.Invoke(sprite);

                Addressables.Release(op);
            };
        }
    }
   
    [System.Serializable]
    public class ModelSet
    {
        public List<string> itemIds;

        public ModelSet()
        {
            itemIds = new List<string>();
        }

        public ModelSet(List<string> itemIds)
        {
            this.itemIds = itemIds;
        }
        public List<ItemData.Item> GetItems()
        {
            List<Item> items = new List<Item>();
            foreach (string id in itemIds)
            {
                items.Add(Game.Controller.Instance.itemData.GetItem(id));
            }
            return items;
        }
        public override string ToString()
        {
            return itemIds.Count + " " + itemIds[0];
        }

    }
    [System.Serializable]
    public class ModelPack
    {
        public List<ItemPack> itemPacks;
        public ModelPack()
        {
            itemPacks = new List<ItemPack>();
            for (int i = 0; i < System.Enum.GetNames(typeof(Category)).Length; i++)
            {
                itemPacks.Add(new ItemPack());
            }
        }
    }
    [System.Serializable]
    public class ItemPack
    {
        public List<Item> items=new List<Item>();

        public Item GetRandom()
        {
            int rd = 0;
            do
            {
                rd = Random.Range(0, items.Count);
            } while (items[rd].unlockType != 0);
            return items[rd];
        }
        [System.NonSerialized]
        private List<Item> temp=new List<Item>();
        public void PrepareItemPool(List<Item> pool, List<Item> excludes, List<Item> excludeIds)
        {
            pool.Clear();
            temp.Clear();
            foreach (Item item in items)
            {
                //if item not in a bundle or bundle has been unlocked
                if (string.IsNullOrEmpty(item.bundleId) || (!string.IsNullOrEmpty(item.bundleId) && DataManagement.DataManager.Instance.userData.inventory.GetItemState(item.id) == 1))
                {
                    temp.Add(item);
                }
            }
            foreach (Item item in excludes)
            {
                Debug.Log("CHECK ITEM: " + item.id);
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].icon.Equals(item.icon))
                    {
                        Debug.Log("REMOVE " + temp[i].id);
                        temp.RemoveAt(i);
                        i--;
                    }
                }
            }
            while (temp.Count >= 2)
            {
                bool normalItemAvailable = false;
                foreach (Item item in temp)
                {
                    if (item.unlockType == 0)
                    {
                        normalItemAvailable = true;
                        break;
                    }
                }
                int rd = 0;
                if (normalItemAvailable)
                {
                    do
                    {
                        rd = UnityEngine.Random.Range(0, temp.Count);
                    } while (temp[rd].unlockType == UnlockType.Ad || (pool.Count < 2 && IsExcluded(temp[rd],excludeIds)));
                }
                else
                {
                    rd = UnityEngine.Random.Range(0, temp.Count);
                }
                pool.Add(temp[rd]);
                temp.RemoveAt(rd);
            }
        }
        bool IsExcluded(Item item, List<Item> excludes)
        {
            if (excludes == null) return false;
            foreach(Item i in excludes)
            {
                if (i.id.Equals(item.id))
                {
                    return true;
                }
            }
            return false;
        }
        public void PrepareItemPool_MixedAd(List<Item> pool, List<Item> excludes, List<Item> excludeIds)
        {
            pool.Clear();
            temp.Clear();
            foreach (Item item in items)
            {
                //if item not in a bundle or bundle has been unlocked
                if (string.IsNullOrEmpty(item.bundleId) || (!string.IsNullOrEmpty(item.bundleId) && DataManagement.DataManager.Instance.userData.inventory.GetItemState(item.id) == 1))
                {
                    temp.Add(item);
                }
            }
            foreach (Item item in excludes)
            {
                Debug.Log("CHECK ITEM: " + item.id);
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].icon.Equals(item.icon))
                    {
                        Debug.Log("REMOVE " + temp[i].id);
                        temp.RemoveAt(i);
                        i--;
                    }
                }
            }
            while (temp.Count >= 2)
            {
                int rd = 0;

                //force find a no ad item
                bool normalItemAvailable = false;
                foreach (Item item in temp)
                {
                    if (item.unlockType == 0)
                    {
                        normalItemAvailable = true;
                        break;
                    }
                }
                if (normalItemAvailable)
                {
                    do
                    {
                        rd = UnityEngine.Random.Range(0, temp.Count);
                    } while (temp[rd].unlockType == UnlockType.Ad || (pool.Count < 2 && IsExcluded(temp[rd], excludeIds)));
                }
                else
                {
                    rd = UnityEngine.Random.Range(0, temp.Count);
                }
                pool.Add(temp[rd]);
                temp.RemoveAt(rd);


                //force find a AD item
                bool adItemAvailable = false;
                foreach (Item item in temp)
                {
                    if (item.unlockType != 0)
                    {
                        adItemAvailable = true;
                        break;
                    }
                }
                if (adItemAvailable)
                {
                    do
                    {
                        rd = UnityEngine.Random.Range(0, temp.Count);
                    } while (temp[rd].unlockType != UnlockType.Ad || (pool.Count < 2 && IsExcluded(temp[rd], excludeIds)));
                }
                else
                {
                    do
                    {
                        rd = UnityEngine.Random.Range(0, temp.Count);
                    } while (temp[rd].unlockType == UnlockType.Ad || (pool.Count < 2 && IsExcluded(temp[rd], excludeIds)));
                }
                pool.Add(temp[rd]);
                temp.RemoveAt(rd);


            }
        }
    }
    [System.Serializable]
    public struct Item
    {
        public string id;
        public Category category;
        public string icon,mainTexture,skin;
        public string title;
        public string bundleId;
        public UnlockType unlockType;

        public RangeValue viewPoint;
        public int cost;
        public void GetIcon(System.Action<Sprite> onLoad)
        {
            Addressables.LoadAsset<Sprite>(icon).Completed += op =>
            {
                Sprite sprite = op.Result;

                onLoad?.Invoke(sprite);

                Addressables.Release(op);
            };
        }
        public async UniTask<Sprite> GetIconAsync()
        {
            Sprite sprite = await Addressables.LoadAssetAsync<Sprite>(icon);
            return sprite;
        }
        public void GetTexture(System.Action<Sprite> onLoad)
        {
            Addressables.LoadAsset<Sprite>(mainTexture).Completed += op =>
            {
                Sprite sprite = op.Result;

                onLoad?.Invoke(sprite);

                Addressables.Release(op);
            };
        }
        public async UniTask<Sprite> GetTextureAsync()
        {
            Sprite sprite=await Addressables.LoadAssetAsync<Sprite>(mainTexture);
            return sprite;
        }
    }
    public enum Category
    {
        Head,Eye,Mouth,Accessory,Body,Pet
    }
    public enum UnlockType
    {
        None, Ad
    }
    public enum EStageItemCategory
    {
        Curtain,
        Light,
        Wall,
        Decoration,
        Stage
    }
}