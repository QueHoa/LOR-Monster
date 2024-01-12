using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePanel : UI.Panel
{
    public TMPro.TextMeshProUGUI messageText;
    public override void PostInit()
    {
    }
    public void SetUp(string message)
    {
        messageText.text = message;
        Show();
    }
    System.Action onClose;
    public void SetUp(string message, System.Action onClose)
    {
        this.onClose = onClose;
        messageText.text = message;
        Show();
    }
    public override void Deactive()
    {
        base.Deactive();
        onClose?.Invoke();
    }
}