using System;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round3Team : ICloneable,IToConsoleColoredString
    {
        public string[] ContestantNames { get; private set; }
        public bool HasPoint { get; private set; }
        public Round3Team(string[] contestantNames)
        {
            ContestantNames = contestantNames;
            HasPoint = false;
        }

        private Round3Team() { }    // for Classify

        public Round3Team ScorePoint()
        {
            return this.ApplyToClone(r3t => { r3t.HasPoint = true; });
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public ConsoleColoredString ToConsoleColoredString()
        {
            return "{0/Yellow} contestants".Color(ConsoleColor.DarkYellow).Fmt(ContestantNames.Length) +
                (HasPoint ? " (1 point)".Color(ConsoleColor.Green) : null);
        }
    }
}
