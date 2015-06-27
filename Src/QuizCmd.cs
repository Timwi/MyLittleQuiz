using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util.CommandLine;

namespace QuizGameEngine
{
    [CommandGroup]
    public abstract class StartQuizCmd
    {
        public abstract QuizStateBase StartState { get; }
    }
}
