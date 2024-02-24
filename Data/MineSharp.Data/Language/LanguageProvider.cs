using MineSharp.Data.Framework.Providers;
using Newtonsoft.Json.Linq;

namespace MineSharp.Data.Language;

internal class LanguageProvider : IDataProvider<LanguageDataBlob>
{
    private JObject token;

    public LanguageProvider(JToken token)
    {
        if (token.Type != JTokenType.Object)
        {
            throw new ArgumentException("Expected token to be an object");
        }

        this.token = (JObject)token;
    }

    public LanguageDataBlob GetData()
    {
        return new LanguageDataBlob(token.ToObject<Dictionary<string, string>>()!);
    }
}
