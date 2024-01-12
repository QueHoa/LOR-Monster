using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class NativeAdBanner : MonoBehaviour
{
    [SerializeField]
    private GameObject mainObj;
    [SerializeField]
    private Image adIcon,adChoiceIcon,ratingImage,bigAdBanner;
    [SerializeField]
    private Text adTitleText,priceText,storeText,bodyText,advertiserText,installText;
    private List<Sprite> adBannerSprites=new List<Sprite>();
    CancellationTokenSource cancellationToken;
    private void Start()
    {
    }
    private void OnEnable()
    {
        if (mainObj == null)
        {
            mainObj = gameObject;
        }

        if (!DataManagement.DataManager.Instance.userData.IsAd || Game.Controller.Instance.gameConfig.skipAd) mainObj.SetActive(false);
        cancellationToken = new CancellationTokenSource();
        LoadAd();
        AD.Controller.Instance.onNativeAdRefresh += OnRefresh;

    }
    void OnRefresh()
    {
        LoadAd();
    }
    int index = 0;
    void ChangeImage()
    {
        bigAdBanner.sprite = adBannerSprites[index%adBannerSprites.Count];
        index++;
        Invoke(nameof(ChangeImage), 4);
    }
    private void OnDisable()
    {
        CancelInvoke();
        if (cancellationToken != null)
        {
            cancellationToken.Cancel();
        }
        AD.Controller.Instance.onNativeAdRefresh -= OnRefresh;

    }
    private void OnDestroy()
    {
        if (cancellationToken != null)
        {
            cancellationToken.Cancel();
            cancellationToken.Dispose();
        }
        AD.Controller.Instance.onNativeAdRefresh -= OnRefresh;
    }
    GoogleMobileAds.Api.NativeAd nativeAd;
    async UniTaskVoid LoadAd()
    {
        if (!AD.Controller.Instance.isAd)
        {
            gameObject.SetActive(false);
            return;
        }
        float timeOut = 10;
        float time = Time.time;
#if UNITY_EDITOR
        await UniTask.Delay(500, ignoreTimeScale: true, cancellationToken: cancellationToken.Token);
        return;
#endif
        await UniTask.WaitUntil(() => AD.Controller.Instance.IsNativeAdAvailable() ||Time.time-time>timeOut,cancellationToken:cancellationToken.Token);

        if (nativeAd != null)
        {
            nativeAd.Destroy();
        }
        nativeAd = (GoogleMobileAds.Api.NativeAd)AD.Controller.Instance.GetNativeAd();
        AD.Controller.Instance.LoadNativeAd();

        if (nativeAd != null)
        {
            Texture2D texture2D = null;
            try
            {
                if (bigAdBanner != null)
                {
                    List<Texture2D> texture2Ds = nativeAd.GetImageTextures();
                    foreach (Texture2D texture in texture2Ds)
                    {
                        adBannerSprites.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero));
                    }
                    index = 0;
                    ChangeImage();

                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("1 " + e);
            }
            try
            {
                texture2D = nativeAd.GetIconTexture();
                adIcon.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);

            }
            catch (System.Exception e)
            {
                Debug.LogError("1 "+e);
            }

            try
            {
                texture2D = nativeAd.GetAdChoicesLogoTexture();
                adChoiceIcon.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            }
            catch (System.Exception e)
            {
                Debug.LogError("2 " + e);
            }
            try
            {
                adTitleText.text = nativeAd.GetHeadlineText();
            }
            catch (System.Exception e)
            {
                Debug.LogError("3 " + e);
            }
          
            try
            {
                bodyText.text = nativeAd.GetBodyText();
            }
            catch (System.Exception e)
            {
                Debug.LogError("6 " + e);
            }
         
            try
            {
                ratingImage.fillAmount = (float)(nativeAd.GetStarRating() / 5f);
            }
            catch (System.Exception e)
            {
                Debug.LogError("8 " + e);
            }
            try
            {
                installText.text = nativeAd.GetCallToActionText() ;
            }
            catch (System.Exception e)
            {
                Debug.LogError("9 " + e);
            }

            try
            {
                nativeAd.RegisterCallToActionGameObject(gameObject);

            }
            catch (System.Exception e)
            {
                Debug.LogError("10 " + e);
            }
            await UniTask.Delay(500, ignoreTimeScale: true, cancellationToken: cancellationToken.Token);
        }
        else
        {
            mainObj.SetActive(false);
        }

    }

  
}
