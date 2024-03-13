using Cysharp.Threading.Tasks;
using DataManagement;
using Game.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageViewHandler : MonoBehaviour
{
    public Transform[] spawnPoints;
    public Dictionary<ItemData.EStageItemCategory, GameObject> itemObjs = new Dictionary<ItemData.EStageItemCategory, GameObject>();
    public Dictionary<ItemData.EStageItemCategory, GameObject> itemPreview = new Dictionary<ItemData.EStageItemCategory, GameObject>();
    public async UniTask SetItem(ItemData.StageItem item, int stageIndex)
    {
        if (string.IsNullOrEmpty(item.mainTexture))
        {
            if (itemObjs.ContainsKey(item.category))
            {
                itemObjs[item.category].SetActive(false);
                itemObjs.Remove(item.category);
            }
            return;
        }
        
        Transform spawnPoint = spawnPoints[(int)item.category];

        GameObject obj = (await GameObjectSpawner.Instance.GetAsync(item.mainTexture)).gameObject;
        obj.transform.SetParent(spawnPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<StageItemObject>().SetUp(stageIndex);
        if (itemObjs.ContainsKey(item.category))
        {
            itemObjs[item.category].GetComponent<StageItemObject>().Hide();
            itemObjs[item.category] = obj;
            obj.GetComponent<StageItemObject>().Show();

        }
        else
        {
            itemObjs.Add(item.category, obj);
            if(((StageGameController)Game.Controller.Instance.gameController).homePanel == null)
            {
                obj.GetComponent<StageItemObject>().Show();
            }
        }
    }
    public async UniTask PreviewItem(ItemData.StageItem item, int stageIndex)
    {
        if (string.IsNullOrEmpty(item.mainTexture))
        {
            if (itemPreview.ContainsKey(item.category))
            {
                itemPreview[item.category].SetActive(false);
                itemPreview.Remove(item.category);
            }
            return;
        }
        foreach (var itemStage in itemPreview)
        {
            if (itemPreview.ContainsKey(item.category))
            {
                itemPreview[itemStage.Key].GetComponent<StageItemObject>().Hide();
            }
        }
        Transform spawnPoint = spawnPoints[(int)item.category];

        GameObject obj = (await GameObjectSpawner.Instance.GetAsync(item.mainTexture)).gameObject;
        obj.transform.SetParent(spawnPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<StageItemObject>().SetUp(stageIndex);
        if (!itemPreview.ContainsKey(item.category))
        {
            itemPreview.Add(item.category, obj);
            obj.GetComponent<StageItemObject>().PreView();
        }
    }
    public void RestorePreview()
    {
        foreach (var item in itemObjs)
        {
            Destroy(item.Value);
            itemObjs[item.Key] = null;
        }
    }

    public void SetUp(StageData currentStageData, List<ItemData.StageItem> items)
    {
        foreach (ItemData.StageItem item in items)
        {
            SetItem(item, currentStageData.index);
        }
        transform.localPosition = new Vector3(160 * currentStageData.index, 0);
        gameObject.SetActive(true);
    }
}
