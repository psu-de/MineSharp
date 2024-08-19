using MineSharp.Core.Geometry;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Serverbound.Play.CommandBlockUpdatePacket;

namespace MineSharp.Protocol.Packets.Serverbound.Play;

/// <summary>
///     Sent by the client when the player updated a command block.
/// </summary>
/// <param name="Location">The position of the command block.</param>
/// <param name="Command">The command to be executed by the command block.</param>
/// <param name="Mode">The mode of the command block.</param>
/// <param name="Flags">The flags for the command block.</param>
public sealed record CommandBlockUpdatePacket(Position Location, string Command, CommandBlockMode Mode, CommandBlockFlags Flags) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.SB_Play_UpdateCommandBlock;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WritePosition(Location);
        buffer.WriteString(Command);
        buffer.WriteVarInt((int)Mode);
        buffer.WriteSByte((sbyte)Flags);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var location = buffer.ReadPosition();
        var command = buffer.ReadString();
        var mode = (CommandBlockMode)buffer.ReadVarInt();
        var flags = (CommandBlockFlags)buffer.ReadSByte();

        return new CommandBlockUpdatePacket(location, command, mode, flags);
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public enum CommandBlockMode
    {
        Sequence,
        Auto,
        Redstone
    }

    [Flags]
    public enum CommandBlockFlags : sbyte
    {
        /// <summary>
        /// If not set, the output of the previous command will not be stored within the command block.
        /// </summary>
        TrackOutput = 0x01,
        Conditional = 0x02,
        Automatic = 0x04
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
