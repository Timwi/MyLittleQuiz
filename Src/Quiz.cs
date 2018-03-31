using System.IO;

namespace Trophy.MyLittleQuiz
{
    public sealed class Quiz : QuizBase
    {
        public override string CssJsFilename { get { return "MyLittleQuiz"; } }
        public string GraphicsPackage = "Brony";
        public override string MoreCss { get { return Path.Combine(GraphicsPackage, GraphicsPackage + ".css"); } }
        public Quiz(string graphicsPackage)
        {
            GraphicsPackage = graphicsPackage;
            CurrentState = new Setup(graphicsPackage);
        }
        private Quiz() { }  // for Classify
    }
}
