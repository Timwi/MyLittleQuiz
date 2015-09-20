using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round4Data : ICloneable
    {
        [ClassifyNotNull]
        public string[] Contestants { get; private set; }

        public Round4Data(string[] contestants)
        {
            Contestants = contestants;
        }

        private Round4Data() { }    // for Classify

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
