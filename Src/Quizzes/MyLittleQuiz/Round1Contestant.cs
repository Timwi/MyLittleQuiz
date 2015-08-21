using System;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round1Contestant : ICloneable
    {
        public string Name { get; private set; }
        public string Roll { get; private set; }

        [ClassifyIgnoreIfDefault]
        public int NumCorrect { get; private set; }
        [ClassifyIgnoreIfDefault]
        public int NumWrong { get; private set; }

        public bool IsOut { get { return NumWrong >= 2; } }
        public bool IsThrough { get { return NumCorrect >= 2; } }
        public bool IsStillInGame { get { return !IsOut && !IsThrough; } }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Round1Contestant(string name, string roll)
        {
            Name = name;
            Roll = roll;
            NumCorrect = 0;
            NumWrong = 0;
        }

        private Round1Contestant() { }    // for Classify

        public override string ToString()
        {
            return "{0}, Roll={1}".Fmt(Name, Roll);
        }

        public Round1Contestant IncScore(bool correct)
        {
            return correct ? this.ApplyToClone(c => { c.NumCorrect++; }) : this.ApplyToClone(c => { c.NumWrong++; });
        }
    }
}
