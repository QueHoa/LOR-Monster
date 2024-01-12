using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageStateHandler : UIHandler.StateHandler
{
    private Image image;
    public Sprite[] sprites;
    [SerializeField]
    private Color[] colors;

    [SerializeField]
    private bool withChildren=false;

    public StatusState currentState;
    public Image GetImage()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        return image;
    }
    public override void SetState(StatusState status)
    {
        currentState = status;
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        if (sprites.Length!=0)
            image.sprite = sprites[(int)status];
        if(colors.Length!=0)
            image.color = colors[(int)status];

        if (withChildren)
        {
            UIHandler.StateHandler[] stateHandler = GetComponentsInChildren<UIHandler.StateHandler>(false);
            foreach(UIHandler.StateHandler handler in stateHandler)
            {
                if (handler != this)
                {
                    handler.SetState(status);
                }
            }
        }
    }
    public override void SetColor(Color c)
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        image.color = c;
    }
    public override Color GetColor()
    {
        return image.color;
    }
    public void SetSprite(Sprite sprite)
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        image.sprite = sprite;
        image.SetNativeSize();
    }

}
