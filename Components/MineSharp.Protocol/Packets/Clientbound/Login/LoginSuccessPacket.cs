using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Login;
#pragma warning disable CS1591
/// <summary>
///     Login success packet
/// </summary>
public class LoginSuccessPacket : IPacket
{
    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="username"></param>
    /// <param name="properties"></param>
    public LoginSuccessPacket(Uuid uuid, string username, Property[]? properties = null)
    {
        Uuid = uuid;
        Username = username;
        Properties = properties;
    }

    /// <summary>
    ///     Uuid
    /// </summary>
    public Uuid Uuid { get; set; }

    /// <summary>
    ///     Username of the client
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    ///     A list of properties sent for versions &gt;= 1.19
    /// </summary>
    public Property[]? Properties { get; set; }

    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Login_Success;

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
    public class Property : ISerializable<Property>
    {
        /// <summary>
        ///     Create a new instance
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="signature"></param>
        public Property(string name, string value, string? signature)
        {
            Name = name;
            Value = value;
            Signature = signature;
        }

        /// <summary>
        ///     Name of this property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Value of this property
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Signature
        /// </summary>
        public string? Signature { get; set; }

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

            return new(name, value, signature);
        }
    }
}
#pragma warning restore CS1591
