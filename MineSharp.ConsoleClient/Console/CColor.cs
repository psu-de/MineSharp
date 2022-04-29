using PrettyPrompt.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console {
    public static class CColor {

        public static string WorldCommand = "gold1";
        public static string PlayerCommand = "lightsteelblue";
        public static string MiscCommand = "greenyellow";
        public static string ChatCommand = "lightseagreen";
        public static string EntityCommand = "cadetblue";
        public static string PromptCommand = "indianred1";

        public static string Good = "green";
        public static string Warn = "darkorange";
        public static string Error = "red";

        public static FormattedString FromMarkup(string markup) {

            var splits = markup.Split("[/]");

            FormattedString formatted = new FormattedString();

            foreach (var s in splits) {
                int colorFrom = s.IndexOf('[');

                if (colorFrom == -1) {
                    formatted += new FormattedString(s);
                } else {
                    int colorTo = s.IndexOf(']');
                    string color = s.Substring(colorFrom + 1, colorTo - colorFrom - 1);


                    if (colorFrom != 0) {
                        formatted += new FormattedString(s.Substring(0, colorFrom));
                    }


                    string text = s.Substring(colorTo + 1);
                    formatted += new FormattedString(text, new ConsoleFormat(GetAnsiColor(color)));
                }
            }

            return formatted;
        }

        public static AnsiColor GetAnsiColor(string color) {

            var assembly = Assembly.GetAssembly(typeof(Spectre.Console.AnsiConsole));
            if (assembly == null) throw new ArgumentNullException();

            var colorTable = assembly.GetType("Spectre.Console.ColorTable");
            if (colorTable == null) throw new ArgumentNullException();

            var method = colorTable.GetMethods()
                    .Where(x => x.Name == "GetColor" &&
                        x.GetParameters().Length == 1 &&
                        x.GetParameters()[0].ParameterType == typeof(string)).FirstOrDefault();
            if (method == null) throw new ArgumentNullException();

            var spColor = (Spectre.Console.Color?)method.Invoke(null, new object[] { color });
            if (spColor == null) throw new ArgumentNullException();

            AnsiColor aColor = AnsiColor.Rgb(spColor.Value.R, spColor.Value.G, spColor.Value.B);
            return aColor;
        }

        public static AnsiColor GetAnsiColor(Spectre.Console.Color color) {
            return AnsiColor.Rgb(color.R, color.G, color.B);
        }

        }
    }
