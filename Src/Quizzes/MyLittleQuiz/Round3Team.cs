using System;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round3Team : ICloneable, IToConsoleColoredString
    {
        [ClassifyNotNull]
        public string[] ContestantNames { get; private set; }
        public int Score { get; private set; }
        public Round3Team(string[] contestantNames)
        {
            ContestantNames = contestantNames;
            Score = 0;
        }

        private Round3Team() { }    // for Classify

        public Round3Team IncScore()
        {
            return this.ApplyToClone(r3t => { r3t.Score++; });
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public ConsoleColoredString ToConsoleColoredString()
        {
            return "{0/Yellow} contestants".Color(ConsoleColor.DarkYellow).Fmt(ContestantNames.Length) +
                (Score > 0 ? " ({0} points)".Color(ConsoleColor.Green).Fmt(Score) : null);
        }
    }
}
