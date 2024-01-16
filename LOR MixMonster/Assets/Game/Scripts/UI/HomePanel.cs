using Cysharp.Threading.Tasks;
using DailyReward;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HomePanel : UI.Panel
{
    bool isProcessing = false;
    [SerializeField]
    private AudioClip playSFX;
    [SerializeField]
    private GameObject nativeAdBanner;
    [SerializeField]
    private TextMeshProUGUI goldText;
    public override void PostInit()
    {
    }
    Effect.EffectAbstract effect;
    public void SetUp()
    {
        Sound.Controller.Instance.PlayMusic(Sound.Controller.Instance.soundData.menuTheme[UnityEngine.Random.Range(0, Sound.Controller.Instance.soundData.menuTheme.Length)]);

        isProcessing = false;
        Effect.EffectSpawner.Instance.Get(4, effect =>
         {
             this.effect = effect;
             this.effect.Active();
         });
        nativeAdBanner.SetActive(DataManagement.DataManager.Instance.userData.progressData.playCount > 0);
        //goldText.text = DataManagement.DataManager.Instance.userData.YourGold.ToString();
        Show();
    }
    public override void Deactive()
    {
        base.Deactive();
        effect.Deactive();
    }
    public void Play(int type)
    {
        if (isProcessing) return;
        isProcessing = true;
        //Game.Controller.Instance.gameConfig.gameType = type;
        Sound.Controller.Instance.PlayOneShot(playSFX);
        LevelLoading.Instance.Active(() =>
        {
            Close();
            Game.Controller.Instance.gameController.SetUp();
        });
    }
    public void Collection()
    {
        if (isProcessing) return;
        isProcessing = true;
        Sound.Controller.Instance.PlayOneShot(playSFX);
        LevelLoading.Instance.Active(() =>
        {
            Close();
            Game.Controller.Instance.gameController.SetUpCollection();
        });
    }
    public void DailyReward()
    {
        if (isProcessing) return;
        isProcessing = true;
        UI.PanelManager.Create(typeof(DailyRewards), (panel, op) =>
        {
            nativeAdBanner.SetActive(false);
            ((DailyRewards)panel).SetUp();
            
            isProcessing = false;
        });
    }
    public void Setting()
    {
        if (isProcessing) return;
        isProcessing = true;
        UI.PanelManager.Create(typeof(SettingPopup), (panel, op) =>
        {
            ((SettingPopup)panel).SetUp();

            isProcessing = false;
        });
    }
    public void LeaderBoard()
    {
        if (isProcessing) return;
        isProcessing = true;
        Sound.Controller.Instance.PlayOneShot(playSFX);
        LevelLoading.Instance.Active(() =>
        {
            Close();
            UI.PanelManager.Create(typeof(LeaderBoardPanel), (panel, op) =>
            {
                ((LeaderBoardPanel)panel).SetUp();
            });
            LevelLoading.Instance.Close();
        });
    }
    public void Upgrade()
    {
        if (isProcessing) return;
        isProcessing = true;
        UI.PanelManager.Create(typeof(SettingPopup), (panel, op) =>
        {
            ((SettingPopup)panel).SetUp();

            isProcessing = false;
        });
    }
    
}
public interface IBooster
{
    bool Active(StageGameController gameController);
    void Remove(StageGameController gameController);
    void OnUpdate(StageGameController gameController);
    bool IsActive();
    void CheckRefill();
    float GetDuration();
    float GetCurrentCoolDown();
    void AddOnBoosterActive(System.Action onBoosterActive);
    void AddOnBoosterRefilled(System.Action onBoosterRefilled);
    void SetActive(bool isActivated);
}
public enum EBooster
{
    InstantMoney,
    SpeedBoost,
    AutoClick
}
