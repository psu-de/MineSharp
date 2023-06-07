namespace MineSharp.Data.Providers;

public interface IDataProvider
{
    public bool IsLoaded { get; }
    
    public void Load();
}
