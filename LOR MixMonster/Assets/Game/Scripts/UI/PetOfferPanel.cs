using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetOfferPanel : UI.Panel
{
    System.Action onSelected;
    [SerializeField]
    private Image iconImg;
    public override void PostInit()
    {
    }
    public void SetUp(ItemData.Item item,System.Action onSelected)
    {
        this.onSelected = onSelected;

        item.GetIcon(sprite => iconImg.sprite = sprite);
        Show();
    }
    public void WatchAd()
    {
        AD.Controller.Instance.ShowRewardedAd("Pet", res =>
        {
            if (res)
            {
                ClaimReward();
            }
        });
    }
    void ClaimReward()
    {
        onSelected?.Invoke();
    }
}
