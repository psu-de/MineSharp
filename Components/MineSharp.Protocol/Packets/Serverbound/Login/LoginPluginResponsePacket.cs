using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Login;

/// <summary>
///     Login plugin response packet
/// </summary>
/// <param name="MessageId">The message ID</param>
/// <param name="Data">The data</param>
public sealed record LoginPluginResponsePacket(int MessageId, byte[]? Data) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Login_LoginPluginResponse;

    /// <inheritdoc />
    public bool Successful => Data != null;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteBool(Data != null);

        if (Data != null)
        {
            buffer.WriteBytes(Data);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var messageId = buffer.ReadVarInt();
        var hasData = buffer.ReadBool();
        var data = hasData switch
        {
            true => buffer.RestBuffer(),
            false => null
        };

        return new LoginPluginResponsePacket(messageId, data);
    }
}

