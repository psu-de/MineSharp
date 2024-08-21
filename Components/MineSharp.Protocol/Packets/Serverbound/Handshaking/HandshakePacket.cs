using MineSharp.Core;
using MineSharp.Core.Common.Protocol;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Handshaking;

#pragma warning disable CS1591
/// <summary>
///     Packet for setting the protocol during handshake
/// </summary>
/// <param name="ProtocolVersion">The protocol version</param>
/// <param name="Host">The host address</param>
/// <param name="Port">The port number</param>
/// <param name="NextState">The next game state</param>
public sealed partial record HandshakePacket(ProtocolVersion ProtocolVersion, string Host, ushort Port, GameState NextState) : IPacketStatic<HandshakePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Handshake_SetProtocol;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt((int)ProtocolVersion);
        buffer.WriteString(Host);
        buffer.WriteUShort(Port);
        buffer.WriteVarInt((int)NextState);
    }

    /// <inheritdoc />
    public static HandshakePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var protocolVersion = (ProtocolVersion)buffer.ReadVarInt();
        var host = buffer.ReadString();
        var port = buffer.ReadUShort();
        var nextState = (GameState)buffer.ReadVarInt();

        return new HandshakePacket(protocolVersion, host, port, nextState);
    }
}
#pragma warning restore CS1591
