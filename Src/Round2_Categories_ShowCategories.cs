﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util;

namespace Trophy.MyLittleQuiz
{
    class Round2_Categories_ShowCategories : Round2_Categories_Base
    {
        public Round2_Categories_ShowCategories(Round2Data data) : base(data) { }
        private Round2_Categories_ShowCategories() { }  // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                if (Data.NextCategoryToPresent != null)
                {
                    var newData = Data.PresentCategory();
                    if (newData.NextCategoryToPresent != null)
                        yield return Transition.Simple(ConsoleKey.P, "Present category " + Data.Categories[newData.NextCategoryToPresent.Value].Name, () => new Round2_Categories_ShowCategories(newData).With(jsJingle: Jingle.Present.ToString()));
                    else
                        yield return Transition.Simple(ConsoleKey.P, "Show question difficulties", () => new Round2_Categories_ShowCategories(newData));
                }
                else if (Data.SelectedCategory == null)
                {
                    yield return Transition.Simple(ConsoleKey.S, "Show scores", () => new Round2_Categories_ShowContestants(Data).With(jsJingle: Jingle.Swoosh.ToString()));
                    yield return Transition.Select(ConsoleKey.C, "Select a category", Data.QuizData.Round2Categories,
                        cat => Data.QuestionsUsed[Data.Categories.IndexOf(cat)].All(used => used) ? cat.Name.Color(ConsoleColor.DarkYellow) + " (taken)".Color(ConsoleColor.DarkRed) : cat.Name.Color(ConsoleColor.Yellow),
                        cat => Data.Categories.IndexOf(cat).Apply(index => new Round2_Categories_ShowCategories(Data.SelectCategory(index)).With("r2_selectCat", new { selected = index }, jsJingle: Jingle.Present.ToString())));
                    yield return Transition.Simple(ConsoleKey.P, "Pass", () => new Round2_Categories_ShowCategories(Data.Pass()).NoTransition());
                }
                else
                {
                    var sel = Data.SelectedCategory.Value;
                    yield return Transition.Select(ConsoleKey.Q, "Select a question",
                        new[] { "Very easy", "Easy", "Medium", "Hard", "Very hard" }
                            .Select((df, ix) => new { Index = ix, Difficulty = df, Taken = Data.QuestionsUsed[sel][ix] }),
                        qs => qs.Difficulty.Color(qs.Taken ? ConsoleColor.DarkYellow : ConsoleColor.Yellow) + (qs.Taken ? " (taken)".Color(ConsoleColor.DarkRed) : null),
                        qs => qs.Taken ? null : new Round2_Categories_Q(Data.SelectQuestion(qs.Index).StartMusic()));
                }
                yield return listContestantsTransition;
            }
        }

        public override ConsoleColoredString Describe { get { return Data.Describe; } }

        public override string JsMethod
        {
            get { return Data.NextCategoryToPresent == null ? "r2_showCats" : "r2_presentCat"; }
        }

        public override object JsParameters
        {
            get
            {
                return new
                {
                    categories = Data.QuizData.Round2Categories.Select(cat => cat.Name).ToArray(),
                    used = Data.QuestionsUsed,
                    selected = Data.SelectedCategory,
                    index = Data.NextCategoryToPresent
                };
            }
        }
    }
}