using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace Trophy.MyLittleQuiz
{
    public sealed class QuizData : IClassifyObjectProcessor
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

        public void BeforeSerialize() { }

        private static string _secret = "MLP:FiMSweetAppleAcres";
        public void AfterDeserialize()
        {
            //var encrypt = Ut.Lambda((string s) => s.Select((c, i) => c <= 32 || c >= 127 ? c : (char) ((c - 33 + _secret[i % _secret.Length] - 33) % (127 - 33) + 33)).JoinString().HtmlEscape(true, true));
            //var decrypt = Ut.Lambda((string s) =>
            //{
            //    var dec = s.Select((c, i) => c <= 32 || c >= 127 ? c : (char) ((c - 33 - (_secret[i % _secret.Length] - 33)) % (127 - 33) + 33)).JoinString();
            //    return dec.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");
            //});

            //foreach (var q in Round4Questions.OfType<SimpleQuestion>())
            //{
            //    q.QuestionText = encrypt(q.QuestionText);
            //    q.Answer = encrypt(q.Answer);
            //}

            //Round3Sets = Round3Sets.Select(set => new Round3Set(encrypt(set.Name), set.Answers.Select(encrypt).ToArray())).ToArray();
        }
    }
}
