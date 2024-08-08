using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;
using static MineSharp.Protocol.Packets.Clientbound.Login.LoginSuccessPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

/// <summary>
///     Login success packet
/// </summary>
/// <param name="Uuid">Uuid</param>
/// <param name="Username">Username of the client</param>
/// <param name="Properties">A list of properties sent for versions &gt;= 1.19</param>
public sealed record LoginSuccessPacket(Uuid Uuid, string Username, Property[]? Properties = null) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Login_Success;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteUuid(Uuid);
        buffer.WriteString(Username);

        if (version.Version.Protocol < ProtocolVersion.V_1_19)
        {
            return;
        }

        if (Properties == null)
        {
            throw new MineSharpPacketVersionException(nameof(Properties), version.Version.Protocol);
        }

        buffer.WriteVarIntArray(Properties, (buffer, property) => property.Write(buffer));
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var uuid = buffer.ReadUuid();
        var username = buffer.ReadString();
        Property[]? properties = null;

        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            properties = buffer.ReadVarIntArray(Property.Read);
        }

        return new LoginSuccessPacket(uuid, username, properties);
    }

    /// <summary>
    ///     A player property
    /// </summary>
    public sealed record Property(string Name, string Value, string? Signature) : ISerializable<Property>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteString(Name);
            buffer.WriteString(Value);
            buffer.WriteBool(Signature == null);

            if (Signature != null)
            {
                buffer.WriteString(Signature);
            }
        }

        /// <inheritdoc />
        public static Property Read(PacketBuffer buffer)
        {
            var name = buffer.ReadString();
            var value = buffer.ReadString();
            string? signature = null;

            if (buffer.ReadBool())
            {
                signature = buffer.ReadString();
            }

            return new Property(name, value, signature);
        }
    }
}
