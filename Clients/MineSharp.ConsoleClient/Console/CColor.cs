using PrettyPrompt.Highlighting;
using Spectre.Console;
using System.Reflection;

namespace MineSharp.ConsoleClient.Console
{
    public static class CColor
    {

        public static string WorldCommand = "gold1";
        public static string PlayerCommand = "lightsteelblue";
        public static string MiscCommand = "greenyellow";
        public static string ChatCommand = "lightseagreen";
        public static string EntityCommand = "cadetblue";
        public static string PromptCommand = "indianred1";
        public static string WindowsCommand = "hotpink3_1";

        public static string Good = "green";
        public static string Warn = "darkorange";
        public static string Error = "red";

        public static FormattedString FromMarkup(string markup)
        {

            var splits = markup.Split("[/]");

            var formatted = new FormattedString();

            foreach (var s in splits)
            {
                var colorFrom = s.IndexOf('[');

                if (colorFrom == -1)
                {
                    formatted += new FormattedString(s);
                } else
                {
                    var colorTo = s.IndexOf(']');
                    var color = s.Substring(colorFrom + 1, colorTo - colorFrom - 1);


                    if (colorFrom != 0)
                    {
                        formatted += new FormattedString(s.Substring(0, colorFrom));
                    }


                    var text = s.Substring(colorTo + 1);
                    formatted += new FormattedString(text, new ConsoleFormat(GetAnsiColor(color)));
                }
            }

            return formatted;
        }

        public static AnsiColor GetAnsiColor(string color)
        {

            var assembly = Assembly.GetAssembly(typeof(AnsiConsole));
            if (assembly == null) throw new ArgumentNullException();

            var colorTable = assembly.GetType("Spectre.Console.ColorTable");
            if (colorTable == null) throw new ArgumentNullException();

            var method = colorTable.GetMethods()
                .Where(x => x.Name == "GetColor" &&
                            x.GetParameters().Length == 1 &&
                            x.GetParameters()[0].ParameterType == typeof(string)).FirstOrDefault();
            if (method == null) throw new ArgumentNullException();

            var spColor = (Color?)method.Invoke(null, new object[] {
                color
            });
            if (spColor == null) throw new ArgumentNullException();

            var aColor = AnsiColor.Rgb(spColor.Value.R, spColor.Value.G, spColor.Value.B);
            return aColor;
        }

        public static AnsiColor GetAnsiColor(Color color) => AnsiColor.Rgb(color.R, color.G, color.B);

    }
}
