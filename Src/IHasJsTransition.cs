using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Servers;
using RT.TagSoup;
using RT.Util.Consoles;
using RT.Util.Json;
using RT.Util.Serialization;

namespace QuizGameEngine
{
    public interface IHasJsTransition
    {
        string JsMethod { get; }
        object JsParameters { get; }
    }
}
