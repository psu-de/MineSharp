using Newtonsoft.Json;

namespace MineSharp.Auth.Json;

#pragma warning disable CS8618
internal class CertificateBlob
{
    [JsonProperty("expiresAt")] public string ExpiresAt;
    [JsonProperty("KeyPair")] public KeyPairBlob KeyPair;

    [JsonProperty("publicKeySignature")] public string PublicKeySignature;

    [JsonProperty("publicKeySignatureV2")] public string PublicKeySignatureV2;

    [JsonProperty("refreshedAfter")] public string RefreshedAfter;
}

internal class KeyPairBlob
{
    [JsonProperty("privateKey")] public string PrivateKey;

    [JsonProperty("publicKey")] public string PublicKey;
}
#pragma warning restore CS8618
