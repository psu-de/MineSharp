using MineSharp.Auth.Responses;
using MineSharp.Core.Common;

namespace MineSharp.Auth;

/// <summary>
///     Represents a Minecraft session.
/// </summary>
public class Session
{
    internal Session(string username, Uuid uuid, string clientToken, string sessionToken, bool isOnline,
                     PlayerCertificate? certificate = null)
    {
        Username = username;
        Uuid = uuid;
        ClientToken = clientToken;
        SessionToken = sessionToken;
        OnlineSession = isOnline;
        Certificate = certificate;
    }

    /// <summary>
    ///     The Username for this session
    /// </summary>
    public string Username { get; }

    /// <summary>
    ///     The UUID associated with the minecraft account
    /// </summary>
    public Uuid Uuid { get; }

    /// <summary>
    ///     The client token for this session
    /// </summary>
    public string ClientToken { get; }

    /// <summary>
    ///     The session token for this session
    /// </summary>
    public string SessionToken { get; }

    /// <summary>
    ///     Whether this session is online (authenticated with minecraft services)
    /// </summary>
    public bool OnlineSession { get; }

    /// <summary>
    ///     The PlayerCertificates for the user. Only set when this is an online session
    /// </summary>
    public PlayerCertificate? Certificate { get; set; }

    /// <summary>
    ///     Create a new offline session with the given username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public static Session OfflineSession(string username)
    {
        return new(username, Guid.NewGuid(), "", "", false);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Session (Username='{Username}', UUID='{Uuid}', Online={OnlineSession})";
    }
}
