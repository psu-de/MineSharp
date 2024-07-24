using System.Text;
using MineSharp.Core.Common.Protocol;
using MineSharp.Protocol.Packets;
using NLog;

namespace MineSharp.Protocol.Channels;

/// <summary>
/// Implements minecraft's plugin channel system
/// </summary>
public class PluginChannels
{
    private readonly MinecraftClient client;
    private readonly Dictionary<string, Channel> channels;
    
    internal PluginChannels(MinecraftClient client)
    {
        this.client   = client;
        this.channels = new()
        {
            { "minecraft:register", new("minecraft:register", this) },
            { "minecraft:unregister", new("minecraft:unregister", this) }
        };
    }

    /// <summary>
    /// Register a channel
    /// </summary>
    /// <param name="channelIdentifier"></param>
    public Channel Register(string channelIdentifier)
    {
        if (channels.TryGetValue(channelIdentifier, out var channel))
        {
            return channel;
        }

        channel = new(channelIdentifier, this);
        channels.Add(channelIdentifier, channel);
        
        this["minecraft:register"].Send(Encoding.UTF8.GetBytes(channelIdentifier));
        
        return channel;
    }

    /// <summary>
    /// Unregister a channel
    /// </summary>
    /// <param name="channelIdentifier"></param>
    public void Unregister(string channelIdentifier)
    {
        channels.Remove(channelIdentifier);
        
        this["minecraft:unregister"].Send(Encoding.UTF8.GetBytes(channelIdentifier));
    }

    internal void Handle(string identifier, byte[] data)
    {
        if (!channels.TryGetValue(identifier, out var channel))
        {
            Logger.Debug($"Received data for unknown channel {identifier}");
            return;
        }
        
        Task.Run(async () => await channel.Handle(data, client.Data));
    }

    internal Task Send(Channel channel, byte[] data, CancellationToken token = default)
    {
        IPacket packet = client.GameState switch
        {
            GameState.Configuration => new Packets.Serverbound.Configuration.PluginMessagePacket
            {
                Data = data, ChannelName = channel.Identifier
            },
            GameState.Play => new Packets.Serverbound.Play.PluginMessagePacket
            {
                Data = data, ChannelName = channel.Identifier
            },
            GameState.Login => throw new NotImplementedException(),
            _ => throw new InvalidOperationException($"you cannot send plugin messages during {client.GameState} phase")
        };

        return client.SendPacket(packet, token);
    }

    /// <summary>
    /// Access a channel by name
    /// </summary>
    /// <param name="identifier"></param>
    public Channel this[string identifier]
    {
        get => channels[identifier];
    }

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
}
