﻿using MineSharp.Core;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Exceptions;

namespace MineSharp.Protocol.Packets.Clientbound.Play;
#pragma warning disable CS1591
public sealed partial record MultiBlockUpdatePacket : IPacketStatic<MultiBlockUpdatePacket>
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_MultiBlockChange;

    // Here is no non-argument constructor allowed
    // Do not use
#pragma warning disable CS8618
    private MultiBlockUpdatePacket()
#pragma warning restore CS8618
    {
    }

    /// <summary>
    ///     Constructor for before 1.20
    /// </summary>
    /// <param name="chunkSection"></param>
    /// <param name="suppressLightUpdates"></param>
    /// <param name="blocks"></param>
    public MultiBlockUpdatePacket(long chunkSection, bool? suppressLightUpdates, long[] blocks)
    {
        ChunkSection = chunkSection;
        SuppressLightUpdates = suppressLightUpdates;
        Blocks = blocks;
    }

    /// <summary>
    ///     Constructor >= 1.20
    /// </summary>
    /// <param name="chunkSection"></param>
    /// <param name="blocks"></param>
    public MultiBlockUpdatePacket(long chunkSection, long[] blocks)
    {
        ChunkSection = chunkSection;
        SuppressLightUpdates = null;
        Blocks = blocks;
    }

    public long ChunkSection { get; init; }
    public bool? SuppressLightUpdates { get; init; }
    public long[] Blocks { get; init; }

    public void Write(PacketBuffer buffer, MinecraftData data)
    {
        if (data.Version.Protocol < ProtocolVersion.V_1_20_0 && SuppressLightUpdates == null)
        {
            throw new MineSharpPacketVersionException(nameof(SuppressLightUpdates), data.Version.Protocol);
        }

        buffer.WriteLong(ChunkSection);
        if (data.Version.Protocol < ProtocolVersion.V_1_20_0)
        {
            buffer.WriteBool(SuppressLightUpdates!.Value);
        }

        buffer.WriteVarIntArray(Blocks, (buf, val) => buf.WriteLong(val));
    }

    public static MultiBlockUpdatePacket Read(PacketBuffer buffer, MinecraftData data)
    {
        var chunkSection = buffer.ReadLong();
        bool? suppressLightUpdates = null;
        if (data.Version.Protocol < ProtocolVersion.V_1_20_0)
        {
            suppressLightUpdates = buffer.ReadBool();
        }

        var blocks = buffer.ReadVarIntArray(buf => buf.ReadVarLong());
        return new MultiBlockUpdatePacket(chunkSection, suppressLightUpdates, blocks);
    }
}
#pragma warning restore CS1591
