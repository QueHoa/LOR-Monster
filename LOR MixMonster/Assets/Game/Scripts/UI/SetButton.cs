using DG.Tweening;
using Spine.Unity.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetButton : MonoBehaviour
{
    public Image bg, icon;
    public Sprite bgOn, bgOff, iconOn, iconOff;
    public Transform onPos, offPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Move(bool mode)
    {
        StartCoroutine(ChangeImage(mode));
    }
    IEnumerator ChangeImage(bool mode)
    {
        if (mode)
        {
            icon.transform.DOMove(offPos.position, 0.2f).SetEase(Ease.OutSine);
            yield return new WaitForSeconds(0.1f);
            icon.sprite = iconOff;
            bg.sprite = bgOff;
        }
        else
        {
            icon.transform.DOMove(onPos.position, 0.2f).SetEase(Ease.OutSine);
            yield return new WaitForSeconds(0.1f);
            icon.sprite = iconOn;
            bg.sprite = bgOn;
        }
    }
}
