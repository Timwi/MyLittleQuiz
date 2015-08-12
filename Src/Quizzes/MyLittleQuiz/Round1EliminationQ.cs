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
        public object AnswerObject { get; private set; }

        public Round1EliminationQ(string undoLine, Round1Elimination prev, Difficulty difficulty, object answerObj = null)
            : base(undoLine)
        {
            Prev = prev;
            Difficulty = difficulty;
            AnswerObject = answerObj;
        }

        private Round1EliminationQ() { }    // for Classify

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\n{1/Cyan}\n\n{2/White}\n{3/Green}{4}\n\n{5}".Color(null).Fmt(
                    /* 0 */ "Question:",
                    /* 1 */ Prev.Questions[Difficulty][0].QuestionFullText,
                    /* 2 */ "Answer(s):",
                    /* 3 */ Prev.Questions[Difficulty][0].AnswerFullText,
                    /* 4 */ AnswerObject == null ? null : "\n\nAnswer given".Color(ConsoleColor.White),
                    /* 5 */ Prev.Describe
                );
            }
        }

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                if (AnswerObject == null)
                    return Prev.Questions[Difficulty][0].AnswerInfos.Select(answerInfo => Transition.Simple(
                        answerInfo.Item1, answerInfo.Item2,
                            () => new Round1EliminationQ("Answer given: " + answerInfo.Item2, Prev, Difficulty, answerInfo.Item3).With("r1_showA", new { answer = answerInfo.Item3 })));

                return Ut.NewArray(
                    Transition.Simple(ConsoleKey.Spacebar, "Back to contestant selection", () => new Round1Elimination("Dismiss question",
                        // Remove that question
                        Prev.Questions.Select(kvp => Ut.KeyValuePair(kvp.Key, kvp.Key == Difficulty ? kvp.Value.RemoveIndex(0) : kvp.Value)).ToDictionary(),
                        // Update the contestant’s score
                        Prev.Contestants.ReplaceIndex(Prev.SelectedContestant.Value, c => c.IncR1Score(!AnswerObject.Equals(false)))))
                );
            }
        }

        public override string JsMethod { get { return AnswerObject == null ? "r1_showQ" : "r1_showQA"; } }
        public override object JsParameters { get { return new { question = Prev.Questions[Difficulty][0], answer = AnswerObject }; } }
    }
}
