using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using UIHandler;

namespace DailyReward
{
    // The States a reward can have
    public enum DailyRewardState
    {
        Claimed,
        UnclaimAvailable,
        UnclaimUnavailable,
    }

    public class UIDailyReward : MonoBehaviour
    {
        public bool showRewardName;

        [Title("Assets")]
        public GameObject selecting;
        
        [Title("UI Elements")]
        public TextMeshProUGUI textDay;
        public GameObject textReward;
        public Image rewardImage;
        public ImageButtonAnimation anim;
        public GameObject tick;
        public RectTransform effect;

        [ReadOnly] public int day;
        [ReadOnly] public Reward reward;
        [ReadOnly] public DailyRewardState state;

        public void Initialize()
        {
            textDay.text = $"Day {day.ToString()}";

            /*if (reward.reward > 0)
                textReward.text = reward.reward + " " + (showRewardName ? reward.unit : "");
            else
                textReward.text = reward.unit.ToString();*/

            rewardImage.sprite = reward.sprite;
        }

        public void Refresh()
        {
            // reset
            tick.SetActive(false);
            textReward.SetActive(true);
            anim.enabled = false;
            if(day == 3 || day == 5 || day == 7)
            {
                effect.gameObject.SetActive(true);
                effect.DOKill();
                effect.localRotation = Quaternion.Euler(0f, 0f, 0f);
                effect.DORotate(new Vector3(0, 0, -360), 5.5f).SetEase(Ease.Linear).SetLoops(-1).SetRelative(true);
            }
            else
            {
                effect.gameObject.SetActive(false);
            }

            switch (state)
            {
                case DailyRewardState.Claimed:
                    selecting.SetActive(false);
                    tick.SetActive(true);
                    textReward.SetActive(false);
                    effect.gameObject.SetActive(false);
                    break;
                case DailyRewardState.UnclaimAvailable:
                    selecting.SetActive(true);
                    anim.enabled = true;
                    if (day == 3 || day == 5 || day == 7)
                    {
                        effect.gameObject.SetActive(true);
                    }
                    else
                    {
                        effect.gameObject.SetActive(false);
                    }
                    break;
                case DailyRewardState.UnclaimUnavailable:
                    selecting.SetActive(false);
                    if (day == 3 || day == 5 || day == 7)
                    {
                        effect.gameObject.SetActive(true);
                    }
                    else
                    {
                        effect.gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }
}