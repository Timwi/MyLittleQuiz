using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class QuizData
    {
        public Dictionary<Difficulty, QuestionBase[]> Round1Questions { get; private set; }
        public Round2Category[] Round2Questions { get; private set; }

        public QuestionBase[] Round5Questions { get; private set; }

        public QuizData(QuestionBase[] r1r5qs, Round2Category[] r2qs)
        {
            Round1Questions = r1r5qs.Where(q => q.Difficulty == Difficulty.Easy || q.Difficulty == Difficulty.Medium).GroupBy(q => q.Difficulty).ToDictionary(g => g.Key, g => g.ToArray());
            Round2Questions = r2qs;
            Round5Questions = r1r5qs.Where(q => q.Difficulty == Difficulty.Hard).ToArray();
        }
    }
}
