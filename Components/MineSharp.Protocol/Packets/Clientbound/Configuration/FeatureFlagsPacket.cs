using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;
#pragma warning disable CS1591
/// <summary>
///     Feature flags packet
/// </summary>
public class FeatureFlagsPacket : IPacket
{
    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="featureFlags"></param>
    public FeatureFlagsPacket(string[] featureFlags)
    {
        FeatureFlags = featureFlags;
    }

    /// <summary>
    ///     The enabled feature flags
    /// </summary>
    public string[] FeatureFlags { get; set; }

    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Configuration_FeatureFlags;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarIntArray(FeatureFlags, (buff, str) => buff.WriteString(str));
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new FeatureFlagsPacket(
            buffer.ReadVarIntArray(buff => buff.ReadString()));
    }
}
#pragma warning restore CS1591
