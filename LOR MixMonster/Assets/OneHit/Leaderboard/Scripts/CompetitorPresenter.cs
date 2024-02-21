using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneHit.Leaderboard
{
    public class CompetitorPresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rank;
        [SerializeField] private new TextMeshProUGUI username;
        [SerializeField] private TextMeshProUGUI score;
        [SerializeField] private Image holder;

        public void SetProperties(int rank, string name, int score, Sprite sprite)
        {
            if (this.rank != null)
                this.rank.text = rank.ToString();
            this.username.text = name;
            if(name.Length > 10)
            {
                username.text = name.Substring(0, 10).Trim();
            }
            this.score.text = GameUtility.GameUtility.ShortenNumber(score);
            if (this.holder != null)
                holder.sprite = sprite;
        }

        public void SetAsBuffer()
        {
            if (this.rank != null)
                this.rank.text = String.Empty;
            this.username.text = "~~~~~~~~~~~~~~~";
            this.score.text = String.Empty;
            Active();
        }
        
        public void SetAsThisPlayer(int rank, Sprite sprite)
        {
            if (this.rank != null)
                this.rank.text = rank.ToString();
            this.username.text = UserProfile.GetUsername();

            if (rank > 20)
                this.score.text = GameUtility.GameUtility.ShortenNumber(DataManagement.DataManager.Instance.userData.progressData.bestViewPoint);


            if (this.holder != null)
                holder.sprite = sprite;
            Active();
        }
        public void SetColorPlayer(Color color)
        {
            if(rank != null)
                rank.color = color;
            username.color = color;
            score.color = color;
        }
        public void Active()
        {
            gameObject.SetActive(true);
        }

        public void Deactive()
        {
            gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            if (this.rank != null)
            {
                holder = GetComponent<Image>();
            }
        }
    }
}