using System;
using System.Linq;
using RT.Serialization;
using RT.Util.ExtensionMethods;

namespace Trophy.MyLittleQuiz
{
    public sealed class Round3Data : ICloneable
    {
        public QuizData QuizData { get; private set; }
        public Round3Team TeamA { get; private set; }
        public Round3Team TeamB { get; private set; }
        public int SetIndex { get; private set; } = 0;

        [ClassifyNotNull]
        public string[] AnswersGiven { get; private set; } = new string[0];
        [ClassifyIgnoreIfDefault]
        public int WrongAnswers { get; private set; } = 0;

        public Round3Set CurrentSet { get { return QuizData.Round3Sets[SetIndex]; } }

        public bool MusicStarted { get; private set; }
        public Round3Data StartMusic()
        {
            return this.ApplyToClone(c => { c.MusicStarted = true; });
        }

        public Round3Data(QuizData data, ContestantAndScore[] teamA, ContestantAndScore[] teamB)
        {
            QuizData = data;
            TeamA = new Round3Team(teamA);
            TeamB = new Round3Team(teamB);
        }
        private Round3Data() { }    // for Classify

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Round3Data InitSet()
        {
            return this.ApplyToClone(r3d =>
            {
                r3d.AnswersGiven = new string[0];
                r3d.WrongAnswers = 0;
            });
        }

        public Round3Data TeamAWins()
        {
            return this.ApplyToClone(r3d =>
            {
                r3d.TeamA = TeamA.IncScore();
                r3d.SetIndex++;
                r3d.MusicStarted = false;
            });
        }

        public Round3Data RemoveStrikes()
        {
            return this.ApplyToClone(r3d => { r3d.AnswersGiven = AnswersGiven.Subarray(0, AnswersGiven.Length - 2); });
        }

        public Round3Data TeamBWins()
        {
            return this.ApplyToClone(r3d =>
            {
                r3d.TeamB = TeamB.IncScore();
                r3d.SetIndex++;
                r3d.MusicStarted = false;
            });
        }

        public Round3Data GiveCorrectAnswer(string answer)
        {
            return this.ApplyToClone(r3d =>
            {
                r3d.AnswersGiven = AnswersGiven.Concat(answer).ToArray();
            });
        }

        public Round3Data GiveWrongAnswer(bool isTieBreak)
        {
            return this.ApplyToClone(r3d =>
           {
               if (isTieBreak)
                   r3d.AnswersGiven = AnswersGiven.Concat(new string[] { null }).ToArray();
               else
                   r3d.WrongAnswers++;
           });
        }
    }
}
