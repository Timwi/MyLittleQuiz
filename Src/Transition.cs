using System;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Json;
using RT.Util.Serialization;

namespace QuizGameEngine
{
    public sealed class Transition
    {
        public ConsoleKey Key { get; private set; }
        public string Name { get; private set; }
        private Func<TransitionResult> _executor;

        private Transition(ConsoleKey key, string name, Func<TransitionResult> executor)
        {
            Key = key;
            Name = name;
            _executor = executor;
        }

        public static Transition Simple(ConsoleKey key, string name, Func<TransitionResult> executor)
        {
            return new Transition(key, name, executor);
        }

        public static Transition Simple(ConsoleKey key, string name, Action action)
        {
            return new Transition(key, name, () => { action(); return null; });
        }

        public static Transition Simple(ConsoleKey key, string name, string jsMethod, object jsParameters = null)
        {
            return new Transition(key, name, () => new TransitionResult(null, jsMethod, jsParameters));
        }

        public static Transition String(ConsoleKey key, string name, string prompt, Func<string, TransitionResult> executor, bool allowEmpty = false)
        {
            return new Transition(key, name, () =>
            {
                Console.Write(prompt);
                var str = Console.ReadLine();
                if (!allowEmpty && string.IsNullOrEmpty(str))
                    return null;
                return executor(str);
            });
        }

        public static Transition String(ConsoleKey key, string name, string prompt, Action<string> executor, bool allowEmpty = false)
        {
            return String(key, name, prompt, s => { executor(s); return null; }, allowEmpty);
        }

        public static Transition String(ConsoleKey key, string name, string prompt1, string prompt2, Func<string, string, TransitionResult> executor, bool allowEmpty = false)
        {
            return new Transition(key, name, () =>
            {
                Console.Write(prompt1);
                var str1 = Console.ReadLine();
                if (!allowEmpty && string.IsNullOrEmpty(str1))
                    return null;

                Console.Write(prompt2);
                var str2 = Console.ReadLine();
                if (!allowEmpty && string.IsNullOrEmpty(str2))
                    return null;

                return executor(str1, str2);
            });
        }

        public static Transition String(ConsoleKey key, string name, string prompt1, string prompt2, Action<string, string> executor, bool allowEmpty = false)
        {
            return String(key, name, prompt1, prompt2, (s1, s2) => { executor(s1, s2); return null; }, allowEmpty);
        }

        public static Transition Select(ConsoleKey key, string name, object[] selection, Func<int, TransitionResult> executor)
        {
            return new Transition(key, name, () =>
            {
                Console.WriteLine();
                for (int i = 0; i < selection.Length; i++)
                    ConsoleUtil.WriteLine("{0/Cyan}: {1/Green}".Color(null).Fmt(i + 1, selection[i]));
                var line = Console.ReadLine();
                int index;
                if (string.IsNullOrWhiteSpace(line) || !int.TryParse(line, out index) || index < 1 || index > selection.Length)
                    return null;
                return executor(index - 1);
            });
        }

        public static Transition Select(ConsoleKey key, string name, object[] selection, Action<int> executor)
        {
            return Select(key, name, selection, index => { executor(index); return null; });
        }

        public TransitionResult Execute()
        {
            return _executor();
        }
    }
}
