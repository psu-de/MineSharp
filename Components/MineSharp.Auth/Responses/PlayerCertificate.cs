using System.Security.Cryptography;
using System.Text.RegularExpressions;
using MineSharp.Auth.Json;
using Newtonsoft.Json;

namespace MineSharp.Auth.Responses;

/// <summary>
///     Player certificates from Mojang.
///     Used to encrypt and decrypt messages.
/// </summary>
public class PlayerCertificate : ICachedResponse<PlayerCertificate>
{
    private const string DateFormat = "yyyy-MM-ddTHH:mm:ss.ffffffZ";

    private PlayerCertificate(KeyPair keys, byte[] publicKeySignature, byte[] publicKeySignatureV2, DateTime expiresAt,
                              DateTime refreshAfter)
    {
        Keys = keys;
        PublicKeySignature = publicKeySignature;
        PublicKeySignatureV2 = publicKeySignatureV2;
        ExpiresAt = expiresAt;
        RefreshAfter = refreshAfter;

        RsaPublic = RSA.Create();
        RsaPrivate = RSA.Create();

        RsaPublic.ImportSubjectPublicKeyInfo(Keys.PublicKey, out _);
        RsaPrivate.ImportPkcs8PrivateKey(Keys.PrivateKey, out _);
    }

    /// <summary>
    ///     The associated KeyPair
    /// </summary>
    public KeyPair Keys { get; }

    /// <summary>
    ///     The public RSA instance
    /// </summary>
    public RSA RsaPublic { get; }

    /// <summary>
    ///     The private RSA instance
    /// </summary>
    public RSA RsaPrivate { get; }

    /// <summary>
    ///     Public key signature
    /// </summary>
    public byte[] PublicKeySignature { get; }

    /// <summary>
    ///     Public key signature V2
    /// </summary>
    public byte[] PublicKeySignatureV2 { get; }

    /// <summary>
    ///     When the certificate expires
    /// </summary>
    public DateTime ExpiresAt { get; }

    /// <summary>
    ///     When the certificate should be refreshed
    /// </summary>
    public DateTime RefreshAfter { get; }

    /// <summary>
    ///     Whether the certificates should be refreshed
    /// </summary>
    /// <returns></returns>
    public bool RequiresRefresh()
    {
        return DateTime.UtcNow >= RefreshAfter;
    }

    /// <summary>
    ///     Serialize the certificates to the given path
    /// </summary>
    /// <param name="path"></param>
    public void Serialize(string path)
    {
        var file = Path.Join(path, "certificate.json");
        var json = JsonConvert.SerializeObject(ToBlob());
        File.WriteAllText(file, json);
    }

    /// <summary>
    ///     Deserialize the certificate from the given path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static PlayerCertificate? Deserialize(string path)
    {
        var file = Path.Join(path, "certificate.json");

        if (!File.Exists(file))
        {
            return null;
        }

        var blob = JsonConvert.DeserializeObject<CertificateBlob>(
            File.ReadAllText(file))!;
        return FromBlob(blob);
    }

    private CertificateBlob ToBlob()
    {
        return new()
        {
            KeyPair =
                new()
                {
                    PrivateKey = EncodeKey(Keys.PrivateKey, "PRIVATE"),
                    PublicKey = EncodeKey(Keys.PublicKey, "PUBLIC")
                },
            PublicKeySignature = Convert.ToBase64String(PublicKeySignature),
            PublicKeySignatureV2 = Convert.ToBase64String(PublicKeySignatureV2),
            ExpiresAt = ExpiresAt.ToString(DateFormat),
            RefreshedAfter = RefreshAfter.ToString(DateFormat)
        };
    }

    internal static PlayerCertificate FromBlob(CertificateBlob blob)
    {
        return new(
            new(
                DecodeKey(blob.KeyPair.PrivateKey),
                DecodeKey(blob.KeyPair.PublicKey)),
            Convert.FromBase64String(blob.PublicKeySignature),
            Convert.FromBase64String(blob.PublicKeySignatureV2),
            DateTime.Parse(blob.ExpiresAt).ToUniversalTime(),
            DateTime.Parse(blob.RefreshedAfter).ToUniversalTime());
    }

    private static byte[] DecodeKey(string key)
    {
        var rx = new Regex("-+[^-]+-+");
        var der = rx.Replace(key, "")
                    .Replace("\r", "")
                    .Replace("\n", "");

        return Convert.FromBase64String(der);
    }

    private static string EncodeKey(byte[] key, string keyType)
    {
        const int line_width = 76;

        var str = Convert.ToBase64String(key);
        str = Regex.Replace(str, $".{line_width}", "$0\n");

        return $"-----BEGIN RSA {keyType} KEY-----\n{str}\n-----END RSA {keyType} KEY-----";
    }

    /// <summary>
    ///     A public / private key pair
    /// </summary>
    /// <param name="PrivateKey">The private key</param>
    /// <param name="PublicKey">The public key</param>
    public record KeyPair(byte[] PrivateKey, byte[] PublicKey);
}
