using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util.Consoles;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;

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

        private static TransitionResult resultFromState(QuizStateBase state, string name)
        {
            if (state == null)
                return null;
            return new TransitionResult(state, name, state.JsMethod, state.JsParameters);
        }

        public static Transition Simple(ConsoleKey key, string name, Func<TransitionResult> executor)
        {
            return new Transition(key, name, () =>
            {
                var ret = executor();
                if (ret != null && ret.UndoLine == null)
                    ret = new TransitionResult(ret.State, name, ret.JsMethod, ret.JsParameters);
                return ret;
            });
        }

        public static Transition Simple(ConsoleKey key, string name, Func<QuizStateBase> executor)
        {
            return new Transition(key, name, () => resultFromState(executor(), name));
        }

        public static Transition Simple(ConsoleKey key, string name, Action action)
        {
            return new Transition(key, name, () => { action(); return null; });
        }

        public static Transition Simple(ConsoleKey key, string name, string jsMethod, object jsParameters = null)
        {
            return new Transition(key, name, () => new TransitionResult(null, null, jsMethod, jsParameters));
        }

        public static Transition String(ConsoleKey key, string name, string prompt, Func<string, TransitionResult> executor, bool allowEmpty = false)
        {
            return new Transition(key, name, () =>
            {
                var str = InputBox.GetLine(prompt);
                if (str == null || (!allowEmpty && string.IsNullOrEmpty(str)))
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
                var str1 = InputBox.GetLine(prompt1);
                if (str1 == null || (!allowEmpty && string.IsNullOrEmpty(str1)))
                    return null;

                var str2 = InputBox.GetLine(prompt2);
                if (str2 == null || (!allowEmpty && string.IsNullOrEmpty(str2)))
                    return null;

                return executor(str1, str2);
            });
        }

        public static Transition String(ConsoleKey key, string name, string prompt1, string prompt2, Action<string, string> executor, bool allowEmpty = false)
        {
            return String(key, name, prompt1, prompt2, (s1, s2) => { executor(s1, s2); return null; }, allowEmpty);
        }

        public static Transition Select<T>(ConsoleKey key, string name, IEnumerable<T> selection, Func<T, ConsoleColoredString> describe, Func<T, TransitionResult> executor)
        {
            return new Transition(key, name, () =>
            {
                var selected = Program.ConsoleSelect(selection, describe);
                if (selected == null)
                    return null;
                var transition = executor(selected);
                if (transition != null && transition.UndoLine == null)
                    transition = new TransitionResult(transition.State, name, transition.JsMethod, transition.JsParameters);
                return transition;
            });
        }

        public static Transition Select<T>(ConsoleKey key, string name, IEnumerable<T> selection, Func<T, ConsoleColoredString> describe, Func<T, QuizStateBase> executor)
        {
            return Select(key, name, selection, describe, obj => resultFromState(executor(obj), name));
        }

        public static Transition Select<T>(ConsoleKey key, string name, IEnumerable<T> selection, Func<T, ConsoleColoredString> describe, Action<T> executor)
        {
            return Select(key, name, selection, describe, obj => { executor(obj); return (TransitionResult) null; });
        }

        public static Transition SelectIndex<T>(ConsoleKey key, string name, T[] selection, Func<int, TransitionResult> executor) where T : IToConsoleColoredString
        {
            return new Transition(key, name, () =>
            {
                var selected = Program.ConsoleSelect(Enumerable.Range(0, selection.Length).Cast<int?>(), index => "({0}) {1}".Color(ConsoleColor.White).Fmt(index.Value + 1, selection[index.Value].ToConsoleColoredString()));
                if (selected == null)
                    return null;
                var transition = executor(selected.Value);
                if (transition != null && transition.UndoLine == null)
                    transition = new TransitionResult(transition.State, name, transition.JsMethod, transition.JsParameters);
                return transition;
            });
        }

        public static Transition SelectIndex<T>(ConsoleKey key, string name, T[] selection, Func<int, QuizStateBase> executor) where T : IToConsoleColoredString
        {
            return SelectIndex(key, name, selection, index => resultFromState(executor(index), name));
        }

        public static Transition SelectIndex<T>(ConsoleKey key, string name, T[] selection, Action<int> executor) where T : IToConsoleColoredString
        {
            return SelectIndex(key, name, selection, index => { executor(index); return (TransitionResult) null; });
        }

        public TransitionResult Execute()
        {
            return _executor();
        }
    }
}
