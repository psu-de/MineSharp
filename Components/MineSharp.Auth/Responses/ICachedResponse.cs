namespace MineSharp.Auth.Responses;

internal interface ICachedResponse<out T> where T : class
{
    bool RequiresRefresh();
    void Serialize(string path);
    static abstract T? Deserialize(string path);
}
