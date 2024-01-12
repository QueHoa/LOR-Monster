namespace Localization
{
    [System.Serializable]
    public class LocalizationTextPackage
    {
        public LocalizationTextID id;
        public string text;
        public int fontId;

        public LocalizationTextPackage()
        {
        }
        public string GetText()
        {
            return text;
        }
    }

}