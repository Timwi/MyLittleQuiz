using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.CommandLine;

namespace QuizGameEngine
{
    [DocumentationRhoML("Quiz engine.")]
    sealed class CommandLine
    {
        [Option("-r"), DocumentationRhoML("Specifies the path containing the resource files. Without this option, the resources embedded in the executable are used.")]
        public string ResourcePath = null;

        [IsMandatory, IsPositional]
        public QuizCmd QuizCmd = null;

        private static void PostBuildCheck(IPostBuildReporter rep)
        {
            CommandLineParser.PostBuildStep<CommandLine>(rep, null);
        }
    }

    [CommandGroup]
    abstract class QuizCmd { }
    [CommandName("start"), DocumentationRhoML("Start a new quiz and save the quiz state to a file.")]
    sealed class QuizCmdStart : QuizCmd
    {
        [IsMandatory, IsPositional, DocumentationRhoML("Path and filename where to save the new quiz.")]
        public string OutputFile = null;
        [IsMandatory, IsPositional, DocumentationRhoML("Specifies the type of quiz to start.")]
        public StartQuizCmd Quiz = null;
    }
    [CommandName("load"), DocumentationRhoML("Load a quiz state from a file.")]
    sealed class QuizCmdLoad : QuizCmd
    {
        [IsMandatory, IsPositional, DocumentationRhoML("Path and filename to the quiz to load.")]
        public string File = null;
    }
}
