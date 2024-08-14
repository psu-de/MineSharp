using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Configuration;
#pragma warning disable CS1591
/// <summary>
///     Resource pack response packet
/// </summary>
/// <param name="Result">The result of the resource pack response</param>
public sealed record ResourcePackResponsePacket(ResourcePackResponsePacket.ResourcePackResult Result) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Configuration_ResourcePackReceive;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt((int)Result);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var result = (ResourcePackResult)buffer.ReadVarInt();
        return new ResourcePackResponsePacket(result);
    }

    /// <summary>
    ///     Enum representing the possible results of a resource pack response
    /// </summary>
    public enum ResourcePackResult
    {
        Success = 0,
        Declined = 1,
        FailedDownload = 2,
        Accepted = 3,
        Downloaded = 4,
        InvalidUrl = 5,
        FailedToReload = 6,
        Discarded = 7
    }
}

#pragma warning restore CS1591
