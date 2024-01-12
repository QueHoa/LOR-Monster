using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Localization;
using Sheet;

#if UNITY_EDITOR
[CustomEditor(typeof(LocalizationTextUI))]
public class LocalizationTextUIEditor : Editor
{

    LocalizationCollectionSO collection;
    GUIContent[] content;
    int lastMaterial = -1;

    private void OnEnable()
    {
        last = -1;
        lastMaterial = -1;

        LocalizationTextUI main = ((LocalizationTextUI)target);
        EditorUtility.SetDirty(main);
    }
    int last = -1;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LocalizationTextUI main = ((LocalizationTextUI)target);
        collection = FindObjectOfType<SheetDataManager>().localizationData;
        content = new GUIContent[collection.Get((LanguageKey)main.language).dataTextPackage.Count];
        int index = 0;

        foreach (LocalizationTextPackage title in collection.Get((LanguageKey)main.language).dataTextPackage)
        {
            content[index++] = new GUIContent(title.id.title);
        }

        if (GUILayout.Button(((LanguageKey)main.language).ToString()))
        {
            main.language++;
            main.language = main.language % collection.GetTotalLanguage();


        }

        main.selection =EditorGUILayout.Popup(main.selection, content);
        if (last != main.selection || lastMaterial != main.selectedMaterialIndex)
        {
            LocalizationTextPackage localizationTextPackage = collection.Get((LanguageKey)main.language).FindByKey(content[main.selection].text);
            ((LocalizationTextUI)target).textID = localizationTextPackage.id;
            if (!string.IsNullOrEmpty(localizationTextPackage.id.title))
            {
                LocalizationDataSO.FontData fontData = collection.Get((LanguageKey)main.language).fontDatas[localizationTextPackage.fontId];


                UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<TMPro.TMP_FontAsset>(fontData.fontRef).Completed += fontOP =>
                {
                    UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Material>(fontData.fontMaterialRefs[main.selectedMaterialIndex]).Completed += materialOP =>
                    {
                        ((LocalizationTextUI)target).ChangeText(localizationTextPackage.text, fontOP.Result, materialOP.Result);

                    };
                };
            }
            last = main.selection;
        }
       
    }

}
#endif