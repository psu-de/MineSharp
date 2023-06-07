namespace MineSharp.Auth.Responses;

public interface ICachedResponse<out T> where T : class
{
    bool RequiresRefresh();
    void Serialize(string path);
    abstract static T? Deserialize(string path);
}
