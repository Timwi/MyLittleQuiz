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

        protected QuizStateBase _currentState;
        public QuizStateBase CurrentState { get { return _currentState; } }

        [ClassifyNotNull, ClassifySubstitute(typeof(StackToListSubstitution<QuizStateBase>))]
        protected Stack<QuizStateBase> _undo = new Stack<QuizStateBase>();
        [ClassifyNotNull, ClassifySubstitute(typeof(StackToListSubstitution<QuizStateBase>))]
        protected Stack<QuizStateBase> _redo = new Stack<QuizStateBase>();

        public void Transition(QuizStateBase newState)
        {
            _redo.Clear();
            _undo.Push(CurrentState);
            _currentState = newState;
        }

        public bool Undo()
        {
            if (_undo.Count == 0)
                return false;
            _redo.Push(CurrentState);
            _currentState = _undo.Pop();
            return true;
        }

        public bool Redo()
        {
            if (_redo.Count == 0)
                return false;
            _undo.Push(CurrentState);
            _currentState = _redo.Pop();
            return true;
        }

        public string UndoLine { get { return CurrentState == null ? null : CurrentState.UndoLine; } }
        public string RedoLine { get { return _redo.Count == 0 ? null : _redo.Peek().UndoLine; } }

        public abstract byte[] Css { get; }
        public abstract byte[] Js { get; }
        public abstract string CssJsFilename { get; }
    }
}
