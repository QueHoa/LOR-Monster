using Sheet;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/StageConfig")]
public class StageConfigSO : ScriptableObject
{
    public int cashEarningForNormalItem = 10;
    public int cashEarningForAdItem = 30;
    public int expandPrice = 300;
    public List<SlotConfig> slotConfigs = new List<SlotConfig>();
    public int boosterRecoverTime;

    public List<BoosterConfig> boosterConfigs = new List<BoosterConfig>();

    [System.Serializable]
    public class BoosterConfig
    {
        public EBooster boosterType;
        public List<Stat> stats;
        private Dictionary<string, Stat> statDict = new Dictionary<string, Stat>();
        public BoosterConfig(EBooster boosterType, params Stat[] stats)
        {
            this.boosterType = boosterType;
            this.stats = new List<Stat>(stats);
        }
        public Stat GetStat(string key)
        {
            if (statDict == null) statDict = new Dictionary<string, Stat>();
            if (statDict.Count == 0)
            {
                foreach (Stat stat in stats)
                {
                    statDict.Add(stat.statType, stat);
                }
            }
            return statDict[key];
        }
    }
    [System.Serializable]
    public class Stat
    {
        public string statType;
        public float value;

        public Stat(string statType, float value)
        {
            this.statType = statType;
            this.value = value;
        }
    }


    [System.Serializable]
    public struct SlotConfig
    {
        public int maxSlot;
        public int cashRequire, adRequire;
    }
    [System.Serializable]
    public struct StageConfig
    {
        public int id;
        public int cashRequire, maxExpandSlot;

    }

    public void ApplyData(GSheetData[] sheets)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        List<RowData> rowDatas = GameUtility.GameUtility.ConvertSheetToList(sheets[0].GoogleSheetData);
        cashEarningForAdItem = int.Parse(rowDatas[2].list[1]);
        cashEarningForNormalItem = int.Parse(rowDatas[1].list[1]);

        ApplySlotConfig(sheets[1].GoogleSheetData);
        ApplyBoosterConfig(sheets[2].GoogleSheetData);

    }
    void ApplySlotConfig(object[,] googleSheetData)
    {
        List<RowData> rowDatas = GameUtility.GameUtility.ConvertSheetToList(googleSheetData);
        int row = 1;
        slotConfigs.Clear();
        while (row < rowDatas.Count)
        {
            slotConfigs.Add(new SlotConfig()
            {
                maxSlot = int.Parse(rowDatas[row].list[0]),
                cashRequire = int.Parse(rowDatas[row].list[1]),
                adRequire = int.Parse(rowDatas[row].list[2]),
            });
            row++;
        }
    }
    void ApplyBoosterConfig(object[,] googleSheetData)
    {
        List<RowData> rowDatas = GameUtility.GameUtility.ConvertSheetToList(googleSheetData);
        boosterRecoverTime = int.Parse(rowDatas[1].list[8]);

        boosterConfigs.Clear();
        boosterConfigs.Add(new BoosterConfig(EBooster.InstantMoney,
            new Stat(BoosterStatKey.INSTANTCASH, int.Parse(rowDatas[1].list[1]))
            ));

        boosterConfigs.Add(new BoosterConfig(EBooster.SpeedBoost,
        new Stat(BoosterStatKey.EARNSPEED, float.Parse(rowDatas[1].list[3])),
        new Stat(BoosterStatKey.DURATION, int.Parse(rowDatas[1].list[4]))
        ));

        boosterConfigs.Add(new BoosterConfig(EBooster.AutoClick,
          new Stat(BoosterStatKey.AUTOCLICKRATE, float.Parse(rowDatas[1].list[6])),
          new Stat(BoosterStatKey.DURATION, int.Parse(rowDatas[1].list[7]))
          ));


    }

    public BoosterConfig GetBooster(EBooster booster)
    {
        return boosterConfigs[(int)booster];
    }
}
public class BoosterStatKey
{
    public const string INSTANTCASH = "InstantCash";
    public const string EARNSPEED = "EarnSpeed";
    public const string DURATION = "Duration";
    public const string AUTOCLICKRATE = "AutoClickRate";
}