using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;
using RT.Util.Text;

namespace Trophy.MyLittleQuiz
{
    public sealed class Round3_SetPoker_Reveal : QuizStateBase
    {
        public Round3Data Data { get; private set; }

        public Round3_SetPoker_Reveal(Round3Data data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            Data = data;
        }

        private Round3_SetPoker_Reveal() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.S, "Show teams", () => new Round3_SetPoker_ShowTeams(Data));

                if (Data.TeamA.Score > 1 || Data.TeamB.Score > 1)
                    yield return Transition.Simple(ConsoleKey.N, "Next round: Final",
                        () => new Round4_Final_Start(new Round4Data(Data.QuizData, (Data.TeamA.Score > 1 ? Data.TeamA : Data.TeamB).Contestants)));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                var remaining = Data.QuizData.Round3Sets[Data.SetIndex - 1].Answers.Except(Data.AnswersGiven).ToArray();
                TextTable bestTable = null;
                int? bestWidth = null;
                for (int cols = 1; cols <= 10; cols++)
                {
                    var tt = new TextTable { ColumnSpacing = 2 };
                    var rows = (remaining.Length + cols - 1) / cols;
                    var widths = new int[cols];
                    for (int i = 0; i < remaining.Length; i++)
                    {
                        var col = i / rows;
                        widths[col] = Math.Max(widths[col], remaining[i].Length);
                        tt.SetCell(col, i % rows, remaining[i], noWrap: true);
                    }
                    var width = widths.Sum() + 2 * (cols - 1);
                    if (bestWidth == null || (width < ConsoleUtil.WrapToWidth() && width > bestWidth.Value))
                    {
                        bestTable = tt;
                        bestWidth = width;
                    }
                }

                return "Revealing set {0/Yellow}:\n\n{1}".Color(ConsoleColor.Green).Fmt(Data.QuizData.Round3Sets[Data.SetIndex - 1].Name, bestTable);
            }
        }

        public override string JsMethod { get { return "r3_reveal"; } }
        public override string JsMusic { get { return null; } }
        public override object JsParameters
        {
            get
            {
                var set = Data.QuizData.Round3Sets[Data.SetIndex - 1];
                return new { set = set.Name, remaining = set.Answers.Except(Data.AnswersGiven).ToArray() };
            }
        }
    }
}
