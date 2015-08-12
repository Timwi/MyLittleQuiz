using System;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Contestant : ICloneable
    {
        public string Name { get; private set; }
        public string Roll { get; private set; }
        public int Round1Correct { get; private set; }
        public int Round1Wrong { get; private set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Contestant(string name, string roll)
        {
            Name = name;
            Roll = roll;
            Round1Correct = 0;
            Round1Wrong = 0;
        }

        private Contestant() { }    // for Classify

        public override string ToString()
        {
            return "{0}, Roll={1}".Fmt(Name, Roll);
        }

        public Contestant IncR1Score(bool correct)
        {
            return correct ? this.ApplyToClone(c => { c.Round1Correct++; }) : this.ApplyToClone(c => { c.Round1Wrong++; });
        }
    }
}
