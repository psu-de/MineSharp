using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MineSharp.Data;

namespace MineSharp.Chat;

/*
 * Thanks to Minecraft-Console-Client
 * https://github.com/MCCTeam/Minecraft-Console-Client
 * 
 * This Class uses a lot of code from Protocol/Message/ChatParser.cs from MCC.
 */

/// <summary>
/// Represents a Chat Message object
/// </summary>
public class ChatComponent
{
    /// <summary>
    /// The raw Json message
    /// </summary>
    public string Json { get; }
    
    /// <summary>
    /// The message without any styling
    /// </summary>
    public string Message { get; private set; }
    
    /// <summary>
    /// The styled message containing style codes
    /// </summary>
    public string StyledMessage { get; private set; }
    

    private readonly MinecraftData data;
    
    /// <summary>
    /// Create a new ChatComponent
    /// </summary>
    /// <param name="json"></param>
    /// <param name="data"></param>
    public ChatComponent(string json, MinecraftData data)
    {
        this.Json = json;
        this.data = data;
        
        this.StyledMessage = this.ParseComponent(JToken.Parse(this.Json));
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
            styleCode = style != null 
                ? style.ToString() 
                : string.Empty;
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
        }

        if (translateProp == null) 
            return sb.ToString();
        
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
            
        var ruleName = this.ParseComponent(translateProp);
        return styleCode + TranslateString(ruleName, usingData)
                         + sb.ToString();

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
    
    private string TranslateString(string ruleName, List<string> usings)
    {
        var rule = this.data.Language.GetTranslation(ruleName);

        var usingIndex = 0;
        string result = Regex.Replace(rule, "%s", match => usings[usingIndex++]);
        result = Regex.Replace(result, "%(\\d)\\$s", match =>
        {
            var idx = match.Groups[1].Value[0] - '1';
            return usings[idx];
        });
        return result;
    }

    /// <inheritdoc />
    public override string ToString() => this.Json;
}