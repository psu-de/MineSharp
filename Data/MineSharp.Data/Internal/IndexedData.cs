using MineSharp.Data.Framework.Providers;

namespace MineSharp.Data.Internal;

internal abstract class IndexedData<T>(IDataProvider<T> provider)
{
    protected bool              Loaded { get; private set; } = false;
    private   IDataProvider<T>? provider = provider;
    private   object            _lock    = new object();

    protected abstract void InitializeData(T data);

    protected void Load()
    {
        lock (this._lock)
        {
            if (this.Loaded)
                return;
        
            this.InitializeData(this.provider!.GetData());

            this.Loaded   = true;
            this.provider = null;
        }
    }
}
