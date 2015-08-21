using System;
using System.Collections.Generic;
using System.Linq;
using RT.TagSoup;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public abstract class QuestionBase : IToConsoleColoredString
    {
        public abstract string QuestionFullText { get; }
        public abstract string AnswerFullText { get; }
        public abstract IEnumerable<Tuple<ConsoleKey, string, object>> AnswerInfos { get; }
        public abstract ConsoleColoredString ToConsoleColoredString();
    }

    public abstract class TextQuestionBase : QuestionBase
    {
        public string QuestionText;
        public override string QuestionFullText { get { return QuestionText; } }
    }

    public sealed class SimpleQuestion : TextQuestionBase
    {
        public string Answer;
        public override IEnumerable<Tuple<ConsoleKey, string, object>> AnswerInfos
        {
            get
            {
                yield return Tuple.Create(ConsoleKey.G, "Correct", (object) true);
                yield return Tuple.Create(ConsoleKey.Z, "Wrong", (object) false);
            }
        }
        public override string AnswerFullText { get { return Answer; } }
        public override ConsoleColoredString ToConsoleColoredString()
        {
            return "{0/Yellow} {1/Red}".Color(null).Fmt(QuestionText, Answer);
        }
    }

    public sealed class NOfQuestion : TextQuestionBase
    {
        public string[] Answers = new string[0];
        public int N;
        public override IEnumerable<Tuple<ConsoleKey, string, object>> AnswerInfos
        {
            get
            {
                var set = Answers.Select((a, ix) => Tuple.Create(a, new[] { ix }));
                for (int i = 1; i < N; i++)
                    set = set.SelectMany(tup => Answers.Select((a, ix) => Tuple.Create(tup.Item1 + " + " + a, tup.Item2.Concat(ix).ToArray())).Skip(tup.Item2.Last() + 1));
                var arr = set.ToArray();

                for (int i = 0; i < arr.Length; i++)
                    yield return Tuple.Create((ConsoleKey) (ConsoleKey.A + i), "Correct: " + arr[i].Item1, (object) arr[i].Item2);
                yield return Tuple.Create(ConsoleKey.Z, "Wrong", (object) false);
            }
        }
        public override string AnswerFullText { get { return Answers.JoinString("\n"); } }
        public override ConsoleColoredString ToConsoleColoredString()
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
