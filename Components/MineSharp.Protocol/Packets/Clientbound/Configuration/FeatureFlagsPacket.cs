using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Feature flags packet
///     See https://wiki.vg/Protocol#Feature_Flags
/// </summary>
public class FeatureFlagsPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Configuration_FeatureFlags;
    
    /// <summary>
    ///     The enabled feature flags
    /// </summary>
    public required string[] FeatureFlags { get; init; }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarIntArray(FeatureFlags, (buff, str) => buff.WriteString(str));
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new FeatureFlagsPacket { FeatureFlags = buffer.ReadVarIntArray(buff => buff.ReadString()) };
    }
}
#pragma warning restore CS1591
