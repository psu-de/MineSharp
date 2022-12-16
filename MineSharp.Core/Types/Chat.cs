using MineSharp.Core.Logging;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MineSharp.Core.Types
{
    public class Chat
    {
        private static readonly Logger Logger = Logger.GetLogger();
        
        public string JSON { get; }

        public string StyledMessage { get; private set; }
        public string Message { get; private set; }

        private readonly Func<string, string>? _translationProvider; 
        
        public Chat(string json, Func<string, string>? translationProvider = null)
        {
            this.JSON = json;
            this._translationProvider = translationProvider;
            
            this.StyledMessage = this.ParseComponent(JToken.Parse(this.JSON));
            this.Message = Regex.Replace(this.StyledMessage, "\\$[0-9a-fk-r]", "");
        }

        private string ParseComponent(JToken token, string styleCode = "")
        {
            return token.Type switch {
                JTokenType.Array => ParseArray((JArray)token, styleCode),
                JTokenType.Object => ParseObject((JObject)token, styleCode),
                JTokenType.String => (string)token!,
                JTokenType.Integer => (string)token!,
                _ => throw new Exception($"Type {token.Type} is not supported")
            };
        }

        private string ParseObject(JObject jObject, string styleCode = "")
        {
            var sb = new StringBuilder();
            
            var colorProp = jObject.GetValue("color");
            if (colorProp != null)
            {
                var color = ParseComponent(colorProp);
                var style = TextStyle.GetTextStyle(color);
                if (style != null)
                {
                    styleCode = style.ToString();
                } else
                {
                    Logger.Warning($"Unknown chat style: {color}");
                    styleCode = "";
                }
            }

            var extraProp = jObject.GetValue("extra");
            if (extraProp != null)
            {
                var extras = (JArray)extraProp!;

                foreach (var item in extras)
                    sb.Append(this.ParseComponent(item, styleCode) + "§r");
            }

            var textProp = jObject.GetValue("text");
            var translateProp = jObject.GetValue("translate");

            if (textProp != null)
            {
                return styleCode + ParseComponent(textProp, styleCode) + sb.ToString();
            } else if (translateProp != null)
            {
                var usingData = new List<string>();

                var usingProp = jObject.GetValue("using");
                var withProp = jObject.GetValue("with");
                if (usingProp != null && withProp == null)
                    withProp = usingProp;

                if (withProp != null)
                {
                    var array = (JArray)withProp;
                    for (int i = 0; i < array.Count; i++)
                    {
                        usingData.Add(this.ParseComponent(array[i], styleCode));
                    }
                }
                
                var ruleName = this.ParseComponent(translateProp!);
                if (this._translationProvider == null)
                {
                    Logger.Warning("No translation provider given. Consider passing MineSharp.Data.Languages.Language.GetRule(string name) as an argument.");
                    return styleCode + string.Join(" ", usingData) + sb.ToString();
                }
                return styleCode + TranslateString(this._translationProvider(ruleName), usingData)
                                 + sb.ToString();
            } else return sb.ToString();
        }

        private string ParseArray(JArray jArray, string styleCode = "")
        {
            var sb = new StringBuilder();
            foreach (var token in jArray)
            {
                sb.Append(ParseComponent(token, styleCode));
            }
            return sb.ToString();
        }
        
        private static string TranslateString(string rule, List<string> usings)
        {
            int usingIndex = 0;
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < rule.Length; i++)
            {
                if (rule[i] == '%' && i + 1 < rule.Length)
                {
                    //Using string or int with %s or %d
                    if (rule[i + 1] == 's' || rule[i + 1] == 'd')
                    {
                        if (usings.Count > usingIndex)
                        {
                            result.Append(usings[usingIndex]);
                            usingIndex++;
                            i += 1;

                            continue;
                        }
                    }
                    
                    else if (char.IsDigit(rule[i + 1]) && i + 3 < rule.Length && rule[i + 2] == '$'
                             && (rule[i + 3] == 's' || rule[i + 3] == 'd'))
                    {
                        int specifiedIdx = rule[i + 1] - '1';

                        if (usings.Count > specifiedIdx)
                        {
                            result.Append(usings[specifiedIdx]);
                            usingIndex++;
                            i += 3;

                            continue;
                        }
                    }
                }

                result.Append(rule[i]);
            }

            return result.ToString();
        }

        public override string ToString() => this.JSON;
    }

    public class TextStyle
    {
        public static TextStyle[] KnownStyles = new[] {
            new TextStyle('0', "black"),
            new TextStyle('1', "dark_blue"),
            new TextStyle('2', "dark_green"),
            new TextStyle('4', "dark_red"),
            new TextStyle('3', "dark_aqua") { Aliases = new[] { "dark_cyan" } },
            new TextStyle('5', "dark_purple") { Aliases = new[] { "dark_magenta" } },
            new TextStyle('6', "gold") { Aliases = new[] { "dark_yellow" } },
            new TextStyle('7', "gray"),
            new TextStyle('8', "dark_gray"),
            new TextStyle('9', "blue"),
            new TextStyle('a', "green"),
            new TextStyle('b', "aqua") { Aliases = new[] { "cyan" } },
            new TextStyle('c', "red"),
            new TextStyle('d', "light_purple") { Aliases = new[] {"magenta" } },
            new TextStyle('e', "yellow"),
            new TextStyle('f', "white"),
            
            new TextStyle('k', "magic"),
            new TextStyle('l', "bold"),
            new TextStyle('m', "strikethrough"),
            new TextStyle('n', "underline"),
            new TextStyle('o', "italic"),
            new TextStyle('r', "reset"),
        };

        private const char PREFIX = '$';

        public char Code { get; private set; }
        public string Name { get; private set; }
        public string[] Aliases { get; set; } = Array.Empty<string>();

        public TextStyle(char code, string name)
        {
            this.Code = code;
            this.Name = name;
        }

        public override string ToString() => $"{PREFIX}{this.Code}";

        public static TextStyle? GetTextStyle(string name)
        {
            return KnownStyles.FirstOrDefault(x =>
                    string.Equals(name, x.Name, StringComparison.OrdinalIgnoreCase) ||
                    x.Aliases.Any(y => string.Equals(name, y, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
