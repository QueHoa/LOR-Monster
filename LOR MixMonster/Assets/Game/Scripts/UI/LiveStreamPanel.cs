using Cysharp.Threading.Tasks;
using GameUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UI;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class LiveStreamPanel : Panel
{
    [SerializeField]
    private HapticTypes hapticTypes = HapticTypes.Warning;
    [SerializeField]
    private TMPro.TextMeshProUGUI viewPointText, likePointText;
    [SerializeField]
    private RectTransform viewPointRect, likePointRect,tapRect,handRect;
    [SerializeField]
    private Transform bonusViewIcon, bonusLikeIcon;
    [SerializeField]
    private AudioClip viewGainSFX,clickSFX;
    [SerializeField]
    private ParticleSystem reactionPS;
    [SerializeField]
    private GameObject handTut;
    [SerializeField]
    private RewardBar rewardBar;
    private bool hapticsAllowed = true;
    CancellationTokenSource cancellation;
    private void OnEnable()
    {
        cancellation = new CancellationTokenSource();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
        }
    }
    private void OnDisable()
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
        }
    }
    public override void PostInit()
    {
    }
    List<UniTask> tasks = new List<UniTask>();
    Effect.EffectAbstract effect;
    int totalViewPoint, totalLikePoint;
    public async UniTask SetUp(int totalViewPoint,int totalLikePoint)
    {
        MMVibrationManager.SetHapticsActive(hapticsAllowed);
        Effect.EffectSpawner.Instance.Get(4, effect =>
        {
            this.effect = effect;
            this.effect.Active();
        }).Forget();
        Show();
        tasks.Clear();
        tasks.Add(FillView(viewPointRect,viewPointText, totalViewPoint));
        tasks.Add(FillLike(likePointRect,likePointText, totalLikePoint));

        ((MakeOverGameController)Game.Controller.Instance.gameController).monster.transform.GetChild(0).Shake(0.15f, 0.8f, 0.1f, cancellationToken: cancellation.Token);
        
        await UniTask.WhenAll(tasks);
        await UniTask.Delay(1000, cancellationToken: cancellation.Token);
    }
    public override void Deactive()
    {
        base.Deactive();
        effect.Deactive();
    }
    [SerializeField]
    private AnimationCurve increaseRateCurve;
    public int bonusView,bonusLike;
    async UniTask FillView(RectTransform rect,TMPro.TextMeshProUGUI text,int target)
    {
        int step = target / 15;
        float time = 0;
        int current = 0;

        float effectTime = 0;
        do
        {
            current += (int)(step*increaseRateCurve.Evaluate(time));
            current = current < target ? current : target;
            totalViewPoint = current;
            text.text = GameUtility.GameUtility.ShortenNumber(current+bonusView);
            time += Time.fixedDeltaTime;
            if (effectTime > 0.08f)
            {
                effectTime = 0;
                rect.Shake(0.15f, 2, 1f, cancellationToken: cancellation.Token);
                Sound.Controller.Instance.PlayOneShot(viewGainSFX,0.1f);
            }
            if (rewardBar.gameObject.activeSelf)
                rewardBar.UpdateBar(current + bonusView);
            effectTime += Time.fixedDeltaTime;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate,cancellationToken:cancellation.Token);
        } while (current < target);
    }
    async UniTask FillLike(RectTransform rect, TMPro.TextMeshProUGUI text, int target)
    {
        int step = target / 15;
        float time = 0;
        int current = 0;

        float effectTime = 0;
        do
        {
            current += (int)(step * increaseRateCurve.Evaluate(time));
            current = current < target ? current : target;
            totalLikePoint = current;
            text.text = GameUtility.GameUtility.ShortenNumber(current + bonusLike);
            time += Time.fixedDeltaTime;
            if (effectTime > 0.08f)
            {
                effectTime = 0;
                rect.Shake(0.15f, 2, 1f, cancellationToken: cancellation.Token);
                Sound.Controller.Instance.PlayOneShot(viewGainSFX, 0.1f);
            }
            effectTime += Time.fixedDeltaTime;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellation.Token);
        } while (current < target);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Sound.Controller.Instance.PlayOneShot(clickSFX,0.4f);
            if (Sound.Controller.VibrationEnable)
            {
                MMVibrationManager.Haptic(hapticTypes, true, true, this);
            }
            reactionPS.Play();
            ((MakeOverGameController)Game.Controller.Instance.gameController).monster.transform.GetChild(0).Shake(0.15f, 0.8f, 0.1f,cancellationToken:cancellation.Token);
            bonusView += UnityEngine.Random.Range(10000, 30000);
            bonusLike += UnityEngine.Random.Range(1000, 3000);
            bonusViewIcon.gameObject.SetActive(true);
            bonusLikeIcon.gameObject.SetActive(true);
            bonusLikeIcon.Shake(0.1f, 1, 0.3f);
            bonusViewIcon.Shake(0.1f, 1, 0.3f);
            likePointText.text = GameUtility.GameUtility.ShortenNumber(totalLikePoint + bonusLike);
            viewPointText.text = GameUtility.GameUtility.ShortenNumber(totalViewPoint + bonusView);


            likePointRect.Shake(0.15f, 2, 1f, cancellationToken: cancellation.Token);
            viewPointRect.Shake(0.15f, 2, 1f, cancellationToken: cancellation.Token);
            tapRect.Shake(0.15f, 2, 1.5f, cancellationToken: cancellation.Token);
            handRect.Shake(0.15f, 2, 1.5f, cancellationToken: cancellation.Token);
            Sound.Controller.Instance.PlayOneShot(viewGainSFX, 0.1f);

            CancelInvoke(nameof(DeactiveBonusIcon));
            Invoke(nameof(DeactiveBonusIcon),0.15f);
        }
    }
    public void DeactiveUI()
    {
        handTut.SetActive(false);
        rewardBar.gameObject.SetActive(false);
        viewPointRect.anchoredPosition = new Vector2(viewPointRect.anchoredPosition.x, viewPointRect.anchoredPosition.y - 30);
        likePointRect.anchoredPosition = new Vector2(likePointRect.anchoredPosition.x, likePointRect.anchoredPosition.y - 30);
    }
    void DeactiveBonusIcon()
    {
        bonusViewIcon.gameObject.SetActive(false);
        bonusLikeIcon.gameObject.SetActive(false);
    }
}
