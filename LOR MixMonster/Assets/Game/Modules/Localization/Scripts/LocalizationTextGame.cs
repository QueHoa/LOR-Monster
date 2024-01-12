using Localization;
using Sheet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LocalizationTextGame : UnityEngine.MonoBehaviour
{
    public Localization.LocalizationTextID textID;
    TMPro.TextMeshPro text;
    public int selection, language,selectedMaterialIndex;

    private void Start()
    {

    }
    public void ChangeText(string newText, TMPro.TMP_FontAsset newFont,Material material)
    {
        TMPro.TextMeshPro text = GetComponent<TMPro.TextMeshPro>();
        text.text = (newText);
        text.font = newFont;
        text.material = material;
        text.UpdateFontAsset();
        text.ForceMeshUpdate(true);

    }
    void OnLanguageChanged(LanguageKey language)
    {
        LocalizationDataSO dictionary = SheetDataManager.Instance.localizationData.Get(language);
        LocalizationTextPackage localizationTextPackage = dictionary.Get(textID.title);
        LocalizationDataSO.FontData fontData = dictionary.fontDatas[localizationTextPackage.fontId];

        Addressables.LoadAssetAsync<TMPro.TMP_FontAsset>(fontData.fontRef).Completed += fontOP =>
        {
            Addressables.LoadAssetAsync<Material>(fontData.fontMaterialRefs[selectedMaterialIndex]).Completed += materialOP =>
            {
                ChangeText(localizationTextPackage.GetText(), fontOP.Result, materialOP.Result);
                Addressables.Release(fontOP);
                Addressables.Release(materialOP);
            };
        };
    }
    private void OnEnable()
    {
        if (text == null)
        {
            text = GetComponent<TMPro.TextMeshPro>();
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
}
