using Sheet;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

[CreateAssetMenu(menuName = "SO/RewardGoldConfig")]
public class RewardGold : ScriptableObject
{
    public List<Point> item = new List<Point>();
    public void ApplyData(System.Collections.Generic.List<RowData> rowDatas)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        item.Clear();
        for (int i = 1; i < rowDatas.Count; i++)
        {
            Debug.Log(i);
            Point point = new Point()
            {
                like = int.Parse(rowDatas[i].list[1]),
                gold = new RangeValue(int.Parse(rowDatas[i].list[2]), int.Parse(rowDatas[i].list[3])),
            };
            item.Add(point);
        }
    }
    [System.Serializable]
    public struct Point
    {
        public int like;
        public RangeValue gold;
    }
}