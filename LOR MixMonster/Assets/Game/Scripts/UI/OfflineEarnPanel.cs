using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineEarnPanel : UI.Panel
{
    [SerializeField]
    private TMPro.TextMeshProUGUI totalCashText;
    [SerializeField]
    private ParticleSystem cashPS;
    [SerializeField]
    private AudioClip rewardSFX;
    public override void PostInit()
    {
    }
    public void SetUp(int totalCash)
    {
        Sound.Controller.Instance.PlayOneShot(rewardSFX);
        totalCashText.text = $"${GameUtility.GameUtility.ShortenNumber(totalCash)}";
        Show();
        cashPS.Play();
    }
    public override void Close()
    {
        base.Close();
        ((StageGameController)Game.Controller.Instance.gameController).hideMonster = false;
    }
}
