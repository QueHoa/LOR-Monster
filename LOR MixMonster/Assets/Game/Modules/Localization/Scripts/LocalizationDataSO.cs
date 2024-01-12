using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Localization;
using UnityEngine.AddressableAssets;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu]
public class LocalizationDataSO : ScriptableObject
{
    private Dictionary<string, LocalizationTextPackage> dictionary;
    public List<LocalizationTextPackage> dataTextPackage;
    public FontData [] fontDatas;
    
    public TempFontData[] tempFontDatas;
    [System.Serializable]
    public struct FontData
    {
        public AssetReferenceT<TMP_FontAsset> fontRef; 
        public AssetReferenceT<Material>[] fontMaterialRefs;
        public string[] fontKeys;
    }
   
    [System.Serializable]
    public class TempFontData
    {
        public TMP_FontAsset font;
        public Dictionary<int,Material> fontMaterials;
        public TempFontData(TMP_FontAsset font)
        {
            this.font = font;
            fontMaterials = new Dictionary<int, Material>();
        }
    }
    public void ClearData()
    {
        GameUtility.GameUtility.Log("CLEAR DATA: " + name);
        tempFontDatas = null;
        tempFontDatas = new TempFontData[fontDatas.Length];
    }
    public LocalizationTextPackage FindByKey(string key)
    {
        for(int i = 0; i < dataTextPackage.Count; i++)
        {
            if (dataTextPackage[i].id.title.Equals(key))
            {
                return dataTextPackage[i];
            }
        }
        return null;
    }
    public LocalizationTextPackage Get(LocalizationTextID textID)
    {
        return Get(textID.title);
    }
    public LocalizationTextPackage Get(string key)
    {
        LocalizationTextPackage result = null;
        bool isExist = dictionary.TryGetValue(key,out result);
        return isExist?result:null;
    }
    public void SetUpDictionary()
    {
        dictionary = new Dictionary<string, LocalizationTextPackage>();
        for (int i = 0; i < dataTextPackage.Count; i++)
        {
            dictionary.Add(dataTextPackage[i].id.title, dataTextPackage[i]);
        }
    }
    public LocalizationDataSO Clone()
    {
//#if UNITY_EDITOR
//        return this;
//#endif
        LocalizationDataSO newData = CreateInstance<LocalizationDataSO>();
        newData.dataTextPackage = dataTextPackage;
        newData.name = name;
        newData.fontDatas = fontDatas;
        newData.tempFontDatas = new TempFontData[fontDatas.Length];
        //newData.list = list;
        return newData;
    }
    /*private void OnValidate()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        for (int i = 0; i < fontDatas.Length; i++)
        {
            fontDatas[i].fontKeys = new string[fontDatas[i].fontMaterialRefs.Length];
            for (int j = 0; j < fontDatas[i].fontKeys.Length; j++)
            {
                fontDatas[i].fontKeys[j] = fontDatas[i].fontMaterialRefs[j].editorAsset.name;
            }
        }
#endif
    }*/
}
