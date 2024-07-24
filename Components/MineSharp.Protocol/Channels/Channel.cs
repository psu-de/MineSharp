using MineSharp.Core.Common;
using MineSharp.Data;
using NLog;

namespace MineSharp.Protocol.Channels;

/// <summary>
/// Represents a minecraft plugin channel.
/// See https://wiki.vg/Plugin_channels
/// </summary>
public class Channel
{
    /// <summary>
    /// The identifier of this channel
    /// </summary>
    public readonly string Identifier;
    
    private readonly PluginChannels            parent;
    private readonly List<AsyncChannelHandler> handlers;
    
    internal Channel(string identifier, PluginChannels parent)
    {
        Identifier  = identifier;
        handlers    = new();
        this.parent = parent;
    }

    internal async Task Handle(byte[] data, MinecraftData minecraftData)
    {
        if (handlers.Count == 0)
        {
            return;
        }

        var tasks = new List<Task>(handlers.Count);
        foreach (var handler in handlers)
        {
            var buffer = new PacketBuffer(data, minecraftData.Version.Protocol);
            tasks.Add(handler(buffer));
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception)
        {
            foreach (var exception in tasks.Where(x => x.Exception != null))
            {
                Logger.Warn($"Error in custom packet handling: {exception.Exception}");
            }
        }
    }

    /// <summary>
    /// Send data through this channel
    /// </summary>
    public void Send(byte[] data)
    {
        this.parent.Send(this, data);
    }

    /// <summary>
    /// Subscribe to this channel
    /// </summary>
    public void On(AsyncChannelHandler handler)
    {
        if (handlers.Count > 0)
        {
            Logger.Warn(
                $"The channel {Identifier} has already been subscribed to. This is supported but may lead to unwanted side-effects.");
        }
        
        handlers.Add(handler);
    }
    
    /// <summary>
    /// Unsubscribe the handler from this channel 
    /// </summary>
    /// <param name="handler"></param>
    public void Remove(AsyncChannelHandler handler)
    {
        handlers.Remove(handler);
    }

    /// <summary>
    /// Subscribe to the channel.
    /// Equivalent to <see cref="Channel.On"/>
    /// </summary>
    /// <param name="channel">The channel to subscribe to</param>
    /// <param name="handler">The handler</param>
    public static Channel operator +(Channel channel, AsyncChannelHandler handler)
    {
        channel.On(handler);
        return channel;
    }

    /// <summary>
    /// Unsubscribe from the channel.
    /// </summary>
    /// <param name="channel">The channel to unsubscribe</param>
    /// <param name="handler">The handler to be removed</param>
    /// <returns></returns>
    public static Channel operator -(Channel channel, AsyncChannelHandler handler)
    {
        channel.handlers.Remove(handler);
        return channel;
    }

    /// <summary>
    /// A delegate for handling channel data
    /// </summary>
    public delegate Task AsyncChannelHandler(PacketBuffer buffer);
    
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
}
