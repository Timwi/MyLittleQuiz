
namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Quiz : QuizBase
    {
        public override string CssJsFilename { get { return "MyLittleQuiz"; } }
        public Quiz() { _currentState = new Setup(); }
    }
}
