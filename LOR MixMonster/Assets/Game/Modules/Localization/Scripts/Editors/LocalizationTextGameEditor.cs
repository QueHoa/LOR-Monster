using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Localization;

#if UNITY_EDITOR
[CustomEditor(typeof(LocalizationTextGame))]
public class LocalizationTextGameEditor : Editor
{

    LocalizationCollectionSO collection;
    //FontAssetDataSO fonts;
    GUIContent[] content;
    private void OnEnable()
    {
        last = -1;
        lastMaterial = -1;

        LocalizationTextGame main = ((LocalizationTextGame)target);
        EditorUtility.SetDirty(main);
    }
    int last = -1;
    int lastMaterial = -1;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LocalizationTextGame main = ((LocalizationTextGame)target);
        //collection = FindObjectOfType<SheetDataManager>().localizationData;
        //fonts = FindObjectOfType<SheetDataManager>().fontData;
        content = new GUIContent[collection.Get((LanguageKey)main.language).dataTextPackage.Count];
        int index = 0;
        foreach (LocalizationTextPackage package in collection.Get((LanguageKey)main.language).dataTextPackage)
        {
            content[index++] = new GUIContent(package.id.title);
        }

        if (GUILayout.Button(((LanguageKey)main.language).ToString()))
        {
            main.language++;
            if (main.language >= collection.collections.Count)
            {
                main.language = 0;
            }
        }

        main.selection = EditorGUILayout.Popup(main.selection, content);
        if (last != main.selection || lastMaterial!=main.selectedMaterialIndex)
        {
            LocalizationTextPackage localizationTextPackage = collection.Get((LanguageKey)main.language).FindByKey(main.textID.title);
            ((LocalizationTextGame)target).textID = localizationTextPackage.id;
            LocalizationDataSO.FontData fontData = collection.Get((LanguageKey)main.language).fontDatas[localizationTextPackage.fontId];
            UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<TMPro.TMP_FontAsset>(fontData.fontRef).Completed += fontOP =>
            {
                UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Material>(fontData.fontMaterialRefs[main.selectedMaterialIndex]).Completed += materialOP =>
                {
                    ((LocalizationTextUI)target).ChangeText(localizationTextPackage.text, fontOP.Result, materialOP.Result);

                };
            };
            last = main.selection;
        }
       
    }

}
#endif