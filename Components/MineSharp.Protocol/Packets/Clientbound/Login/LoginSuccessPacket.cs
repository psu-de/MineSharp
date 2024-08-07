using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

/// <summary>
///     Login success packet
/// </summary>
public class LoginSuccessPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
public static PacketType StaticType => PacketType.CB_Login_Success;
    
    /// <summary>
    ///     Uuid
    /// </summary>
    public required Uuid Uuid { get; init; }

    /// <summary>
    ///     Username of the client
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    ///     A list of properties sent for versions &gt;= 1.19
    /// </summary>
    public Property[]? Properties { get; init; } = null;

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

        return new LoginSuccessPacket()
        {
            Uuid = uuid,
            Username = username,
            Properties = properties
        };
    }

    /// <summary>
    ///     A player property
    /// </summary>
    public class Property : ISerializable<Property>
    {
        /// <summary>
        ///     Name of this property
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        ///     Value of this property
        /// </summary>
        public required string Value { get; init; }

        /// <summary>
        ///     Signature
        /// </summary>
        public required string? Signature { get; init; }

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

            return new() { Name = name, Value = value, Signature = signature };
        }
    }
}
