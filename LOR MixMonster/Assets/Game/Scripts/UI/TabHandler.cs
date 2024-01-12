using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabHandler : MonoBehaviour
{
    public TabButton[] tabButtons;
    public void SetTab(int index)
    {
        if (tabButtons==null)
        {
            tabButtons = GetComponentsInChildren<TabButton>();
        }
        for(int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].SetUp(i == index,i<index);
        }
    }
}
