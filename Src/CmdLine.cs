using RT.Util;
using RT.Util.CommandLine;

namespace QuizGameEngine
{
    [CommandLine, DocumentationRhoML("Quiz game engine.")]
    abstract class QuizCmdLine
    {
        [Option("-r"), DocumentationRhoML("Specifies the path containing the resource files. Without this option, the resources embedded in the executable are used.")]
        public string ResourcePath = null;

        [Option("-l"), DocumentationRhoML("Specifies a path and filename to which to write logging messages. Without this option, no logging is performed.")]
        public string LogFile = null;

        private static void PostBuildCheck(IPostBuildReporter rep)
        {
            CommandLineParser.PostBuildStep<QuizCmdLine>(rep, null);
        }
    }

    [CommandName("start"), DocumentationRhoML("Start a new quiz and save the quiz state to a file.")]
    abstract class QuizCmdStart : QuizCmdLine
    {
        [IsMandatory, IsPositional, DocumentationRhoML("Path and filename where to save the new quiz.")]
        public string OutputFile = null;

        public abstract QuizBase StartState { get; }
    }

    [CommandName("load"), DocumentationRhoML("Load a quiz state from a file.")]
    sealed class QuizCmdLoad : QuizCmdLine
    {
        [IsMandatory, IsPositional, DocumentationRhoML("Path and filename to the quiz to load.")]
        public string File = null;
    }
}
