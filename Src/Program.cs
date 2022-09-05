using System;
using System.Reflection;
using RT.PostBuild;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy.MyLittleQuiz
{
    static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "--post-build-check")
                return PostBuildChecker.RunPostBuildChecks(args[1], Assembly.GetExecutingAssembly());

            ConsoleUtil.WriteLine("This program cannot be run by itself. Install this as a plugin for Trophy and then run Trophy.".Color(ConsoleColor.Magenta));
            return 1;
        }
    }
}
