using System.Collections.Generic;
using UnityEngine;
using Localization;
using Cysharp.Threading.Tasks;
using Sheet;

[CreateAssetMenu]
public class LocalizationCollectionSO : ScriptableObject
{
    public List<LocalizationDataSO> collections = new List<LocalizationDataSO>();

    public int GetTotalLanguage()
    {
        return collections.Count;
    }
    public LocalizationDataSO Get(LanguageKey language)
    {
        return collections[(int)language];
    }

    public LocalizationCollectionSO Clone()
    {
        LocalizationCollectionSO newData = CreateInstance<LocalizationCollectionSO>();
        newData.collections = new List<LocalizationDataSO>();
        for(int i = 0; i < collections.Count; i++)
        {
            newData.collections.Add(collections[i].Clone());
        }

        return newData;
    }

 
    public void ApplyData(List<RowData> rowDatas)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        
        int row = 1;
        for (int i = 0; i < collections.Count; i++)
        {
            collections[i].dataTextPackage = new List<LocalizationTextPackage>();
        }
        int total = rowDatas[0].list.Count - 3;
        while (row < rowDatas.Count)
        {
            string title = rowDatas[row].list[0];
            int fontId= int.Parse(rowDatas[row].list[1]);
            int id = row - 1;

            Localization.LocalizationTextID textId = new LocalizationTextID(title);
            
            int language = 0;
            while (language < total)
            {
                Localization.LocalizationTextPackage textPackage = new LocalizationTextPackage();
                textPackage.id = textId;
                textPackage.fontId = fontId;
                textPackage.text =rowDatas[row].list[language+3];

                collections[language].dataTextPackage.Add(textPackage);
                //GameUtility.GameUtility.Log($"ADD {language} : {textId.title}-{textPackage.fontId}-{textPackage.id}-{textPackage.text}");

                //if (!collections[language].dictionary.ContainsKey(textId.title))
                //{
                //    collections[language].dictionary.Add(textId.title, textPackage);
                //    //collections[language].list.Add(textPackage);
                //    GameUtility.GameUtility.Log($"ADD {language} : {textId.title}-{textPackage.fontId}-{textPackage.id}-{textPackage.text}");
                //}
                //else
                //{
                //    GameUtility.GameUtility.Log("CONTAIN ERROR:" +textId.title + " " + textPackage.text);

                //}
                language++;
            }
            row++;
        }
        for (int i = 0; i < collections.Count; i++)
        {
            collections[i].SetUpDictionary();
        }
    }

}
