using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class HolidayEventImageHandler : MonoBehaviour
{
    [SerializeField]
    private AssetReferenceT<Sprite> []mainSprites;
    private Image image;
    private SpriteRenderer sr;

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
        HolidayEvent.HolidayEventHandler.onEventChange -= OnEventChange;
        HolidayEvent.HolidayEventHandler.onEventChange += OnEventChange;
        OnEventChange(HolidayEvent.HolidayEventHandler.Holiday);

    }
    private void OnDisable()
    {
        HolidayEvent.HolidayEventHandler.onEventChange -= OnEventChange;
    }
    private void OnDestroy()
    {
        HolidayEvent.HolidayEventHandler.onEventChange -= OnEventChange;
    }

    void OnEventChange(HolidayEvent.ThemeType currentEvent)
    {
        if (!mainSprites[(int)currentEvent].RuntimeKeyIsValid())
        {
            if (image != null)
                image.enabled = false;
            if (sr != null)
                sr.enabled = false;
            return;
        }
        Addressables.LoadAssetAsync<Sprite>(mainSprites[(int)currentEvent]).Completed += op =>
        {
            if (image != null)
                image.sprite =op.Result;
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
}
