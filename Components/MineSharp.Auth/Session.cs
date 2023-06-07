using MineSharp.Auth.Responses;
using MineSharp.Core.Common;
using System.Text;

namespace MineSharp.Auth;

public class Session
{
    public string Username { get; }
    public UUID UUID { get; }
    public string ClientToken { get; }
    public string SessionToken { get; }
    public bool OnlineSession { get; }

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
    /// Login via Microsoft to obtain a valid Minecraft online session.
    /// </summary>
    /// <param name="username">The username is only used for cache purposes and must not be the real username.</param>
    /// <returns>A valid Session</returns>
    public static Task<Session> Login(string username)
    {
        return MicrosoftAuth.MicrosoftLogin(username);
    }

    public static Session OfflineSession(string username)
    {
        return new Session(username, UUID.NewUuid(), "", "", false);
    }

    public override string ToString()
    {
        return $"Session (Username='{this.Username}', UUID='{this.UUID}', Online={this.OnlineSession})";
    }
}
