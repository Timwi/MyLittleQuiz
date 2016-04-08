using RT.Util.CommandLine;

namespace Trophy.MyLittleQuiz
{
    [CommandName("MyLittleQuiz"), DocumentationLiteral("Brony Fair 2016 quiz")]
    sealed class Cmd : QuizCmdStart
    {
        public override QuizBase StartState { get { return new Quiz(); } }
    }
}
