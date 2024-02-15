using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneHit.Leaderboard
{
    public class InputNamePanel : MonoBehaviour
    {
        public TMP_InputField nameInput;
        //public GameObject nameExisted;
        //public Button cancelBtn;
        public string GetInput()
        {
            return nameInput.text.Trim();
        }

        public async void ShowNameExisted()
        {
            //nameExisted.SetActive(true);
            await UniTask.Delay(200);
            //nameExisted.SetActive(false);
        }

        public void ValidateInput(String newstring)
        {
            char[] invalidChars = { '.', ',', '$', '#', '[', ']', '/' };
            foreach (char invalidChar in invalidChars)
            {
                if (nameInput.text.Contains(invalidChar))
                {
                    nameInput.text = nameInput.text.Replace(invalidChar, '?');
                }
            }

            if (nameInput.text.Length > 10)
                nameInput.text = nameInput.text.Substring(0, 10).Trim();
        }

        /*public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            cancelBtn.onClick.RemoveAllListeners();
            if (UserProfile.IsExist())
            {
                cancelBtn.onClick.AddListener(() =>
                {
                    gameObject.SetActive(false);
                });
            }
            else
            {
                cancelBtn.onClick.AddListener(() =>
                {
                transform.parent.GetComponent<LeaderboardPresenter>().gameObject.SetActive(false);
                });
            }
        }*/
    }
}