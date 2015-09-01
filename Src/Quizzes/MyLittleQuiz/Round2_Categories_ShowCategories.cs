using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    class Round2_Categories_ShowCategories : QuizStateBase
    {
        private Round2Data Data;

        public Round2_Categories_ShowCategories(Round2Data data)
        {
            Data = data;
        }

        private Round2_Categories_ShowCategories() { }  // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                if (Data.NextCategoryToPresent != null)
                    yield return Transition.Simple(ConsoleKey.P, "Present category " + Data.Categories[Data.NextCategoryToPresent.Value].Name, () =>
                        new Round2_Categories_ShowCategories(Data.PresentCategory()));
                else if (Data.SelectedCategory == null)
                    yield return Transition.Select(ConsoleKey.C, "Select a category", Data.QuizData.Round2Categories.Select(cat => cat.Name).ToArray<object>(),
                        index => new Round2_Categories_ShowCategories(Data.SelectCategory(index)).With("r2_selectCat", new { selected = index }));
                else
                {
                    var sel = Data.SelectedCategory.Value;
                    var difficulties = new[] { "Very easy", "Easy", "Medium", "Hard", "Very hard" };
                    yield return Transition.Select(ConsoleKey.Q, "Select a question",
                        Data.QuestionsUsed[sel].Select((taken, index) => difficulties[index] + (taken ? " (taken)" : "")).ToArray<object>(),
                        index => new Round2_Categories_Q(Data.SelectQuestion(index)));
                }
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
