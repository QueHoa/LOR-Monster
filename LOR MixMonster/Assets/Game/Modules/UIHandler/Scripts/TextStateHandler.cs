using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextStateHandler : UIHandler.StateHandler
{
    private TMPro.TextMeshProUGUI text;
    [SerializeField]
    private Color lockColor, unlockColor, currentColor;

    
    public override void SetState(StatusState status)
    {
        if (text == null)
        {
            text = GetComponent<TMPro.TextMeshProUGUI>();
        }
        text.color = status == StatusState.Current ? currentColor : (status == StatusState.Lock ? lockColor : unlockColor);
    }
    public override void SetColor(Color c)
    {
        if (text == null)
        {
            text = GetComponent<TMPro.TextMeshProUGUI>();
        }
        text.color = c;
    }
    public override Color GetColor()
    {
        return text.color;
    }
    public void SetText(string value)
    {
        if (text == null)
        {
            text = GetComponent<TMPro.TextMeshProUGUI>();
        }
        text.text = value;
    }
}
