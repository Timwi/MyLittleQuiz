using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2Contestant
    {
        public string Name { get; private set; }
        public int Score { get; private set; }

        public Round2Contestant(string name, int score)
        {
            Name = name;
            Score = score;
        }

        public Round2Contestant IncScore(int amount)
        {
            return new Round2Contestant(Name, Score + amount);
        }
    }
}
