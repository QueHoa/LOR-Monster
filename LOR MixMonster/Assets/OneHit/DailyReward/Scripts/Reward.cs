using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DailyReward
{
    [Serializable]
    public class Reward
    {
        public EUnit unit;

        public int reward;

        [PreviewField(50)]
        public Sprite sprite;
    }
}