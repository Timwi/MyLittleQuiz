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

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    [CommandName("MyLittleQuiz"), DocumentationLiteral("Brony Fair 2016 quiz")]
    public sealed class Cmd : StartQuizCmd
    {
        public override QuizBase StartState { get { return new Quiz(); } }
    }
}
