using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ItemSelectButton : MonoBehaviour
{
    [SerializeField]
    private GameObject adIcon, viewBonusIcon;
    [SerializeField]
    private Image itemIconImg, iconTry;
    [SerializeField]
    private Button btnBuy;
    [SerializeField]
    private TextMeshProUGUI priceGoldText, goldText;
    [SerializeField]
    private Animator animTry;
    public ItemData.Item item;
    [SerializeField]
    private AudioClip selectSFX;
    Vector2 defaultPos;
    MakeOverPanelAbstract makeOverPanel;
    public int index;
    int priceGold;

    bool isProcessing = false;
    private void OnEnable()
    {
        defaultPos = GetComponent<RectTransform>().anchoredPosition;
    }
    public async UniTask SetUp(ItemData.Item item,MakeOverPanelAbstract makeOverPanel)
    {

        isProcessing = false;
        this.makeOverPanel = makeOverPanel;
        this.item = item;
        priceGold = item.cost;
        /*item.GetIcon(async sprite =>
        {
            itemIconImg.sprite = sprite;
        });*/
        itemIconImg.sprite = await Addressables.LoadAssetAsync<Sprite>(item.icon);

        adIcon.SetActive(item.unlockType ==ItemData.UnlockType.Ad);
        viewBonusIcon.SetActive(item.unlockType == ItemData.UnlockType.Ad || !string.IsNullOrEmpty(item.bundleId));
        await Show();
    }
    [SerializeField]
    private AnimationCurve showCurve,scaleCurve, highLightMoveCurve;

    async UniTask Show()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        GetComponent<PanelFadeAnimation>().Show();
        Vector3 startPos = defaultPos;
        startPos.y -= 100;
        startPos.x = rectTransform.anchoredPosition.x;
        rectTransform.anchoredPosition = startPos;

        float pos = startPos.y ;
        float t = 0;
        do
        {
            startPos.y = pos + 100 * showCurve.Evaluate(t);
            rectTransform.anchoredPosition = startPos;

            t += Time.fixedDeltaTime;
            t = t < 1 ? t : 1;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        } while (t < 1);
        rectTransform.anchoredPosition = defaultPos;
    }

    public async UniTask HighLight()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 startPos = defaultPos;
        Vector3 scale = Vector3.one;
        float pos = rectTransform.anchoredPosition.y;
        float t = 0;
        do
        {
            startPos.y = pos + 100 * highLightMoveCurve.Evaluate(t);
            rectTransform.anchoredPosition = startPos;

            scale.x = scale.y = 1 + scaleCurve.Evaluate(t);
            rectTransform.localScale = scale;

            t += Time.fixedDeltaTime;
            t = t < 1 ? t : 1;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        } while (t < 1);
        rectTransform.anchoredPosition = defaultPos;

    }

    public void Hide()
    {
        GetComponent<PanelFadeAnimation>().Close(() => gameObject.SetActive(false));
    }
    public void OnSelect()
    {
        if (isProcessing || makeOverPanel.isProcessing) return;
        isProcessing = true;
        if (item.unlockType == ItemData.UnlockType.None)
        {
            Sound.Controller.Instance.PlayOneShot(selectSFX);
            makeOverPanel.OnSelect(this);
        }
        else
        {
            priceGoldText.text = priceGold.ToString();

            if (DataManagement.DataManager.Instance.userData.YourGold >= priceGold)
            {
                btnBuy.interactable = true;
            }
            else
            {
                btnBuy.interactable = false;
            }

            iconTry.sprite = itemIconImg.sprite;
            animTry.gameObject.SetActive(true);
            isProcessing = false;
        }
    }
    public void SelectAds()
    {
        if (isProcessing || makeOverPanel.isProcessing) return;
        isProcessing = true;
        AD.Controller.Instance.ShowRewardedAd("AdOption", res =>
        {
            if (res)
            {
                Sound.Controller.Instance.PlayOneShot(selectSFX);
                makeOverPanel.OnSelect(this);
                animTry.SetTrigger("close");
            }
            else
            {
                isProcessing = false;
            }
        });
    } 
    public void Buy()
    {
        if (isProcessing || makeOverPanel.isProcessing) return;
        isProcessing = true;
        /*if (item.category == ItemData.Category.Body)
        {
            makeOverPanel.isProcessing = true;
        }*/
        Sound.Controller.Instance.PlayOneShot(selectSFX);
        makeOverPanel.OnSelect(this);
        int changeGold = DataManagement.DataManager.Instance.userData.YourGold - priceGold;
        goldText.transform.DOScale(Vector3.one * 1.3f, 0.3f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
        DOTween.To(() => DataManagement.DataManager.Instance.userData.YourGold, x => DataManagement.DataManager.Instance.userData.YourGold = x, changeGold, 1f).SetEase(Ease.OutQuad).OnUpdate(() =>
        {
            goldText.text = DataManagement.DataManager.Instance.userData.YourGold.ToString();
        }).OnComplete(() =>
        {
            DataManagement.DataManager.Instance.userData.YourGold = DataManagement.DataManager.Instance.userData.YourGold;
            DataManagement.DataManager.Instance.Save();
        });
        animTry.SetTrigger("close");
        isProcessing = false;
    }
}
