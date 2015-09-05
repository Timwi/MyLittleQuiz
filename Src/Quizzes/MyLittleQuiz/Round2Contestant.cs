using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2Contestant : IToConsoleColoredString, ICloneable
    {
        public string Name { get; private set; }
        public int Score { get; private set; }
        public int Passes { get; private set; }

        public Round2Contestant(string name, int score)
        {
            Name = name;
            Score = score;
            Passes = 3;
        }

        private Round2Contestant() { Passes = 3; }  // for Classify

        public Round2Contestant IncScore(int amount)
        {
            return this.ApplyToClone(c => { c.Score = Score + amount; });
        }

        public Round2Contestant DecPasses()
        {
            return this.ApplyToClone(c => { c.Passes = Passes - 1; });
        }

        public ConsoleColoredString ToConsoleColoredString()
        {
            return "{0/Yellow}{1/DarkYellow} Score={2/Cyan}, {3/DarkRed}{4/Red}".Color(ConsoleColor.DarkCyan).Fmt(Name, ",", Score, "Passes=", Passes);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
