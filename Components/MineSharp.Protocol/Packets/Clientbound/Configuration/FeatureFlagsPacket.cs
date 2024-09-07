using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Configuration;

/// <summary>
///     Feature flags packet
///     See https://wiki.vg/Protocol#Feature_Flags
/// </summary>
/// <param name="FeatureFlags">The enabled feature flags</param>
public sealed record FeatureFlagsPacket(Identifier[] FeatureFlags) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Configuration_FeatureFlags;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarIntArray(FeatureFlags, (buff, str) => buff.WriteIdentifier(str));
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new FeatureFlagsPacket(buffer.ReadVarIntArray(buff => buff.ReadIdentifier()));
    }
}
