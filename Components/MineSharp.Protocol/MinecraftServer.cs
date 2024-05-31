using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MineSharp.Auth;
using MineSharp.Core.Common.Protocol;
using MineSharp.Data;
using NLog;
using Org.BouncyCastle.Crypto.Tls;

namespace MineSharp.Protocol;

/// <summary>
/// Minecraft server class
/// </summary>
public sealed class MinecraftServer
{
    /// <summary>
    /// The latest version supported
    /// </summary>
    public const string LATEST_SUPPORTED_VERSION = "1.20.4";

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly MinecraftData Data;

    private readonly IPAddress _address;
    private readonly ushort _port;
    private TcpListener? _listener;

    internal readonly MinecraftApi? Api;

    
    /// <summary>
    /// The constructor for the MinecraftServer class
    /// </summary>
    public MinecraftServer(
        MinecraftData data,
        IPAddress address,
        ushort port)
    {
        this.Data = data;
        this._address = address;
        this._port = port;
    }

    /// <summary>
    /// Start the Minecraft server
    /// </summary>
    public void Start()
    {
        _listener = new TcpListener(_address, _port);

        _listener.Start();

        Task.Run(ListenForNewClients);
    }

    private void ListenForNewClients()
    {
        while (true)
        {
            using TcpClient client = _listener!.AcceptTcpClient();

            Task.Run(() => HandleNewClient(client));
        }
    }

    private void HandleNewClient(TcpClient client)
    {
        MinecraftClient minecraftClient = new MinecraftClient(client, _address, Data);
        minecraftClient.UpdateGameState(GameState.Handshaking);
    } 
}
