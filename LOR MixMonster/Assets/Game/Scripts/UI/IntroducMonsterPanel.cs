using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroducMonsterPanel : UI.Panel
{
    GameUtility.Pooling.PoolHandler pool;
    [SerializeField]
    private AudioClip rewardSFX;
    public override void PostInit()
    {
    }
    public void SetUp(List<ItemData.Item> rewardItems)
    {
        Sound.Controller.Instance.PlayOneShot(rewardSFX);
        pool = GetComponentInChildren<GameUtility.Pooling.PoolHandler>();

        foreach (ItemData.Item item in rewardItems)
        {
            RewardSlot rewardSlot = pool.Get().GetComponent<RewardSlot>();
            rewardSlot.SetUp(item);
        }

        Show();
    }
}
