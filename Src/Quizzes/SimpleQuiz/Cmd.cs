using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util.CommandLine;
using RT.Util.Consoles;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.SimpleQuiz
{
    [CommandName("SimpleQuiz"), DocumentationLiteral("Simple quiz for testing.")]
    public sealed class Cmd : StartQuizCmd, ICommandLineValidatable
    {
        [IsPositional, IsMandatory, DocumentationRhoML("Specifies the path and filename to a file containing questions and answers, formatted as a JSON list containing two-element lists, each containing a question and its answer.")]
        public string QAFilePath;

        [Ignore]
        public Tuple<string, string>[] Questions;

        public override QuizBase StartState { get { return new Quiz(Questions); } }

        public ConsoleColoredString Validate()
        {
            if (!File.Exists(QAFilePath))
                return "The specified file, {0/Magenta}, does not exist.".Color(null).Fmt(QAFilePath);

            try
            {
                Questions = ClassifyJson.DeserializeFile<Tuple<string, string>[]>(QAFilePath);
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return null;
        }
    }
}
