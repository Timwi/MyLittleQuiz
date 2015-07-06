using System;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine
{
    public sealed class Transition
    {
        public ConsoleKey Key { get; private set; }
        public string Name { get; private set; }

        private Func<QuizStateBase> _executor;

        private Transition(ConsoleKey key, string name, Func<QuizStateBase> executor)
        {
            Key = key;
            Name = name;
            _executor = executor;
        }

        public static Transition Simple(ConsoleKey key, string name, Func<QuizStateBase> executor)
        {
            return new Transition(key, name, executor);
        }

        public static Transition String(ConsoleKey key, string name, string prompt, Func<string, QuizStateBase> executor, bool cancelIfEmpty)
        {
            return new Transition(key, name, () =>
            {
                Console.Write(prompt);
                var str = Console.ReadLine();
                if (cancelIfEmpty && string.IsNullOrEmpty(str))
                    return null;
                return executor(str);
            });
        }

        public static Transition Select(ConsoleKey key, string name, string[] selection, Func<int, QuizStateBase> executor)
        {
            return new Transition(key, name, () =>
            {
                Console.WriteLine();
                for (int i = 0; i < selection.Length; i++)
                    ConsoleUtil.WriteLine("{0/Cyan}: {1/Green}".Color(null).Fmt(i + 1, selection[i]));
                var line = Console.ReadLine();
                int index;
                if (string.IsNullOrWhiteSpace(line) || !int.TryParse(line, out index) || index < 0 || index >= selection.Length)
                    return null;
                return executor(index);
            });
        }

        public QuizStateBase Execute()
        {
            return _executor();
        }
    }
}
