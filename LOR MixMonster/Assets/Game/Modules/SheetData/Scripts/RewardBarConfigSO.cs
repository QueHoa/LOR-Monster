using Sheet;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/RewardBarConfig")]
public class RewardBarConfigSO : ScriptableObject
{
    public int[] views;

    public void ApplyData(System.Collections.Generic.List<RowData> rowDatas)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif

        views = new int[rowDatas.Count - 1];
        for(int i = 0; i < views.Length; i++)
        {
            views[i] = int.Parse(rowDatas[i + 1].list[1]);
        }
    }
}