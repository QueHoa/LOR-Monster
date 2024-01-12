using CodeStage.AntiCheat.ObscuredTypes;
using ItemData;

public class RewardPackage
{
    public ItemData.Item item;
    public CodeStage.AntiCheat.ObscuredTypes.ObscuredInt total;

    public RewardPackage()
    {
    }

    public RewardPackage(Item item, ObscuredInt total)
    {
        this.item = item;
        this.total = total;
    }
}