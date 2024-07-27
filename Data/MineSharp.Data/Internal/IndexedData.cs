using MineSharp.Data.Framework.Providers;

namespace MineSharp.Data.Internal;

internal abstract class IndexedData<T>(IDataProvider<T> provider)
{
    private readonly object @lock = new();
    private IDataProvider<T>? provider = provider;
    protected bool Loaded { get; private set; }

    protected abstract void InitializeData(T data);

    protected void Load()
    {
        lock (@lock)
        {
            if (Loaded)
            {
                return;
            }

            InitializeData(provider!.GetData());

            Loaded = true;
            provider = null;
        }
    }
}
