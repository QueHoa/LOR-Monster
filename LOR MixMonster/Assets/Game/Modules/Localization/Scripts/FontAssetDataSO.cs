using UnityEngine;
[CreateAssetMenu]
public class FontAssetDataSO:ScriptableObject
{
    public FontData[] fontDatas;
    [System.Serializable]
    public class FontData
    {
        public TMPro.TMP_FontAsset[] fonts;

    }
}