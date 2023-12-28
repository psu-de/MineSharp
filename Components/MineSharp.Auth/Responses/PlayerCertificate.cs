using MineSharp.Auth.Json;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MineSharp.Auth.Responses;

/// <summary>
/// Player certificates from Mojang.
/// Used to encrypt and decrypt messages.
/// </summary>
public class PlayerCertificate : ICachedResponse<PlayerCertificate>
{
    private const string DATE_FORMAT = "yyyy-MM-ddTHH:mm:ss.ffffffZ";
    
    /// <summary>
    /// The associated KeyPair
    /// </summary>
    public KeyPair Keys { get; }
    
    /// <summary>
    /// The public RSA instance
    /// </summary>
    public RSA RsaPublic { get; }
    
    /// <summary>
    /// The private RSA instance
    /// </summary>
    public RSA RsaPrivate { get; }
    
    /// <summary>
    /// Public key signature
    /// </summary>
    public byte[] PublicKeySignature { get; }
    
    /// <summary>
    /// Public key signature V2
    /// </summary>
    public byte[] PublicKeySignatureV2 { get; }
    
    /// <summary>
    /// When the certificate expires
    /// </summary>
    public DateTime ExpiresAt { get; }
    
    /// <summary>
    /// When the certificate should be refreshed
    /// </summary>
    public DateTime RefreshAfter { get; }

    private PlayerCertificate(KeyPair keys, byte[] publicKeySignature, byte[] publicKeySignatureV2, DateTime expiresAt, DateTime refreshAfter)
    {
        this.Keys = keys;
        this.PublicKeySignature = publicKeySignature;
        this.PublicKeySignatureV2 = publicKeySignatureV2;
        this.ExpiresAt = expiresAt;
        this.RefreshAfter = refreshAfter;

        this.RsaPublic = RSA.Create();
        this.RsaPrivate = RSA.Create();
        
        this.RsaPublic.ImportSubjectPublicKeyInfo(this.Keys.PublicKey, out _);
        this.RsaPrivate.ImportPkcs8PrivateKey(this.Keys.PrivateKey, out _);
    }

    private CertificateBlob ToBlob()
    {
        return new CertificateBlob() {
            KeyPair = new KeyPairBlob() {
                PrivateKey = EncodeKey(this.Keys.PrivateKey, "PRIVATE"),
                PublicKey = EncodeKey(this.Keys.PublicKey, "PUBLIC"),
            },
            PublicKeySignature = Convert.ToBase64String(this.PublicKeySignature),
            PublicKeySignatureV2 = Convert.ToBase64String(this.PublicKeySignatureV2),
            ExpiresAt = this.ExpiresAt.ToString(DATE_FORMAT),
            RefreshedAfter = this.RefreshAfter.ToString(DATE_FORMAT)
        };
    }

    internal static PlayerCertificate FromBlob(CertificateBlob blob)
    {
        return new PlayerCertificate(
            new KeyPair(
                DecodeKey(blob.KeyPair.PrivateKey),
                DecodeKey(blob.KeyPair.PublicKey)),
            Convert.FromBase64String(blob.PublicKeySignature),
            Convert.FromBase64String(blob.PublicKeySignatureV2),
            DateTime.ParseExact(blob.ExpiresAt, DATE_FORMAT, CultureInfo.InvariantCulture).ToUniversalTime(), 
            DateTime.ParseExact(blob.RefreshedAfter, DATE_FORMAT, CultureInfo.InvariantCulture).ToUniversalTime());
    }

    /// <summary>
    /// Whether the certificates should be refreshed
    /// </summary>
    /// <returns></returns>
    public bool RequiresRefresh()
        => DateTime.UtcNow >= this.RefreshAfter;

    /// <summary>
    /// Serialize the certificates to the given path
    /// </summary>
    /// <param name="path"></param>
    public void Serialize(string path)
    {
        var file = Path.Join(path, "certificate.json");
        var json = JsonConvert.SerializeObject(this.ToBlob());
        File.WriteAllText(file, json);
    }
    
    /// <summary>
    /// Deserialize the certificate from the given path
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
        const int lineWidth = 76;
        
        var str = Convert.ToBase64String(key);
        str = Regex.Replace(str, $".{lineWidth}", "$0\n");

        return $"-----BEGIN RSA {keyType} KEY-----\n{str}\n-----END RSA {keyType} KEY-----";
    }

    /// <summary>
    /// A public / private key pair
    /// </summary>
    /// <param name="PrivateKey">The private key</param>
    /// <param name="PublicKey">The public key</param>
    public record KeyPair(byte[] PrivateKey, byte[] PublicKey);
}
