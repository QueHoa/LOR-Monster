using GameUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardBarMarker : GameUtility.Pooling.PoolComponent
{
    [SerializeField]
    private TMPro.TextMeshProUGUI viewText;
    [SerializeField]
    private GameObject rewardObj;
    [SerializeField]
    private ParticleSystem unlockPS;
    RectTransform _transform;
    public void SetUp(Vector2 pos,int view)
    {
        if (_transform == null)
        {
            _transform = GetComponent<RectTransform>();
        }
        rewardObj.SetActive(DataManagement.DataManager.Instance.userData.BestView < view);
        GetComponent<UIHandler.StateHandler>().SetState(UIHandler.StateHandler.StatusState.Lock);
        viewText.text = GameUtility.GameUtility.ShortenNumber(view);
        _transform.anchoredPosition = pos;
        gameObject.SetActive(true);

    }
    public void Finish()
    {
        GetComponent<UIHandler.StateHandler>().SetState(UIHandler.StateHandler.StatusState.Unlock);
            unlockPS.Play();
        _transform.Shake(0.15f, 1, 2f);
    }
}
