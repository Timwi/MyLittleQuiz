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

        public Round2Contestant(string name, int score)
        {
            Name = name;
            Score = score;
        }

        private Round2Contestant() { }  // for Classify

        public Round2Contestant IncScore(int amount)
        {
            return this.ApplyToClone(c => { c.Score = Score + amount; });
        }

        public ConsoleColoredString ToConsoleColoredString()
        {
            return "{0/Yellow}, Score={1/Cyan}".Color(ConsoleColor.DarkCyan).Fmt(Name, Score);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
