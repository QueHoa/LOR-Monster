using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : GameUtility.Pooling.PoolComponent
{
    [SerializeField]
    private Image icon;
    public void SetUp(ItemData.Item item)
    {
        item.GetIcon(sprite => icon.sprite=sprite);
        gameObject.SetActive(true);
    }
}
