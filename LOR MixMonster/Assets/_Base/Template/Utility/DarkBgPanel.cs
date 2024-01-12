using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class DarkBgPanel : MonoBehaviour
{
    [Title("Canvas")]
    [InlineButton(nameof(ShowHide))]
    public CanvasGroup canvasGroup;

    [Title("Background")]
    public Image darkBg;
    public float appearDuration = 0.4f;
    public float disappearDuration = 0.3f;

    [Title("Popup (must be named Popup)")]
    public Transform popupTransform;
    public CanvasGroup popupCanvas;
    public float startScale = 0.5f;

    public void ShowHide()
    {
        canvasGroup.alpha = (canvasGroup.alpha == 0) ? 1 : 0;
    }

    protected virtual void OnValidate()
    {
        if (darkBg == null) darkBg = GetComponent<Image>();
        popupTransform = transform.Find("Popup");
        popupCanvas = popupTransform.GetComponent<CanvasGroup>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        GetComponent<CanvasGroup>().alpha = 1;

        darkBg.DOKill();
        darkBg.color = new Color(1, 1, 1, 0);

        popupCanvas.DOKill();
        popupCanvas.alpha = 0;
        popupCanvas.interactable = false;

        popupTransform.DOKill();
        popupTransform.localScale = Vector3.one * startScale;
    }

    public virtual void Appear()
    {
        // SoundManager.PlaySound(SoundName.POPUP_POPUP);

        gameObject.SetActive(true);

        darkBg.DOKill();
        darkBg.DOFade(1f, appearDuration).SetEase(Ease.OutCubic);

        popupTransform.DOKill();
        popupTransform.DOScale(1f, appearDuration).SetEase(Ease.OutBack);

        popupCanvas.DOKill();
        popupCanvas.DOFade(1f, appearDuration).SetEase(Ease.OutCubic)
            .OnComplete(() => popupCanvas.interactable = true);
    }

    public virtual void Disappear()
    {
        // SoundManager.PlaySound(SoundName.CLOSE_POPUP);

        darkBg.DOKill();
        darkBg.DOFade(0f, disappearDuration).SetEase(Ease.OutCubic).OnComplete(Hide);

        popupCanvas.DOKill();
        popupCanvas.interactable = false;
        popupCanvas.DOFade(0f, disappearDuration).SetEase(Ease.OutCubic);

        popupTransform.DOKill();
        popupTransform.DOScale(startScale, disappearDuration).SetEase(Ease.InBack);
    }
}