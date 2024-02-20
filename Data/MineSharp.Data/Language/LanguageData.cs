using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;
using MineSharp.Data.Internal;

namespace MineSharp.Data.Language;

internal class LanguageData(IDataProvider<LanguageDataBlob> provider) : IndexedData<LanguageDataBlob>(provider), ILanguageData
{
    private Dictionary<string, string> translations = new();
    
    public string? GetTranslation(string name)
    {
        if (!this.Loaded)
            this.Load();

        return this.translations.GetValueOrDefault(name);
    }

    protected override void InitializeData(LanguageDataBlob data)
    {
        this.translations = data.Translations;
    }
}