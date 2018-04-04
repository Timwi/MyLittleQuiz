using System.IO;
using RT.Util.Json;

namespace Trophy.MyLittleQuiz
{
    public sealed class Quiz : QuizBase
    {
        public override string CssJsFilename { get { return "MyLittleQuiz"; } }
        public GraphicsPackage GraphicsPackage = GraphicsPackage.Brony;
        public override string MoreCss { get { return Path.Combine(GraphicsPackage.ToString(), GraphicsPackage.ToString() + ".css"); } }
        public Quiz(GraphicsPackage graphicsPackage)
        {
            GraphicsPackage = graphicsPackage;
            CurrentState = new Setup();
        }
        private Quiz() { }  // for Classify
        public override object ExtraData
        {
            get
            {
                return new
                {
                    GraphicsPackage = GraphicsPackage,

                    PreloadableImages =
                        GraphicsPackage == GraphicsPackage.Brony ? new[] { "BonBon.svg", "Derpy.png", "Horseshoe.svg", "Logo.png", "Lyra.png", "Minuette.png", "Muffin1_sm.png", "Muffin2_sm.png", "Muffin3_sm.png", "Octavia.png", "Roseluck.png", "Trophy.png" } :
                        GraphicsPackage == GraphicsPackage.Esperanto ? new[] { "R1bg.svg", "R1a.svg", "R1b.svg", "R1c.svg", "R2a.svg", "R2b.svg", "R3a.svg", "R3b.svg", "R4.svg", "Logo.png", "Trophy.png" } :
                        new string[0],

                    PreloadableJingles = new[] { "Round1CorrectAnswer", "Round1WrongAnswer", "Present", "Tada", "Swoosh", "PresentSet", "Round3CorrectAnswer", "Round3WrongAnswer", "Round1Start", "Round2Start", "Round3Start", "Round4Start", "WinnerAndOutro" },

                    Round1Title =
                        GraphicsPackage == GraphicsPackage.Brony ? "Round 1" :
                        GraphicsPackage == GraphicsPackage.Esperanto ? "Rondo 1" : "",

                    Round2Title =
                        GraphicsPackage == GraphicsPackage.Brony ? "Round 2" :
                        GraphicsPackage == GraphicsPackage.Esperanto ? "Rondo 2" : "",

                    Round2Name =
                        GraphicsPackage == GraphicsPackage.Brony ? "Categories" :
                        GraphicsPackage == GraphicsPackage.Esperanto ? "Kategorioj" : "",

                    Round3Title =
                        GraphicsPackage == GraphicsPackage.Brony ? "Round 3" :
                        GraphicsPackage == GraphicsPackage.Esperanto ? "Rondo 3" : "",

                    Round4Title =
                        GraphicsPackage == GraphicsPackage.Brony ? "Round 4" :
                        GraphicsPackage == GraphicsPackage.Esperanto ? "Rondo 4" : "",

                    Round4Name =
                        GraphicsPackage == GraphicsPackage.Brony ? "Sudden Death" :
                        GraphicsPackage == GraphicsPackage.Esperanto ? "Tuja Elĵeto" : "",
                };
            }
        }
    }
}
