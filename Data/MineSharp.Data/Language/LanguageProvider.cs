namespace MineSharp.Data.Language;

/// <summary>
/// Provides translation strings from minecraft.
/// </summary>
public class LanguageProvider
{

    private readonly LanguageVersion _version;
    
    internal LanguageProvider(LanguageVersion version)
    {
        this._version = version;
    }

    /// <summary>
    /// Get a translation by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetTranslation(string name)
        => this._version.Translations[name];
}