namespace MineSharp.Data.Language;

public class LanguageProvider
{

    private readonly LanguageVersion _version;
    
    internal LanguageProvider(LanguageVersion version)
    {
        this._version = version;
    }

    public string GetTranslation(string name)
        => this._version.Translations[name];
}