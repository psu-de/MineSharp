using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;
#pragma warning disable CS1591
/// <summary>
/// Feature flags packet
/// </summary>
public class FeatureFlagsPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_FeatureFlags;

    /// <summary>
    /// The enabled feature flags
    /// </summary>
    public string[] FeatureFlags { get; set; }

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="featureFlags"></param>
    public FeatureFlagsPacket(string[] featureFlags)
    {
        this.FeatureFlags = featureFlags;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarIntArray(this.FeatureFlags, (buff, str) => buff.WriteString(str));
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new FeatureFlagsPacket(
            buffer.ReadVarIntArray(buff => buff.ReadString()));
    }
}
#pragma warning restore CS1591
