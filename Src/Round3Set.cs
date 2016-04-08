using System;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace Trophy.MyLittleQuiz
{
    public sealed class Round3Set : IToConsoleColoredString
    {
        public string Name { get; private set; }
        [ClassifyNotNull]
        public string[] Answers { get; private set; }

        public Round3Set(string name, string[] answers)
        {
            if (answers == null)
                throw new ArgumentNullException("answers");

            Name = name;
            Answers = answers;
        }

        private Round3Set() // for Classify
        {
            Answers = new string[0];
        }

        public ConsoleColoredString ToConsoleColoredString()
        {
            return "{0/Yellow} ({1/Green} answers)".Color(ConsoleColor.DarkGreen).Fmt(Name, Answers.Length);
        }
    }
}
