using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Serverbound.Login;

/// <summary>
///     Login plugin response packet
/// </summary>
/// <param name="MessageId">The message ID</param>
/// <param name="Data">The data</param>
public sealed partial record LoginPluginResponsePacket(int MessageId, byte[]? Data) : IPacketStatic<LoginPluginResponsePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Login_LoginPluginResponse;

    /// <inheritdoc />
    public bool Successful => Data != null;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteVarInt(MessageId);
        buffer.WriteBool(Data != null);

        if (Data != null)
        {
            buffer.WriteBytes(Data);
        }
    }

    /// <inheritdoc />
    public static LoginPluginResponsePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var messageId = buffer.ReadVarInt();
        var hasData = buffer.ReadBool();
        var pluginData = hasData switch
        {
            true => buffer.RestBuffer(),
            false => null
        };

        return new LoginPluginResponsePacket(messageId, pluginData);
    }
}

