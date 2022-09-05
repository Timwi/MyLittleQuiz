﻿using System;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy.MyLittleQuiz
{
    public sealed class Round2Contestant : IToConsoleColoredString, ICloneable
    {
        public string Name { get; private set; }
        public int Score { get; private set; }

        public Round2Contestant(string name, int score)
        {
            Name = name;
            Score = score;
        }

        private Round2Contestant() { }  // for Classify

        public Round2Contestant IncScore(int amount)
        {
            return this.ApplyToClone(c => { c.Score = Score + amount; });
        }

        public ConsoleColoredString ToConsoleColoredString()
        {
            return
                "{0/Yellow}, ".Color(ConsoleColor.DarkYellow).Fmt(Name) +
                "Score={1/Cyan}".Color(ConsoleColor.DarkCyan).Fmt(Score);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
