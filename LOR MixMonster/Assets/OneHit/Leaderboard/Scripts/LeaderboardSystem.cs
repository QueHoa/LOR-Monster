using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OneHit.Leaderboard
{
    public class LeaderboardSystem
    {
        DatabaseReference dbRef;
        internal string nameOfLeaderboard = "HighScore";
        private bool allowNameDuplicate = false;

        internal int maxCompetitor = 1000;

        public void Init()
        {
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        }

        [Button]
        public async UniTask<bool> AddCompetitor(string name = "PhucDev02", int score = 1234, string extra = "")
        {
            if (!allowNameDuplicate)
            {
                bool isExist = await IsExist(name);
                if (isExist)
                {
                    ActionIfNameExist();
                    return false;
                }
            }
            Competitor competitor = new Competitor(name, score, extra);
            competitor.id = GetId();
            UserProfile.SetProfile(competitor);
            await dbRef.Child(nameOfLeaderboard).Child(competitor.id).Child("name").SetValueAsync(competitor.name);
            await dbRef.Child(nameOfLeaderboard).Child(competitor.id).Child("score").SetValueAsync(competitor.score);
            await dbRef.Child(nameOfLeaderboard).Child(competitor.id).Child("extra")
                .SetValueAsync(competitor.extraToken);
            RemoveLastCompetitor();
            return true;
        }
        
        public async UniTask UpdateThisPlayer(int score)
        {
            UserProfile.SetScore(score);
            await dbRef.Child(nameOfLeaderboard).Child(UserProfile.GetId()).Child("score").SetValueAsync(score);            
        }
        protected virtual void ActionIfNameExist()
        {
            Debug.LogWarning("Name is exist");
        }

        [Button]
        private void RemoveCompetitor(string id)
        {
            dbRef.Child(nameOfLeaderboard).Child(id).RemoveValueAsync();
        }

        [Button]
        private async void RemoveLastCompetitor()
        {
            DataSnapshot snapshot = await FetchData();
            if (snapshot != null)
            {
                if (snapshot.ChildrenCount > maxCompetitor)
                {
                    RemoveCompetitor(snapshot.Children.First().Key);
                }
            }
        }

        [Button]
        public async UniTask<bool> IsExist(string userName = "PhucDev02")
        {
            bool res = false;
            DataSnapshot snapshot = await FetchData();
            if (snapshot != null)
            {
                foreach (var child in snapshot.Children)
                {
                    if (child.Child("name").Value.Equals(userName))
                    {
                        res = true;
                        break;
                    }
                }
            }

            return res;
        }

        public async UniTask<int> GetPlayerRank()
        {
            if (UserProfile.IsExist())
            {
                DataSnapshot snapshot = await FetchData();
                int index = 1;
                foreach (var child in snapshot.Children.Reverse())
                {
                    if (UserProfile.GetId() == child.Key)
                    {
                        return index;
                    }

                    index++;
                }
            }

            return Random.Range(maxCompetitor, maxCompetitor * 3);
        }

        [Button]
        public async UniTask<DataSnapshot> FetchData()
        {
            // loadingPanel.SetActive(true);
            try
            {
                var snapshot = await dbRef.Child(nameOfLeaderboard).OrderByChild("score").GetValueAsync();
                Debug.Log(snapshot.ChildrenCount);
                return snapshot;
            }
            catch (Exception ex)
            {
                Debug.Log("Fetch Error: " + ex.Message);
                return null;
            }
            finally
            {
                // loadingPanel.SetActive(false);
            }
        }

        public static string GetId()
        {
            DateTime now = DateTime.Now;
            return $"{now.Year}{now.Month}{now.Day}{now.Hour}_{now.Minute}_{now.Second}_{now.Millisecond}";
        }
    }
}