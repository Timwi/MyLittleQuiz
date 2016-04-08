using System;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace Trophy.MyLittleQuiz
{
    public sealed class Round1Contestant : ICloneable, IToConsoleColoredString
    {
        public string Name { get; private set; }
        public string Roll { get; private set; }

        [ClassifyIgnoreIfDefault]
        public int NumCorrect { get; private set; }
        [ClassifyIgnoreIfDefault]
        public int NumWrong { get; private set; }
        [ClassifyIgnoreIfDefault]
        public bool IsDisqualified { get; private set; }

        public bool IsOut { get { return NumWrong >= 2 || IsDisqualified; } }
        public bool IsThrough { get { return NumCorrect >= 2; } }
        public bool IsStillInGame { get { return !IsOut && !IsThrough && !IsDisqualified; } }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Round1Contestant(string name, string roll)
        {
            Name = name;
            Roll = roll;
            NumCorrect = 0;
            NumWrong = 0;
            IsDisqualified = false;
        }

        private Round1Contestant() { }    // for Classify

        public Round1Contestant IncScore(bool correct)
        {
            return correct ? this.ApplyToClone(c => { c.NumCorrect++; }) : this.ApplyToClone(c => { c.NumWrong++; });
        }

        public Round1Contestant Disqualify()
        {
            return this.ApplyToClone(c => { c.IsDisqualified = true; });
        }

        public override string ToString()
        {
            return "{0}, Roll={1}".Fmt(Name, Roll);
        }

        public ConsoleColoredString ToConsoleColoredString()
        {
            return "{0}, Roll={1}{2}".Color(IsStillInGame ? ConsoleColor.Gray : ConsoleColor.DarkRed).Fmt(
                Name.Color(IsStillInGame ? ConsoleColor.Yellow : ConsoleColor.Red),
                Roll.Color(IsStillInGame ? ConsoleColor.Cyan : ConsoleColor.Red),
                IsStillInGame ? null : IsDisqualified ? " (disqualified)" : IsOut ? " (out)" : IsThrough ? " (through)" : " (ERROR)");
        }
    }
}
