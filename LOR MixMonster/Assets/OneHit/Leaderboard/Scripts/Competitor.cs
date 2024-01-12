namespace OneHit.Leaderboard
{
    public class Competitor
    {
        public string id;
        public string name;
        public int score;
        public string extraToken;

        public Competitor()
        {
        }

        public Competitor(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
        public Competitor(string name, int score,string extra)
        {
            this.name = name;
            this.score = score;
            this.extraToken = extra;
        }
    }
}