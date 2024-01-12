using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class HolidayEventImageMultiSpriteHandler : MonoBehaviour
{
    [System.Serializable]
    public struct TexturePack
    {
        public string packId;
        public AssetReferenceT<Sprite>[] mainSprites;
    }

    public string pack = string.Empty;
    private Image image;
    private SpriteRenderer sr;
    public TexturePack[] texturePacks;
    [SerializeField]
    private bool useNativeSize = true;
    private void OnEnable()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
            if (image != null)
            {
                //mainSprites[0] = image.sprite;
            }
        }
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                //mainSprites[0] = sr.sprite;
                useNativeSize = false;
            }
        }
        //HolidayEvent.HolidayEventHandler.onEventChange -= OnEventChange;
        //HolidayEvent.HolidayEventHandler.onEventChange += OnEventChange;
        //OnEventChange(HolidayEvent.HolidayEventHandler.Holiday);

    }
    private void OnDisable()
    {
        HolidayEvent.HolidayEventHandler.onEventChange -= OnEventChange;
    }
    private void OnDestroy()
    {
        HolidayEvent.HolidayEventHandler.onEventChange -= OnEventChange;
    }
    public void Refresh()
    {
        OnEventChange(HolidayEvent.HolidayEventHandler.Holiday);
    }
    void OnEventChange(HolidayEvent.ThemeType currentEvent)
    {
        if (string.IsNullOrEmpty(pack)) return;
        TexturePack selectedPack = GetPack();
        if (!selectedPack.mainSprites[(int)currentEvent].RuntimeKeyIsValid())
        {
            if (image != null)
                image.enabled = false;
            if (sr != null)
                sr.enabled = false;
            return;
        }
        
        Addressables.LoadAssetAsync<Sprite>(selectedPack.mainSprites[(int)currentEvent]).Completed += op =>
        {
            if (image != null)
                image.sprite = op.Result;
            if (sr != null)
                sr.sprite = op.Result;
            if (useNativeSize)
            {
                if (image != null)
                    image.SetNativeSize();
            }
            Addressables.Release(op);
        };

    }

    TexturePack GetPack()
    {
        foreach(TexturePack pack in texturePacks)
        {
            if (pack.packId.Equals(this.pack))
            {
                return pack;
            }
        }
        return texturePacks[0];
    }
}