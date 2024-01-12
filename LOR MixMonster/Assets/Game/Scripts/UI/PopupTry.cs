using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTry : MonoBehaviour
{

    private Animator anim;
    void OnEnable()
    {
        anim = GetComponent<Animator>();
    }
    public void Close()
    {
        anim.SetTrigger("close");
    }
    public void Off()
    {
        gameObject.SetActive(false);
    }
}
