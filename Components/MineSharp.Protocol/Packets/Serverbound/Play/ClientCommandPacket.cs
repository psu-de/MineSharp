using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Represents a client command packet.
/// </summary>
/// <param name="ActionId">The action ID of the client command.</param>
public sealed record ClientCommandPacket(int ActionId) : IPacketStatic<ClientCommandPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_ClientCommand;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(ActionId);
    }

    /// <inheritdoc />
    public static ClientCommandPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var actionId = buffer.ReadVarInt();
        return new ClientCommandPacket(actionId);
    }

    static IPacket IPacketStatic.Read(PacketBuffer buffer, MinecraftData data)
    {
        return Read(buffer, data);
    }
}
