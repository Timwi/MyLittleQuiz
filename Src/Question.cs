using System;
using RT.Serialization;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy.MyLittleQuiz
{
    public abstract class QuestionBase : IToConsoleColoredString
    {
        [ClassifyIgnoreIfDefault]
        public bool Check = false;
        public abstract string QuestionFullText { get; }
        public abstract string AnswerFullText { get; }
        public string HostHint;
        public ConsoleColoredString ToConsoleColoredString() { return (Check ? "[CHECK!] ".Color(ConsoleColor.Red) : "") + toConsoleColoredString(); }
        protected abstract ConsoleColoredString toConsoleColoredString();
        public int Priority = 1;

        public ConsoleColoredString Describe(bool? answerGiven)
        {
            return "{0/White}\n{1/Cyan}\n\n{5}{2/White}\n{3/Green}{4}".Color(null).Fmt(
                /* 0 */ "Question:",
                /* 1 */ QuestionFullText.WordWrap(ConsoleUtil.WrapToWidth()).JoinColoredString(Environment.NewLine),
                /* 2 */ "Answer:",
                /* 3 */ AnswerFullText.WordWrap(ConsoleUtil.WrapToWidth()).JoinColoredString(Environment.NewLine),
                /* 4 */ answerGiven == null ? null : "\n\nAnswer given".Color(answerGiven.Value ? ConsoleColor.Green : ConsoleColor.Red),
                /* 5 */ HostHint == null ? null : "{0/White}\n{1/Magenta}\n\n".Color(null).Fmt("Host hint:", HostHint)
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
            return (Priority < 2 ? "{0/DarkYellow} {1/DarkGreen}" : "{0/Yellow} {1/Green}").Color(null).Fmt(QuestionText, Answer);
        }
    }
}
