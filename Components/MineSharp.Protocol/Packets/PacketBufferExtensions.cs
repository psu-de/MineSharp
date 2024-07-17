﻿using MineSharp.ChatComponent;
using MineSharp.Core;
using MineSharp.Core.Common;
using Newtonsoft.Json.Linq;

namespace MineSharp.Protocol.Packets;

internal static class PacketBufferExtensions
{
    public static Chat ReadChatComponent(this PacketBuffer buffer)
    {
        if (buffer.ProtocolVersion >= ProtocolVersion.V_1_20_3)
            return Chat.Parse(buffer.ReadNbt()!);

        return Chat.Parse(buffer.ReadString());
    }

    public static void WriteChatComponent(this PacketBuffer buffer, Chat chat)
    {
        if (buffer.ProtocolVersion >= ProtocolVersion.V_1_20_3)
            buffer.WriteNbt(chat.ToNbt());
        else buffer.WriteString(chat.ToJson().ToString());
    }
}