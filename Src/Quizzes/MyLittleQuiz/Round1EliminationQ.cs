using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round1EliminationQ : QuizStateBase
    {
        public Round1Elimination Prev { get; private set; }
        public Difficulty Difficulty { get; private set; }

        public Round1EliminationQ(string undoLine, Round1Elimination prev, Difficulty difficulty)
            : base(undoLine)
        {
            Prev = prev;
            Difficulty = difficulty;
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                // 
                return "{0/White}\n{1/Cyan}\n\n{2/White}\n{3/Green}\n\n{4}".Color(null).Fmt(
                    /* 0 */ "Question:",
                    /* 1 */ Prev.Questions[Difficulty][0].QuestionFullText,
                    /* 2 */ "Answer(s):",
                    /* 3 */ Prev.Questions[Difficulty][0].AnswerFullText,
                    /* 4 */ Prev.Describe
                );
            }
        }

        public override IEnumerable<Transition> Transitions
        {
            get { throw new NotImplementedException(); }
        }

        public override string JsMethod
        {
            get { throw new NotImplementedException(); }
        }

        public override object JsParameters
        {
            get { throw new NotImplementedException(); }
        }
    }
}
