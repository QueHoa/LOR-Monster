using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;

namespace Settings
{
    public class UIToggle : MonoBehaviour
    {
        [Title("Components")]
        public Image icon, bg;
        public RectTransform switchRectTransform;
        public Sprite iconOn, iconOff, bgOn, bgOff;

        [Title("Animation")]
        public float targetAnchorPosX = 41f;
        public float duration = 0.25f;
        public Ease ease = Ease.OutQuart;

        private void Reset()
        {
            icon = transform.GetChild(0).GetComponent<Image>();
            switchRectTransform = icon.rectTransform;
        }

        [Button]
        public void TurnOn(bool immediate = false)
        {
            icon.DOKill();
            switchRectTransform.DOKill();

            if (immediate)
            {
                icon.sprite = iconOn;
                bg.sprite = bgOn;
                switchRectTransform.anchoredPosition = new Vector2(targetAnchorPosX, 0f);
            }
            else
            {
                float t = 0;
                switchRectTransform.DOAnchorPosX(targetAnchorPosX, duration).SetEase(ease).OnUpdate(() =>
                {
                    t += Time.deltaTime;
                    if(t >= 0.125f)
                    {
                        icon.sprite = iconOn;
                        bg.sprite = bgOn;
                    }
                });
            }
        }

        [Button]
        public void TurnOff(bool immediate = false)
        {
            icon.DOKill();
            switchRectTransform.DOKill();

            if (immediate)
            {
                icon.sprite = iconOff;
                bg.sprite = bgOff;
                switchRectTransform.anchoredPosition = new Vector2(-targetAnchorPosX, 0f);
            }
            else
            {
                float t = 0;
                switchRectTransform.DOAnchorPosX(-targetAnchorPosX, duration).SetEase(ease).OnUpdate(() =>
                {
                    t += Time.deltaTime;
                    if (t >= 0.125f)
                    {
                        icon.sprite = iconOff;
                        bg.sprite = bgOff;
                    }
                });
            }
        }
    }
}