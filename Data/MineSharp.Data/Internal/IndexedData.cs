using MineSharp.Data.Framework;
using MineSharp.Data.Framework.Providers;

namespace MineSharp.Data.Internal;

internal abstract class IndexedData<T>(IDataProvider<T> provider)
{
    protected bool              Loaded { get; private set; } = false;
    private   IDataProvider<T>? provider = provider;

    protected abstract void InitializeData(T data);

    protected void Load()
    {
        if (this.Loaded)
            return;

        this.InitializeData(this.provider!.GetData());

        this.provider = null;
        this.Loaded   = true;
    }
}
