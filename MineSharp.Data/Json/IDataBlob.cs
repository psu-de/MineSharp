namespace MineSharp.Data.Json;

public interface IDataBlob<out T>
{
    public int Id { get; }
    public string Name { get; }
    
    public T ToElement();
}
