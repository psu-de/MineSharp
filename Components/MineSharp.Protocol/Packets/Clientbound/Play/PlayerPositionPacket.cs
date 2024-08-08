using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed record PlayerPositionPacket : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Position;

    // Here is no non-argument constructor allowed
    // Do not use
    private PlayerPositionPacket()
    {
    }

    private PlayerPositionPacket(double x, double y, double z, float yaw, float pitch, PositionFlags flags, int teleportId,
                            bool? dismountVehicle)
    {
        X = x;
        Y = y;
        Z = z;
        Yaw = yaw;
        Pitch = pitch;
        Flags = flags;
        TeleportId = teleportId;
        DismountVehicle = dismountVehicle;
    }

    /// <summary>
    ///     Constructor for 1.18.x-1.19.3
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="yaw"></param>
    /// <param name="pitch"></param>
    /// <param name="flags"></param>
    /// <param name="teleportId"></param>
    /// <param name="dismountVehicle"></param>
    public PlayerPositionPacket(double x, double y, double z, float yaw, float pitch, PositionFlags flags, int teleportId,
                                bool dismountVehicle)
        : this(x, y, z, yaw, pitch, flags, teleportId, (bool?)dismountVehicle)
    {
    }

    /// <summary>
    ///     Constructor for >= 1.19.4
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="yaw"></param>
    /// <param name="pitch"></param>
    /// <param name="flags"></param>
    /// <param name="teleportId"></param>
    public PlayerPositionPacket(double x, double y, double z, float yaw, float pitch, PositionFlags flags, int teleportId)
        : this(x, y, z, yaw, pitch, flags, teleportId, null)
    {
    }

    public double X { get; init; }
    public double Y { get; init; }
    public double Z { get; init; }
    public float Yaw { get; init; }
    public float Pitch { get; init; }
    public PositionFlags Flags { get; init; }
    public int TeleportId { get; init; }
    public bool? DismountVehicle { get; init; }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteDouble(X);
        buffer.WriteDouble(Y);
        buffer.WriteDouble(Z);
        buffer.WriteFloat(Yaw);
        buffer.WriteFloat(Pitch);
        buffer.WriteSByte((sbyte)Flags);
        buffer.WriteVarInt(TeleportId);

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_4)
        {
            return;
        }

        if (!DismountVehicle.HasValue)
        {
            throw new MineSharpPacketVersionException(nameof(DismountVehicle), version.Version.Protocol);
        }

        buffer.WriteBool(DismountVehicle.Value);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var x = buffer.ReadDouble();
        var y = buffer.ReadDouble();
        var z = buffer.ReadDouble();
        var yaw = buffer.ReadFloat();
        var pitch = buffer.ReadFloat();
        var flags = (PositionFlags)buffer.ReadSByte();
        var teleportId = buffer.ReadVarInt();

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_4)
        {
            return new PlayerPositionPacket(x, y, z, yaw, pitch, flags, teleportId);
        }

        var dismountVehicle = buffer.ReadBool();
        return new PlayerPositionPacket(x, y, z, yaw, pitch, flags, teleportId, dismountVehicle);
    }

    [Flags]
    public enum PositionFlags : sbyte
    {
        X = 0x01,
        Y = 0x02,
        Z = 0x04,
        YRot = 0x08,
        XRot = 0x10
    }
}
#pragma warning restore CS1591
