using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabButton : MonoBehaviour
{
    [SerializeField]
    private GameObject finishObj;
    public void SetUp(bool isCurrent,bool isFinished)
    {
        GetComponent<UIHandler.StateHandler>().SetState(isCurrent ? UIHandler.StateHandler.StatusState.Current : (isFinished ? UIHandler.StateHandler.StatusState.Unlock : UIHandler.StateHandler.StatusState.Lock));
        finishObj.SetActive(isFinished);
    }
}
