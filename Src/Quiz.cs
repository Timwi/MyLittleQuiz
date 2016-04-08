
namespace Trophy.MyLittleQuiz
{
    public sealed class Quiz : QuizBase
    {
        public override string CssJsFilename { get { return "MyLittleQuiz"; } }
        public Quiz() { CurrentState = new Setup(); }
    }
}
