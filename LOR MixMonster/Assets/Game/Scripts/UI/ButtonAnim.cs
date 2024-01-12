using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnim : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float scale = 0.95f;
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(scale, 0.3f).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(1, 0.3f).SetUpdate(true);

    }

}
