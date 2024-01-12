using Localization;
using Sheet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LocalizationTextUI : UnityEngine.MonoBehaviour
{
    public Localization.LocalizationTextID textID;
    TMPro.TextMeshProUGUI text;
    public int selection,language,selectedMaterialIndex;

    private void Start()
    {
        
    }
    public void ChangeText(string newText,TMPro.TMP_FontAsset newFont,Material material)
    {
        TMPro.TextMeshProUGUI text = GetComponent<TMPro.TextMeshProUGUI>();
        text.text=(newText);
        text.font = newFont;
        text.fontSharedMaterial = material;
        text.UpdateFontAsset();
        text.ForceMeshUpdate(true);

    }
    void OnLanguageChanged(LanguageKey language)
    {
        try
        {
            LocalizationDataSO dictionary = SheetDataManager.Instance.localizationData.Get(language);
            LocalizationTextPackage localizationTextPackage = dictionary.Get(textID.title);

            LocalizationDataSO.FontData fontData = dictionary.fontDatas[localizationTextPackage.fontId];

            if (dictionary.tempFontDatas[localizationTextPackage.fontId] == null)
            {
                Addressables.LoadAssetAsync<TMPro.TMP_FontAsset>(fontData.fontRef).Completed += fontOP =>
                {
                    Addressables.LoadAssetsAsync<Material>(fontData.fontMaterialRefs, null,Addressables.MergeMode.Union).Completed += materialOP =>
                     {
                         //prepare temp material
                         dictionary.tempFontDatas[localizationTextPackage.fontId] = new LocalizationDataSO.TempFontData(fontOP.Result);
                         for (int i = 0; i < materialOP.Result.Count; i++)
                         {
                             dictionary.tempFontDatas[localizationTextPackage.fontId].fontMaterials.Add(i, materialOP.Result[i]);
                         }
                         //
                         LocalizationDataSO.FontData defaultFontData = SheetDataManager.Instance.localizationData.collections[(int)LanguageKey.English].fontDatas[localizationTextPackage.fontId];
                         int materialId = -1;
                         for (int i = 0; i < defaultFontData.fontMaterialRefs.Length; i++)
                         {
                           
                             if (text.fontSharedMaterial.ToString().Contains(defaultFontData.fontKeys[i]))
                             {
                                 materialId = i;
                                 break;
                             }
                         }
                         if (materialId == -1)
                         {
                             for (int i = 0; i < fontData.fontMaterialRefs.Length; i++)
                             {
                                 //GameUtility.GameUtility.Log($"check 2 : {text.text} {text.fontSharedMaterial.ToString()} vs {fontData.fontKeys[i]}");
                                 if (text.fontSharedMaterial.ToString().Contains(fontData.fontKeys[i]))
                                 {
                                     materialId = i;
                                     break;
                                 }
                             }
                         }
                         //

                         ChangeText(localizationTextPackage.GetText(), fontOP.Result, materialOP.Result[materialId]);
                         Addressables.Release(fontOP);
                         Addressables.Release(materialOP);
                     };
                };
            }
            else
            {
                LocalizationDataSO.FontData defaultFontData = SheetDataManager.Instance.localizationData.collections[(int)LanguageKey.English].fontDatas[localizationTextPackage.fontId];
                int materialId = -1;
                for (int i = 0; i < defaultFontData.fontMaterialRefs.Length; i++)
                {
                    //GameUtility.GameUtility.Log($"check 1: {text.text} {text.fontSharedMaterial.ToString()} vs {defaultFontData.fontKeys[i]} ");
                    if (text.fontSharedMaterial.ToString().Contains(defaultFontData.fontKeys[i]))
                    {
                        materialId = i;
                        break;
                    }
                }
                if (materialId == -1)
                {
                    for (int i = 0; i < fontData.fontMaterialRefs.Length; i++)
                    {
                        //GameUtility.GameUtility.Log($"check 2 : {text.text} {text.fontSharedMaterial.ToString()} vs {fontData.fontKeys[i]}");
                        if (text.fontSharedMaterial.ToString().Contains(fontData.fontKeys[i]))
                        {
                            materialId = i;
                            break;
                        }
                    }
                }
           
                ChangeText(localizationTextPackage.GetText(), dictionary.tempFontDatas[localizationTextPackage.fontId].font, dictionary.tempFontDatas[localizationTextPackage.fontId].fontMaterials[materialId]);
            }

        }
        catch (System.Exception e)
        {
            GameUtility.GameUtility.LogError(gameObject.name+" "+textID.title+"\n"+e);
        }
    }
    private void OnEnable()
    {
        if (text == null)
        {
            text= GetComponent<TMPro.TextMeshProUGUI>();
        }
        LocalizationHandler.onLanguageChanged -= OnLanguageChanged;
        LocalizationHandler.onLanguageChanged += OnLanguageChanged;
        OnLanguageChanged(LocalizationHandler.LANGUAGE);
    }
    private void OnDisable()
    {
        LocalizationHandler.onLanguageChanged -= OnLanguageChanged;
    }
    private void OnDestroy()
    {
        LocalizationHandler.onLanguageChanged -= OnLanguageChanged;
    }
    private void OnValidate()
    {
#if UNITY_EDITOR
        if(SheetDataManager.Instance!=null && !Application.isPlaying)
            OnLanguageChanged(LanguageKey.English);
#endif
    }
}
