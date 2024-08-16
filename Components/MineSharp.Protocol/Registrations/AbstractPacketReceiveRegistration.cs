using static MineSharp.Protocol.MinecraftClient;

namespace MineSharp.Protocol.Registrations;

/// <summary>
/// Abstract class for packet receive registration.
/// </summary>
public abstract class AbstractPacketReceiveRegistration : AbstractDisposableRegistration
{
    protected readonly MinecraftClient Client;
    protected readonly AsyncPacketHandler Handler;

    protected AbstractPacketReceiveRegistration(MinecraftClient client, AsyncPacketHandler handler)
    {
        Client = client;
        Handler = handler;
    }
}
