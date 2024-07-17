using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Login;
#pragma warning disable CS1591
/// <summary>
/// Login success packet
/// </summary>
public class LoginSuccessPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => PacketType.CB_Login_Success;

    /// <summary>
    /// Uuid
    /// </summary>
    public UUID Uuid { get; set; }

    /// <summary>
    /// Username of the client
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// A list of properties sent for versions &gt;= 1.19
    /// </summary>
    public Property[]? Properties { get; set; }

    /// <summary>
    /// Create a new instance
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="username"></param>
    /// <param name="properties"></param>
    public LoginSuccessPacket(UUID uuid, string username, Property[]? properties = null)
    {
        this.Uuid       = uuid;
        this.Username   = username;
        this.Properties = properties;
    }

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteUuid(this.Uuid);
        buffer.WriteString(this.Username);

        if (version.Version.Protocol < ProtocolVersion.V_1_19)
        {
            return;
        }

        if (this.Properties == null)
        {
            throw new MineSharpPacketVersionException(nameof(Properties), version.Version.Protocol);
        }

        buffer.WriteVarIntArray(this.Properties, ((buffer, property) => property.Write(buffer)));
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var         uuid       = buffer.ReadUuid();
        var         username   = buffer.ReadString();
        Property[]? properties = null;

        if (version.Version.Protocol >= ProtocolVersion.V_1_19)
        {
            properties = buffer.ReadVarIntArray(Property.Read);
        }

        return new LoginSuccessPacket(uuid, username, properties);
    }

    /// <summary>
    /// A player property
    /// </summary>
    public class Property : ISerializable<Property>
    {
        /// <summary>
        /// Name of this property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of this property
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Signature
        /// </summary>
        public string? Signature { get; set; }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="signature"></param>
        public Property(string name, string value, string? signature)
        {
            this.Name      = name;
            this.Value     = value;
            this.Signature = signature;
        }

        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteString(this.Name);
            buffer.WriteString(this.Value);
            buffer.WriteBool(Signature == null);

            if (Signature != null)
            {
                buffer.WriteString(this.Signature);
            }
        }

        /// <inheritdoc />
        public static Property Read(PacketBuffer buffer)
        {
            string  name      = buffer.ReadString();
            string  value     = buffer.ReadString();
            string? signature = null;

            if (buffer.ReadBool())
            {
                signature = buffer.ReadString();
            }

            return new Property(name, value, signature);
        }
    }
}
#pragma warning restore CS1591
