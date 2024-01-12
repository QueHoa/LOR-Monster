using Sheet;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Localization
{
    public class LocalizationHandler
    {
        public static LanguageKey LANGUAGE = LanguageKey.English;

        public delegate void OnLanguageChanged(LanguageKey language);
        public static OnLanguageChanged onLanguageChanged;
        public static bool isInit = false;
        private static LocalizationDataSO currentData;
        public static void Init()
        {
            if (isInit) return;
            onLanguageChanged = null;
            //if (!PlayerPrefs.HasKey("Lang"))
            //{
            //    PlayerPrefs.SetInt("Lang", 1);
            //    switch (Application.systemLanguage)
            //    {
            //        case SystemLanguage.Vietnamese:
            //            PlayerPrefs.SetInt("Language", (int)LanguageKey.Vietnamese);
            //            break;
            //        case SystemLanguage.Japanese:
            //            PlayerPrefs.SetInt("Language", (int)LanguageKey.Japanese);
            //            break;
            //        case SystemLanguage.Korean:
            //            PlayerPrefs.SetInt("Language", (int)LanguageKey.Korean);
            //            break;
            //        case SystemLanguage.ChineseTraditional:
            //            PlayerPrefs.SetInt("Language", (int)LanguageKey.ChineseTraditional);
            //            break;
            //        case SystemLanguage.Indonesian:
            //            PlayerPrefs.SetInt("Language", (int)LanguageKey.Indonesian);
            //            break;
            //        case SystemLanguage.Russian:
            //            PlayerPrefs.SetInt("Language", (int)LanguageKey.Russian);
            //            break;
            //        case SystemLanguage.Ukrainian:
            //            PlayerPrefs.SetInt("Language", (int)LanguageKey.Russian);
            //            break;
            //        case SystemLanguage.Spanish:
            //            PlayerPrefs.SetInt("Language", (int)LanguageKey.Spanish);
            //            break;
            //        case SystemLanguage.Thai:
            //            PlayerPrefs.SetInt("Language", (int)LanguageKey.Thai);
            //            break;
            //        case SystemLanguage.Portuguese:
            //            PlayerPrefs.SetInt("Language", (int)LanguageKey.Brazil);
            //            break;
            //        default:
            //            PlayerPrefs.SetInt("Language", (int)LanguageKey.English);
            //            break;
            //    }
            //}
            LANGUAGE = (LanguageKey)PlayerPrefs.GetInt("Language", 0);
            ChangeLanguage(LANGUAGE);
        }
        public static void ChangeLanguage(LanguageKey language)
        {
            if (currentData != null)
            {
                currentData.ClearData();
            }
            LANGUAGE = language;
            currentData = SheetDataManager.Instance.localizationData.Get(language);
            PlayerPrefs.SetInt("Language", (int)language);
            onLanguageChanged?.Invoke(language);
        }
        
        public static string GetText(string key, TMPro.TextMeshProUGUI text, string defaultText = "")
        {
            if (SheetDataManager.Instance.gameData == null)
            {
                return defaultText;
            }
            LocalizationDataSO data = SheetDataManager.Instance.localizationData.Get(LANGUAGE);

            LocalizationTextPackage localizationTextPackage = data.Get(key);
            if (localizationTextPackage == null)
            {
                return defaultText;
            }
            else
            {
                try
                {
                    if (text != null)
                    {

                        LocalizationDataSO.FontData fontData = SheetDataManager.Instance.localizationData.collections[(int)LANGUAGE].fontDatas[localizationTextPackage.fontId];
                        Addressables.LoadAssetAsync<TMPro.TMP_FontAsset>(fontData.fontRef).Completed+=op=> 
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

                            Addressables.LoadAssetAsync<Material>(fontData.fontMaterialRefs[materialId]).Completed += op =>
                            {
                                text.fontSharedMaterial =op.Result;
                                Addressables.Release(op);
                            };

                            text.font = op.Result;

                            Addressables.Release(op);
                        };

                        LocalizationDataSO.FontData defaultFontData = SheetDataManager.Instance.localizationData.collections[(int)LanguageKey.English].fontDatas[localizationTextPackage.fontId];
                    }
                }
                catch (System.Exception e)
                {
                    GameUtility.GameUtility.LogError("ERROR LOCALIZE: " + key);
                }

                return localizationTextPackage.text;
            }
        }
    }
}