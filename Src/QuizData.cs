using System.Collections.Generic;
using RT.Serialization;

namespace Trophy.MyLittleQuiz
{
    public sealed class QuizData
    {
        // Round 1: Elimination
        [ClassifyNotNull]
        [EditorLabel("Round 1 (Elimination): Questions")]
        public Dictionary<Difficulty, QuestionBase[]> Round1Questions { get; private set; } = new Dictionary<Difficulty, QuestionBase[]>();
        [EditorLabel("Round 1 (Elimination): Number of contestants needed")]
        public int Round1NumContestantsNeeded { get; private set; } = 10;

        // Round 2: Categories
        [ClassifyNotNull]
        [EditorLabel("Round 2 (Categories): Categories")]
        public Round2Category[] Round2Categories { get; private set; } = new Round2Category[0];
        [EditorLabel("Round 2 (Categories): Number of contestants needed")]
        public int Round2NumContestantsNeeded { get; private set; } = 6;

        // Round 3: Set poker
        [ClassifyNotNull]
        [EditorLabel("Round 3 (Set Poker): Sets")]
        public Round3Set[] Round3Sets { get; private set; } = new Round3Set[0];

        // Round 4: Final/Sudden Death
        [ClassifyNotNull]
        [EditorLabel("Round 4 (Final): Questions")]
        public QuestionBase[] Round4Questions { get; private set; } = new QuestionBase[0];

        [EditorLabel("Round 4 (Final): Best of n")]
        public int Round4MinQuestions { get; private set; } = 5;
    }
}
