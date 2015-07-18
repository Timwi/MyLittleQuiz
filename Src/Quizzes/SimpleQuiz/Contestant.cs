using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.SimpleQuiz
{
    public sealed class Contestant
    {
        public string Name { get; private set; }
        public int Score { get; private set; }
        public Contestant(string name, int score)
        {
            Name = name;
            Score = score;
        }
        private Contestant() { }    // for Classify

        public static implicit operator Contestant(string name) { return name == null ? null : new Contestant(name, 0); }

        public override string ToString() { return "{0} ({1})".Fmt(Name, Score); }
        public Contestant IncScore(int by = 1) { return new Contestant(Name, Score + by); }
    }
}
