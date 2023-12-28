using MineSharp.Auth.Responses;
using MineSharp.Core.Common;
using System.Text;

namespace MineSharp.Auth;

/// <summary>
/// Represents a Minecraft session.
/// </summary>
public class Session
{
    /// <summary>
    /// The Username for this session
    /// </summary>
    public string Username { get; }
    
    /// <summary>
    /// The UUID associated with the minecraft account
    /// </summary>
    public UUID UUID { get; }
    
    /// <summary>
    /// The client token for this session
    /// </summary>
    public string ClientToken { get; }
    
    /// <summary>
    /// The session token for this session
    /// </summary>
    public string SessionToken { get; }
    
    /// <summary>
    /// Whether this session is online (authenticated with minecraft services)
    /// </summary>
    public bool OnlineSession { get; }

    /// <summary>
    /// The PlayerCertificates for the user. Only set when this is an online session
    /// </summary>
    public PlayerCertificate? Certificate { get; set; }

    internal Session(string username, UUID uuid, string clientToken, string sessionToken, bool isOnline, PlayerCertificate? certificate = null)
    {
        this.Username = username;
        this.UUID = uuid;
        this.ClientToken = clientToken;
        this.SessionToken = sessionToken;
        this.OnlineSession = isOnline;
        this.Certificate = certificate;
    }

    /// <summary>
    /// Create a new offline session with the given username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public static Session OfflineSession(string username)
    {
        return new Session(username, Guid.NewGuid(), "", "", false);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Session (Username='{this.Username}', UUID='{this.UUID}', Online={this.OnlineSession})";
    }
}
