using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class QuizData
    {
        // Round 1: Elimination
        [EditorLabel("Round 1 (Elimination): Number of contestants needed")]
        public int Round1NumContestantsNeeded { get; private set; }

        [ClassifyNotNull]
        [EditorLabel("Round 1 (Elimination): Questions")]
        public Dictionary<Difficulty, QuestionBase[]> Round1Questions { get; private set; }

        // Round 2: Categories
        [ClassifyNotNull]
        [EditorLabel("Round 2 (Categories): Categories")]
        public Round2Category[] Round2Categories { get; private set; }

        // Round 3: Set poker

        // Round 4: Sequences

        // Round 5: Sudden Death
        [ClassifyNotNull]
        [EditorLabel("Round 5 (Sudden Death): Questions")]
        public QuestionBase[] Round5Questions { get; private set; }

        public QuizData()
        {
            Round1Questions = new Dictionary<Difficulty, QuestionBase[]>();
            Round2Categories = new Round2Category[0];
            Round5Questions = new QuestionBase[0];
        }
    }
}
