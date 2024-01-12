using Cysharp.Threading.Tasks;
using ItemData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHead : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer eye, accessory, mouth,head;

    public async UniTask<GameObject> SetItem(ItemData.Item item)
    {
        switch (item.category)
        {
            case ItemData.Category.Head:
                head.sprite = await item.GetTextureAsync();
                return head.gameObject;
            case ItemData.Category.Accessory:
                accessory.sprite=await item.GetTextureAsync();
                return accessory.gameObject;
            case ItemData.Category.Eye:
                eye.sprite=await item.GetTextureAsync();
                return eye.gameObject;
            case ItemData.Category.Mouth:
                mouth.sprite=await item.GetTextureAsync();
                return mouth.gameObject;

        }
        return null;
    }

    public GameObject GetItemPlace(Item item)
    {
        switch (item.category)
        {
            case ItemData.Category.Head:
                return head.gameObject;
            case ItemData.Category.Accessory:
                return accessory.gameObject;
            case ItemData.Category.Eye:
                return eye.gameObject;
            case ItemData.Category.Mouth:
                return mouth.gameObject;
        }
        return null;
    }
}
