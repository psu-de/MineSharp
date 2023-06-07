using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Login;

public class LoginSuccessPacket : IPacket
{
    public static int Id => 0x02;

    public UUID Uuid { get; set; }
    public string Username { get; set; }
    public Property[]? Properties { get; set; }

    public LoginSuccessPacket(UUID uuid, string username, Property[]? properties = null)
    {
        this.Uuid = uuid;
        this.Username = username;
        this.Properties = properties;
    }

    public void Write(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        buffer.WriteUuid(this.Uuid);
        buffer.WriteString(this.Username);

        if (version.Protocol.Version < ProtocolVersion.V_1_19)
        {
            return;
        }
        
        if (this.Properties == null)
        {
            throw new PacketVersionException("Login Success packets expect to have properties set after version 1.19");
        }
            
        buffer.WriteVarIntArray(this.Properties, ((buffer, property) => property.Write(buffer)));
    }
    
    public static IPacket Read(PacketBuffer buffer, MinecraftData version, string packetName)
    {
        var uuid = buffer.ReadUuid();
        var username = buffer.ReadString();
        Property[]? properties = null;
        
        if (version.Protocol.Version >= ProtocolVersion.V_1_19)
        {
            properties = buffer.ReadVarIntArray<Property>(Property.Read);
        }

        return new LoginSuccessPacket(uuid, username, properties);
    }

    public class Property : ISerializable<Property>
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string? Signature { get; set; }

        public Property(string name, string value, string? signature)
        {
            this.Name = name;
            this.Value = value;
            this.Signature = signature;
        }

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

        public static Property Read(PacketBuffer buffer)
        {
            string name = buffer.ReadString();
            string value = buffer.ReadString();
            string? signature = null;
            
            if (buffer.ReadBool())
            {
                signature = buffer.ReadString();
            }
            
            return new Property(name, value, signature);
        }
    }
}
