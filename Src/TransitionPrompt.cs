using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGameEngine
{
    public sealed class TransitionPrompt
    {
        public string Name { get; private set; }
        public ConsoleKey Key { get; private set; }
        public Func<QuizStateBase> Executor;

        public TransitionPrompt(string name, ConsoleKey key, Func<QuizStateBase> executor)
        {
            Name = name;
            Key = key;
            Executor = executor;
        }

        public TransitionPrompt(ConsoleKey key, string name, string prompt, Func<string, QuizStateBase> executor, bool cancelIfEmpty)
        {
            Name = name;
            Key = key;
            Executor = () =>
            {
                Console.Write(prompt);
                var str = Console.ReadLine();
                if (cancelIfEmpty && string.IsNullOrEmpty(str))
                    return null;
                return executor(str);
            };
        }

        public QuizStateBase Execute()
        {
            return Executor();
        }
    }
}
