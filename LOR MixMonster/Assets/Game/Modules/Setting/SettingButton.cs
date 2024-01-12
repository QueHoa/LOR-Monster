using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingButton : MonoBehaviour
{
    [SerializeField]
    private ImageStateHandler[] icons;

    [SerializeField]
    private RectTransform switchTransform;
    bool isTriggered = false;
    System.Action<bool> onTriggered;
    public void SetUp(System.Action<bool> onTriggered,bool isTriggered)
    {
        this.onTriggered = onTriggered;
        this.isTriggered = isTriggered;

        UpdateView();
    }
    void UpdateView()
    {
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].SetState(isTriggered ? UIHandler.StateHandler.StatusState.Unlock : UIHandler.StateHandler.StatusState.Lock);
        }
        switchTransform.anchoredPosition = new Vector2(isTriggered ? 50 : -50, -60);
    }
    public void Trigger()
    {
        isTriggered = !isTriggered;

        UpdateView();
        onTriggered?.Invoke(isTriggered);

    }
}
