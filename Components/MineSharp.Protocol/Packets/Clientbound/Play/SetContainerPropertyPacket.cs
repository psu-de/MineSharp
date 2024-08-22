using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Set Container Property packet
/// </summary>
/// <param name="WindowId">The window ID</param>
/// <param name="Property">The property to be updated</param>
/// <param name="Value">The new value for the property</param>
public sealed record SetContainerPropertyPacket(byte WindowId, short Property, short Value) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_CraftProgressBar;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteByte(WindowId);
        buffer.WriteShort(Property);
        buffer.WriteShort(Value);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var windowId = buffer.ReadByte();
        var property = buffer.ReadShort();
        var value = buffer.ReadShort();

        return new SetContainerPropertyPacket(windowId, property, value);
    }

    // TODO: Add all the meanings of the properties
    // depends on the type of container but we only get the window ID here
    // so we need static methods that have the container type as a parameter
    // https://wiki.vg/index.php?title=Protocol&oldid=19208#Set_Container_Content
}
