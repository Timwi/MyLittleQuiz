using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;
using RT.Util.Dialogs;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round3_SetPoker_ShowSet : QuizStateBase
    {
        public Round3Data Data { get; private set; }

        public Round3_SetPoker_ShowSet(Round3Data data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            Data = data;
        }

        private Round3_SetPoker_ShowSet() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                if (Data.SetIndex == 2)
                {
                    yield return Transition.Simple(ConsoleKey.A, "Play tie breaker; Team A starts", () => new Round3_SetPoker_PlayTieBreaker(Data, true));
                    yield return Transition.Simple(ConsoleKey.B, "Play tie breaker; Team B starts", () => new Round3_SetPoker_PlayTieBreaker(Data, false));
                }
                else
                    yield return Transition.Simple(ConsoleKey.P, "Play", () =>
                    {
                        var bidStr = "";
                        while (true)
                        {
                            if ((bidStr = InputBox.GetLine("Bid?", bidStr, "Enter bid")) == null)
                                return null;
                            int bid;
                            if (int.TryParse(bidStr, out bid))
                                return new Round3_SetPoker_Play(Data, bid);
                        }
                    });
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\nTeam A: {1/Cyan}\nTeam B: {2/Cyan}\n\n{3/White} {4/Magenta}\n\n{5/Green}".Color(ConsoleColor.Yellow).Fmt(
                    /* 0 */ "Score:",
                    /* 1 */ Data.TeamA.Score,
                    /* 2 */ Data.TeamB.Score,
                    /* 3 */ "Current set:",
                    /* 4 */ Data.CurrentSet.Name,
                    /* 5 */ Data.SetIndex == 2 ? "Tie Breaker" : "Regular");
            }
        }

        public override string JsMethod { get { return "r3_showSet"; } }
        public override string JsMusic { get { return null; } }
        public override object JsParameters { get { return new { set = Data.CurrentSet.Name }; } }
    }
}
