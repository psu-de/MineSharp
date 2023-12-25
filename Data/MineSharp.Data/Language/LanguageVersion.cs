namespace MineSharp.Data.Language;

internal abstract class LanguageVersion
{
    public abstract Dictionary<string, string> Translations { get; }
}