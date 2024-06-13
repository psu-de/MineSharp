using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

#pragma warning disable CS1591
public class SetPassengersPacket : IPacket
{
    public PacketType Type => PacketType.CB_Play_SetPassengers;

    public int EntityId { get; set; }

    public int[] PassengersId { get; set; }

    public SetPassengersPacket(int entityId, int[] passengersId)
    {
        EntityId = entityId;
        PassengersId = passengersId;
    }

    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(this.EntityId);
        buffer.WriteVarIntArray(this.PassengersId, (buf, elem) => buffer.WriteVarInt(elem));
    }

    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        return new SetPassengersPacket(
            buffer.ReadVarInt(),
            buffer.ReadVarIntArray<int>(buf => buf.ReadVarInt()));
    }
}
#pragma warning restore CS1591
