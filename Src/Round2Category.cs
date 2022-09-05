using System;
using RT.Serialization;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy.MyLittleQuiz
{
    public sealed class Round2Category : IToConsoleColoredString
    {
        public string Name { get; private set; }
        [ClassifyNotNull]
        public QuestionBase[] Questions { get; private set; }

        public Round2Category(string name, QuestionBase[] questions, bool[] questionTaken = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (questions == null)
                throw new ArgumentNullException("questions");

            Name = name;
            Questions = questions;
        }

        private Round2Category()    // for Classify
        {
            Questions = new QuestionBase[0];
        }

        public ConsoleColoredString ToConsoleColoredString()
        {
            return "{0/Yellow} ({1/Cyan} questions)".Color(null).Fmt(Name, Questions.Length);
        }
    }
}
