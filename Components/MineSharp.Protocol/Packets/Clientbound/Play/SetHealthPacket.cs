﻿using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed partial record SetHealthPacket(float Health, int Food, float Saturation) : IPacketStatic<SetHealthPacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_UpdateHealth;

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        buffer.WriteFloat(Health);
        buffer.WriteVarInt(Food);
        buffer.WriteFloat(Saturation);
    }

    public static SetHealthPacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var health = buffer.ReadFloat();
        var food = buffer.ReadVarInt();
        var saturation = buffer.ReadFloat();
        return new SetHealthPacket(health, food, saturation);
    }
}
#pragma warning restore CS1591
