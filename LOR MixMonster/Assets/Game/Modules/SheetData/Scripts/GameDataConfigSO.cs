using UnityEngine;
[CreateAssetMenu(menuName ="SO/GameDataConfigSO")]
public class GameDataConfigSO : ScriptableObject
{
    public string localizationSheetUrl;
    public string itemSheetUrl;
    public string stageSheetUrl;
    public LocalizationCollectionSO localizationData;
    public ItemData.ItemDictionarySO itemData;
    public RewardBarConfigSO rewardBarConfig;
    public StageConfigSO stageConfig;
    public RewardGold rewardGold;
    public GameDataConfigSO Clone()
    {
        GameDataConfigSO instance = CreateInstance<GameDataConfigSO>();
        instance.localizationData = localizationData.Clone();
        instance.localizationSheetUrl = localizationSheetUrl;
        instance.itemData = itemData;
        instance.rewardBarConfig = rewardBarConfig;
        instance.stageConfig = stageConfig;
        instance.rewardGold = rewardGold;
        return instance;
    }
}
