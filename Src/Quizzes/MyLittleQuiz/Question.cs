﻿using System;
using System.Collections.Generic;
using System.Linq;
using RT.TagSoup;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public abstract class QuestionBase
    {
        public abstract string QuestionFullText { get; }
        public abstract string AnswerFullText { get; }
        public abstract string JsMethodForAsking { get; }
        public abstract object JsParametersForAsking { get; }
        public abstract IEnumerable<Tuple<ConsoleKey, string, object>> AnswerInfos { get; }
        public abstract string JsMethodForShowingAnswer(object answerInfo);
        public abstract object JsParametersForShowingAnswer(object answerInfo);

        public Difficulty Difficulty;
    }

    public abstract class TextQuestionBase : QuestionBase
    {
        public string QuestionText;

        public override string JsMethodForAsking { get { return "r1_showQ"; } }
        public override object JsParametersForAsking { get { return new { question = QuestionText, answerHtml = Tag.ToString(AnswerHtml) }; } }
        public override string JsMethodForShowingAnswer(object answerInfo) { return "r1_showA"; }
        public override string QuestionFullText { get { return QuestionText; } }

        public abstract object AnswerHtml { get; }
    }

    public sealed class SimpleQuestion : TextQuestionBase
    {
        public string Answer;
        public override object AnswerHtml { get { return Answer; } }
        public override IEnumerable<Tuple<ConsoleKey, string, object>> AnswerInfos
        {
            get
            {
                yield return Tuple.Create(ConsoleKey.G, "Correct", (object) true);
                yield return Tuple.Create(ConsoleKey.M, "Wrong", (object) false);
            }
        }
        public override object JsParametersForShowingAnswer(object answerInfo) { return new { answer = answerInfo }; }
        public override string AnswerFullText { get { return Answer; } }
    }

    public sealed class NOfQuestion : TextQuestionBase
    {
        public string[] Answers;
        public int N;
        public override object AnswerHtml { get { return new UL(Answers.Select(a => new LI(a))); } }
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
                yield return Tuple.Create(ConsoleKey.Z, "Wrong", (object) null);
            }
        }
        public override object JsParametersForShowingAnswer(object answerInfo) { return new { answer = answerInfo }; }
        public override string AnswerFullText { get { return Answers.JoinString("\n"); } }
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
}
