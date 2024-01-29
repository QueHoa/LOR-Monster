using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTry : MonoBehaviour
{
    public MakeOverPanelAbstract main;
    private Animator anim;
    void OnEnable()
    {
        anim = GetComponent<Animator>();
    }
    public void Close()
    {
        main.isProcessing = false;
        anim.SetTrigger("close");
    }
    public void Off()
    {
        gameObject.SetActive(false);
    }
}
