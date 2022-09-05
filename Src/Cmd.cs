using RT.CommandLine;

namespace Trophy.MyLittleQuiz
{
    [CommandName("MyLittleQuiz"), DocumentationLiteral("Brony Fair 2016 quiz")]
    sealed class Cmd : QuizCmdStart
    {
        [IsPositional, DocumentationLiteral("Specifies the graphics package. Default is Brony.")]
        public GraphicsPackage GraphicsPackage = GraphicsPackage.Brony;

        public override QuizBase StartState { get { return new Quiz(GraphicsPackage); } }
    }
}
