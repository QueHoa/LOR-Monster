using UnityEngine;

namespace OneHit.Leaderboard
{
    public class UserProfile
    {
        public static void SetProfile(Competitor competitor)
        {
            SetId(competitor.id);
            if(!IsExist()) return;
            SetUserName(competitor.name);
            SetScore(competitor.score);
            SetExtra(competitor.extraToken);
        }

        private static void SetExtra(string competitorExtraToken)
        {
            PlayerPrefs.SetString("LeaderboardExtra", competitorExtraToken);
        }
        public static string GetExtra()
        {
            return PlayerPrefs.GetString("LeaderboardExtra", "");
        }
        public static void SetId(string id)
        {
            PlayerPrefs.SetString("LeaderboardPlayerId", id);
        }
        public static string GetId()
        {
            return PlayerPrefs.GetString("LeaderboardPlayerId", "");
        }
        public static int GetHighscore()
        {
            return PlayerPrefs.GetInt("Highscore", 0);
        }
        public static void SetScore(int score)
        {
            PlayerPrefs.SetInt("Highscore", score);
        }
        public static void SetUserName(string name)
        {
            PlayerPrefs.SetString("LeaderboardUsername", name.Trim());
        }
        public static string GetUsername()
        {
            return PlayerPrefs.GetString("LeaderboardUsername", "");
        }
        public static bool IsExist()
        {
            return PlayerPrefs.HasKey("LeaderboardUsername");
        }
    }
}
