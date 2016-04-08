namespace Trophy.MyLittleQuiz
{
    public sealed class ContestantAndScore
    {
        public string Name { get; private set; }
        public int Score { get; private set; }

        public ContestantAndScore(string name, int score)
        {
            Name = name;
            Score = score;
        }

        private ContestantAndScore() { }    // for Classify
    }
}