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
    public sealed class Round3Data : ICloneable
    {
        public QuizData QuizData { get; private set; }
        public Round3Team TeamA { get; private set; }
        public Round3Team TeamB { get; private set; }
        public int SetIndex { get; private set; }

        public Round3Set CurrentSet { get { return QuizData.Round3Sets[SetIndex]; } }

        public Round3Data(QuizData data, string[] teamA, string[] teamB)
        {
            QuizData = data;
            TeamA = new Round3Team(teamA);
            TeamB = new Round3Team(teamB);
            SetIndex = 0;
        }
        private Round3Data() { }    // for Classify

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Round3Data TeamAWins()
        {
            return this.ApplyToClone(r3d =>
            {
                r3d.TeamA = TeamA.IncScore();
                r3d.SetIndex++;
            });
        }

        public Round3Data TeamBWins()
        {
            return this.ApplyToClone(r3d =>
            {
                r3d.TeamB = TeamB.IncScore();
                r3d.SetIndex++;
            });
        }
    }
}
