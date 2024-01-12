using UnityEngine;
using UnityEngine.UI;

public class PetSlot:GameUtility.Pooling.PoolComponent
{
    [SerializeField]
    private Image iconImg;
    public void SetUp(ItemData.Item item)
    {
        item.GetIcon(sprite => iconImg.sprite=sprite);
        GetComponent<PanelFadeAnimation>().Show();
    }
}
