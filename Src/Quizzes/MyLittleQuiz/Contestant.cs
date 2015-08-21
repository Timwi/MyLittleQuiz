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
    public sealed class Contestant
    {
        public string Name { get; private set; }
        public string Roll { get; private set; }

        public Contestant(string name, string roll)
        {
            Name = name;
            Roll = roll;
        }

        private Contestant() { }    // for Classify

        public override string ToString()
        {
            return "{0}, Roll={1}".Fmt(Name, Roll);
        }
    }
}
