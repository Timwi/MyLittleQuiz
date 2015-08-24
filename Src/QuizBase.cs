using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util.Serialization;

namespace QuizGameEngine
{
    public abstract class QuizBase
    {
        protected QuizBase() { }    // for Classify

        [ClassifyNotNull]
        protected QuizStateBase _currentState;
        public QuizStateBase CurrentState { get { return _currentState; } }

        [ClassifyIgnoreIfDefault]
        public string UndoLine { get; private set; }

        [ClassifyNotNull, ClassifySubstitute(typeof(StackToListSubstitution<Tuple<QuizStateBase, string>>))]
        protected Stack<Tuple<QuizStateBase, string>> _undo = new Stack<Tuple<QuizStateBase, string>>();
        [ClassifyNotNull, ClassifySubstitute(typeof(StackToListSubstitution<Tuple<QuizStateBase, string>>))]
        protected Stack<Tuple<QuizStateBase, string>> _redo = new Stack<Tuple<QuizStateBase, string>>();

        public void Transition(QuizStateBase newState, string undoLine)
        {
            _redo.Clear();
            _undo.Push(Tuple.Create(CurrentState, UndoLine));
            _currentState = newState;
            UndoLine = undoLine;
        }

        public bool Undo()
        {
            if (_undo.Count == 0)
                return false;
            _redo.Push(Tuple.Create(CurrentState, UndoLine));
            var tup = _undo.Pop();
            _currentState = tup.Item1;
            UndoLine = tup.Item2;
            return true;
        }

        public bool Redo()
        {
            if (_redo.Count == 0)
                return false;
            _undo.Push(Tuple.Create(CurrentState, UndoLine));
            var tup = _redo.Pop();
            _currentState = tup.Item1;
            UndoLine = tup.Item2;
            return true;
        }

        public string RedoLine { get { return _redo.Count == 0 ? null : _redo.Peek().Item2; } }

        public abstract byte[] Css { get; }
        public abstract byte[] Js { get; }
        public abstract string CssJsFilename { get; }
    }
}
