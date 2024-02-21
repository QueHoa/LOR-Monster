using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OneHit.Leaderboard
{
    public class LeaderboardPresenter : MonoBehaviour
    {
        private LeaderboardSystem _system;

        [FoldoutGroup("LeaderboardProperties")]
        public string nameOfLeaderboard = "HighScore";

        [FoldoutGroup("LeaderboardProperties")]
        public int maxCompetitor = 1000;

        [FoldoutGroup("LeaderboardProperties")]
        private int top = 21;

        void Awake()
        {
            _system = new LeaderboardSystem();
            _system.Init();
            _system.nameOfLeaderboard = nameOfLeaderboard;
            _system.maxCompetitor = maxCompetitor;
        }

        public GameObject loadingPanel;
        public InputNamePanel input;
        public TMP_InputField changename;
        [Header("Other define")] public Sprite defaultHolder, playerHolder;
        [Header("Other define")] public List<CompetitorPresenter> competitors;

        private void Start()
        {
            if (!UserProfile.IsExist())
            {
                string name = "player" + Random.Range(1000, 9999);
                UserProfile.SetUserName(name);
                input.nameInput.text = name;
                SubmitPlayerToLeaderboard();
                
            }
            else
            {
                CheatScore(DataManagement.DataManager.Instance.userData.progressData.bestViewPoint);
            }
            changename.text = UserProfile.GetUsername();
        }

        private async void RefreshLeaderboard()
        {
            loadingPanel.SetActive(true);
            DataSnapshot data = await _system.FetchData();

            int index = 0;
            competitors[^2].Active();
            competitors[^1].Active();
            foreach (var child in data.Children.Reverse())
            {
                Debug.Log(index);
                competitors[index].SetProperties(index + 1, child.Child("name").Value.ToString(),
                    int.Parse(child.Child("score").Value.ToString()), defaultHolder);
                index++;
                if (index >= top)
                {
                    break;
                }
            }

            competitors[^2].Deactive();
            competitors[^1].Deactive();
            int playerRank = await _system.GetPlayerRank();
            Debug.Log(playerRank);
            if(playerRank <= 3)
            {
                competitors[playerRank - 1].SetColorPlayer(Color.green);
            }
            if (playerRank > 3 && playerRank <= top)
            {
                competitors[playerRank - 1].SetAsThisPlayer(playerRank, playerHolder);
                competitors[playerRank - 1].SetColorPlayer(Color.white);
            }
            else if (playerRank > top)
            {
                //competitors[^2].SetAsBuffer();
                competitors[^1].SetAsThisPlayer(playerRank, playerHolder);
            }

            loadingPanel.SetActive(false);
        }
        public async void SubmitPlayerToLeaderboard()
        {
            bool res = await _system.AddCompetitor(input.GetInput(), DataManagement.DataManager.Instance.userData.progressData.bestViewPoint);
            Debug.Log(res);
            if (res)
            {
                //input.SetActive(false);
                UserProfile.SetUserName(input.GetInput());
                RefreshLeaderboard();
            }
            else
            {
                input.ShowNameExisted();
            }
        }

        public async void ChangeNamePlayerToLeaderboard()
        {
            UserProfile.SetUserName(input.GetInput());
            await _system.UpdatePlayerName();
            RefreshLeaderboard();
        }


        private void OnValidate()
        {
            competitors = GetComponentsInChildren<CompetitorPresenter>().ToList();
        }

        [Button]
        private async void CheatScore(int score)
        {
            await _system.UpdateThisPlayer(score);
            RefreshLeaderboard();
        }
    }
}
/*#if UNITY_EDITOR
        public static readonly string[] Names =
        {
            "Michael", "James", "John", "Robert", "William",
            "David", "Richard", "Charles", "George", "Joseph",
            "Thomas", "Christopher", "Daniel", "Matthew", "Anthony",
            "Joshua", "Andrew", "Jonathan", "Ryan", "Brandon",
            "Kevin", "Austin", "Justin", "Jacob", "Noah",
            "Ethan", "Gabriel", "William", "Alexander", "David",
            "Michael", "Matthew", "Christopher", "Anthony", "Daniel",
            "지민", "김", "이", "박", "장",
            "윤", "최", "강", "황", "서",
            "조", "안", "오", "송", "신",
            "한", "유", "정", "채", "고",
            "전", "홍", "배", "손", "임",
            "나", "이정", "김정", "박정", "장정",
            "佐藤", "鈴木", "高橋", "田中", "渡辺",
            "山本", "中村", "伊藤", "小林", "松本",
            "加藤", "山田", "斉藤", "中島", "村上",
            "吉田", "山口", "田村", "佐々木", "井上",
            "小川", "中川", "加藤", "山田", "斉藤",
            "中島", "村上", "吉田", "山口", "田村"
        };

        [Button]
        public async void GenerateFakeUser()
        {
            int cnt = 0;
            while (cnt++ < 200)
            {
                string newName = Names[UnityEngine.Random.Range(0, Names.Length)] + Random.Range(100000000, 999999999);
                _system.AddCompetitor(newName.Substring(0, 10), 100 * Random.Range(10000, 800000));
            }
        }
    }
#endif
}*/
