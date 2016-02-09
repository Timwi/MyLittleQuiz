using System;
using System.Collections.Generic;
using System.Linq;
using RT.TagSoup;
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
        public abstract IEnumerable<Tuple<ConsoleKey, string, object>> CorrectAnswerInfos { get; }
        public ConsoleColoredString ToConsoleColoredString() { return (Check ? "[CHECK!] ".Color(ConsoleColor.Red) : "") + toConsoleColoredString(); }
        protected abstract ConsoleColoredString toConsoleColoredString();

        public ConsoleColoredString Describe(object answer)
        {
            return "{0/White}\n{1/Cyan}\n\n{2/White}\n{3/Green}{4}".Color(null).Fmt(
                /* 0 */ "Question:",
                /* 1 */ QuestionFullText.WordWrap(ConsoleUtil.WrapToWidth()).JoinColoredString(Environment.NewLine),
                /* 2 */ "Answer:",
                /* 3 */ AnswerFullText.WordWrap(ConsoleUtil.WrapToWidth()).JoinColoredString(Environment.NewLine),
                /* 4 */ answer == null ? null : "\n\nAnswer given".Color(answer.Equals(false) ? ConsoleColor.Red : ConsoleColor.Green)
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
        public override IEnumerable<Tuple<ConsoleKey, string, object>> CorrectAnswerInfos
        {
            get
            {
                yield return Tuple.Create(ConsoleKey.G, "Correct", (object) true);
            }
        }
        public override string AnswerFullText { get { return Answer; } }
        protected override ConsoleColoredString toConsoleColoredString()
        {
            return "{0/Yellow} {1/Green}".Color(null).Fmt(QuestionText, Answer);
        }
    }

    public sealed class NOfQuestion : TextQuestionBase
    {
        public string[] Answers = new string[0];
        public int N;
        public override IEnumerable<Tuple<ConsoleKey, string, object>> CorrectAnswerInfos
        {
            get
            {
                var set = Answers.Select((a, ix) => Tuple.Create(a, new[] { ix }));
                for (int i = 1; i < N; i++)
                    set = set.SelectMany(tup => Answers.Select((a, ix) => Tuple.Create(tup.Item1 + " + " + a, tup.Item2.Concat(ix).ToArray())).Skip(tup.Item2.Last() + 1));
                return set.Select((tup, i) => Tuple.Create((ConsoleKey) (ConsoleKey.A + i), "Correct: " + tup.Item1, (object) tup.Item2));
            }
        }
        public override string AnswerFullText { get { return Answers.JoinString("\n"); } }
        protected override ConsoleColoredString toConsoleColoredString()
        {
            return "{0/Yellow} {1/Cyan} {{ {2} }}".Color(null).Fmt(QuestionText, N, Answers.Select(a => a.Color(ConsoleColor.Red)).JoinColoredString(", "));
        }
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
}
