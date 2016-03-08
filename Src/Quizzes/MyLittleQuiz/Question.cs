using System;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public abstract class QuestionBase : IToConsoleColoredString
    {
        [ClassifyIgnoreIfDefault]
        public bool Check = false;
        public abstract string QuestionFullText { get; }
        public abstract string AnswerFullText { get; }
        public ConsoleColoredString ToConsoleColoredString() { return (Check ? "[CHECK!] ".Color(ConsoleColor.Red) : "") + toConsoleColoredString(); }
        protected abstract ConsoleColoredString toConsoleColoredString();

        public ConsoleColoredString Describe(bool? answerGiven)
        {
            return "{0/White}\n{1/Cyan}\n\n{2/White}\n{3/Green}{4}".Color(null).Fmt(
                /* 0 */ "Question:",
                /* 1 */ QuestionFullText.WordWrap(ConsoleUtil.WrapToWidth()).JoinColoredString(Environment.NewLine),
                /* 2 */ "Answer:",
                /* 3 */ AnswerFullText.WordWrap(ConsoleUtil.WrapToWidth()).JoinColoredString(Environment.NewLine),
                /* 4 */ answerGiven == null ? null : "\n\nAnswer given".Color(answerGiven.Value ? ConsoleColor.Green : ConsoleColor.Red)
            );
        }
    }

    public abstract class TextQuestionBase : QuestionBase
    {
        public string QuestionText;
        public override string QuestionFullText { get { return QuestionText; } }
    }

    public sealed class SimpleQuestion : TextQuestionBase
    {
        public string Answer;
        public override string AnswerFullText { get { return Answer; } }
        protected override ConsoleColoredString toConsoleColoredString()
        {
            return "{0/Yellow} {1/Green}".Color(null).Fmt(QuestionText, Answer);
        }
    }
}
