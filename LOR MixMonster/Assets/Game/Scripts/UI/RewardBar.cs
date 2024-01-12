using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class RewardBar : MonoBehaviour
{

    GameUtility.Pooling.PoolHandler pool;
    [SerializeField]
    private Image barImg;
    CancellationTokenSource cancellation;
    [SerializeField]
    private RectTransform tipEffectTransform;
    [SerializeField]
    private AudioClip reachMarkerSFX,beatBestSFX,finishSFX;
    [SerializeField]
    private ParticleSystem[] effects;
    List<RewardBarMarker> markers = new List<RewardBarMarker>();
    [SerializeField]
    private BestViewMarker bestViewMarker;
    Vector2 size;
    int index = 0;
    private void OnEnable()
    {
        pool = GetComponentInChildren<GameUtility.Pooling.PoolHandler>();
        cancellation = new CancellationTokenSource();
        size = barImg.rectTransform.rect.size;
        int max = Sheet.SheetDataManager.Instance.gameData.rewardBarConfig.views[Sheet.SheetDataManager.Instance.gameData.rewardBarConfig.views.Length - 1];
        foreach (int view in Sheet.SheetDataManager.Instance.gameData.rewardBarConfig.views)
        {
            var marker = pool.Get().GetComponent<RewardBarMarker>();

            Vector2 pos = new Vector2(0,-size.y/2f+size.y*view/max);
         

            marker.SetUp(pos,view);

            markers.Add(marker);
        }
        if (DataManagement.DataManager.Instance.userData.BestView != 0)
        {
            bestViewMarker.SetUp(new Vector2(0, -size.y / 2f + size.y * Mathf.Clamp01(DataManagement.DataManager.Instance.userData.BestView*1f / max)), DataManagement.DataManager.Instance.userData.BestView);
        }
        else
        {
            bestViewMarker.gameObject.SetActive(false);
        }
        effects[0].Play();
    }
    private void OnDisable()
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
        }
    }
    private void OnDestroy()
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
        }
    }
    bool isNewBest = false;
    public void UpdateBar(int view)
    {
        int max = Sheet.SheetDataManager.Instance.gameData.rewardBarConfig.views[Sheet.SheetDataManager.Instance.gameData.rewardBarConfig.views.Length - 1];

        float current = Mathf.Clamp01(view * 1f / max);
        Vector2 tipPoint = Vector2.zero;
        tipPoint.y = -size.y / 2f + size.y * current;
        barImg.fillAmount = current;
        tipEffectTransform.anchoredPosition = tipPoint;

        if (index< Sheet.SheetDataManager.Instance.gameData.rewardBarConfig.views.Length && view >= Sheet.SheetDataManager.Instance.gameData.rewardBarConfig.views[index])
        {
            markers[index].Finish();
            index++;
            if(index== Sheet.SheetDataManager.Instance.gameData.rewardBarConfig.views.Length - 1)
            {
                Sound.Controller.Instance.PlayOneShot(finishSFX);
            }
            else
            {
                Sound.Controller.Instance.PlayOneShot(reachMarkerSFX);
            }
        }
        if (!isNewBest && view >= DataManagement.DataManager.Instance.userData.BestView)
        {
            bestViewMarker.Finish();
            effects[0].Stop();
            effects[1].Play();
            isNewBest = true;
        }
        else if(view >= DataManagement.DataManager.Instance.userData.BestView)
        {
            bestViewMarker.SetUp(tipPoint, view);
            bestViewMarker.Shake();
        }
    }
}
