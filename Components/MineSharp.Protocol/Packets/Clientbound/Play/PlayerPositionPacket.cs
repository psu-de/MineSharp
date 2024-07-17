using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public class PlayerPositionPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_Position;

    public double X               { get; set; }
    public double Y               { get; set; }
    public double Z               { get; set; }
    public float  Yaw             { get; set; }
    public float  Pitch           { get; set; }
    public sbyte  Flags           { get; set; }
    public int    TeleportId      { get; set; }
    public bool?  DismountVehicle { get; set; }

    /// <summary>
    /// Constructor for 1.18.x-1.19.3
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="yaw"></param>
    /// <param name="pitch"></param>
    /// <param name="flags"></param>
    /// <param name="teleportId"></param>
    /// <param name="dismountVehicle"></param>
    public PlayerPositionPacket(double x, double y, double z, float yaw, float pitch, sbyte flags, int teleportId, bool dismountVehicle)
    {
        this.X               = x;
        this.Y               = y;
        this.Z               = z;
        this.Yaw             = yaw;
        this.Pitch           = pitch;
        this.Flags           = flags;
        this.TeleportId      = teleportId;
        this.DismountVehicle = dismountVehicle;
    }

    /// <summary>
    /// Constructor for >= 1.19.4
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="yaw"></param>
    /// <param name="pitch"></param>
    /// <param name="flags"></param>
    /// <param name="teleportId"></param>
    public PlayerPositionPacket(double x, double y, double z, float yaw, float pitch, sbyte flags, int teleportId)
    {
        this.X               = x;
        this.Y               = y;
        this.Z               = z;
        this.Yaw             = yaw;
        this.Pitch           = pitch;
        this.Flags           = flags;
        this.TeleportId      = teleportId;
        this.DismountVehicle = null;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteDouble(this.X);
        buffer.WriteDouble(this.Y);
        buffer.WriteDouble(this.Z);
        buffer.WriteFloat(this.Yaw);
        buffer.WriteFloat(this.Pitch);
        buffer.WriteSByte(this.Flags);
        buffer.WriteVarInt(this.TeleportId);

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_4)
            return;

        if (!this.DismountVehicle.HasValue)
        {
            throw new MineSharpPacketVersionException(nameof(this.DismountVehicle), version.Version.Protocol);
        }

        buffer.WriteBool(this.DismountVehicle.Value);
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var x          = buffer.ReadDouble();
        var y          = buffer.ReadDouble();
        var z          = buffer.ReadDouble();
        var yaw        = buffer.ReadFloat();
        var pitch      = buffer.ReadFloat();
        var flags      = buffer.ReadSByte();
        var teleportId = buffer.ReadVarInt();

        if (version.Version.Protocol >= ProtocolVersion.V_1_19_4)
            return new PlayerPositionPacket(x, y, z, yaw, pitch, flags, teleportId);

        var dismountVehicle = buffer.ReadBool();
        return new PlayerPositionPacket(x, y, z, yaw, pitch, flags, teleportId, dismountVehicle);
    }
}
#pragma warning restore CS1591
