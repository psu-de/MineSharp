using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Items;
using MineSharp.Data.Windows;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.Windows;
using MineSharp.Windows.Clicks;
using NLog;
using System.Collections.Concurrent;
using MineSharp.Windows.Specific;

namespace MineSharp.Bot.Plugins;

/// <summary>
/// The Window plugin takes care of minecraft window's system.
/// It handles the Bot's Inventory, window slot updates and provides
/// methods to open blocks like chests, crafting tables, ...
/// </summary>
public class WindowPlugin : Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// The window currently opened by the bot. Null if none is open.
    /// </summary>
    public Window? CurrentlyOpenedWindow { get; private set; }
    
    /// <summary>
    /// The bots inventory window
    /// </summary>
    public Inventory? Inventory { get; private set; }
    
    /// <summary>
    /// The item the bot is currently holding
    /// </summary>
    public Item? HeldItem => this.Inventory!
        .GetSlot((short)(PlayerWindowSlots.HotbarStart + this.SelectedHotbarIndex))
        .Item;

    /// <summary>
    /// Index of the selected hot bar slot
    /// </summary>
    public byte SelectedHotbarIndex { get; private set; }
    
    /// <summary>
    /// Fires whenever a window is opened (fe: Chest opened)
    /// </summary>
    public event Events.WindowEvent? OnWindowOpened;
    /// <summary>
    /// Fires whenever the bots held item changed.
    /// </summary>
    public event Events.ItemEvent? OnHeldItemChanged;
    
    private readonly TaskCompletionSource _inventoryLoadedTsc;
    private readonly IDictionary<int, Window> _openWindows;
    private readonly object _windowLock;
    private readonly Window _mainInventory;

    private PlayerPlugin? _playerPlugin;
    private DateTime? _cacheTimestamp;
    private WindowItemsPacket? _cachedWindowItemsPacket;

    /// <summary>
    /// Create a new WindowPlugin instance
    /// </summary>
    /// <param name="bot"></param>
    public WindowPlugin(MineSharpBot bot) : base(bot)
    {
        this._inventoryLoadedTsc = new TaskCompletionSource();
        this._openWindows = new ConcurrentDictionary<int, Window>();
        this._windowLock = new object();
        
        this._mainInventory = new Window(
            255,
            "MainInventory",
            4 * 9, 
            null, 
            this._synchronizeWindowClick);
        this._mainInventory.OnSlotChanged += this.MainInventory_SlotUpdated;
    }
    
    /// <inheritdoc />
    protected override Task Init()
    {
        this._playerPlugin = this.Bot.GetPlugin<PlayerPlugin>();   
        
        this.Bot.Client.On<WindowItemsPacket>(this.HandleWindowItems);
        this.Bot.Client.On<WindowSetSlotPacket>(this.HandleSetSlot);
        this.Bot.Client.On<SetHeldItemPacket>(this.HandleHeldItemChange);

        this.CreateInventory();

        return base.Init();
    }

    /// <summary>
    /// Wait until the bot's inventory items are loaded
    /// </summary>
    /// <returns></returns>
    public Task WaitForInventory() 
        => this._inventoryLoadedTsc.Task;
    
    /// <summary>
    /// Try to open the given block and return the opened window
    /// </summary>
    /// <param name="block"></param>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Window> OpenContainer(Block block, int timeoutMs = 10 * 1000)
    {
        if (!this.Bot.Data.Windows.AllowedBlocksToOpen.Contains(block.Info.Type))
        {
            throw new ArgumentException("Cannot open block of type " + block.Info.Name);
        }

        PlaceBlockPacket packet = new PlaceBlockPacket(
            (int)PlayerHand.MainHand, 
            block.Position,
            BlockFace.Top,
            0.5f,
            0.5f,
            0.5f, 
            false,
            ++this.Bot.SequenceId);
        
        _ = this.Bot.Client.SendPacket(packet);
        _ = this._playerPlugin?.SwingArm();
        var receive = this.Bot.Client.WaitForPacket<OpenWindowPacket>();

        var cancellation = new CancellationTokenSource();
        cancellation.CancelAfter(timeoutMs);
        await receive.WaitAsync(cancellation.Token);

        var result = await receive;

        var windowInfo = this.Bot.Data.Windows.Windows[result.InventoryType];
        var window = this.OpenWindow(result.WindowId, windowInfo);
        this.CurrentlyOpenedWindow = window;
        
        return window;
    }

    /// <summary>
    /// Close the window with the given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task CloseWindow(int id)
    {
        if (id == 0)
            return Task.CompletedTask;
        
        if (!this._openWindows.Remove(id, out var window))
        {
            Logger.Warn("Tried to close window which is not open!");
            return Task.CompletedTask;
        }
        
        if (this.CurrentlyOpenedWindow?.WindowId == window.WindowId)
        {
            this.CurrentlyOpenedWindow = null;
        }
        
        // TODO: window.Close();
        return this.Bot.Client.SendPacket(new Protocol.Packets.Serverbound.Play.CloseWindowPacket((byte)id));
    }

    /// <summary>
    /// Close the given window
    /// </summary>
    /// <param name="window"></param>
    /// <returns></returns>
    public Task CloseWindow(Window window)
        => this.CloseWindow(window.WindowId);

    /// <summary>
    /// Set the selected hot bar slot
    /// </summary>
    /// <param name="hotbarIndex"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task SelectHotbarIndex(byte hotbarIndex)
    {
        if (hotbarIndex > 8) 
            throw new ArgumentOutOfRangeException(nameof(hotbarIndex) + " must be between 0 and 8");

        var packet = new SetHeldItemPacket(hotbarIndex);
        await this.Bot.Client.SendPacket(packet);

        this.SelectedHotbarIndex = hotbarIndex;
        this.OnHeldItemChanged?.Invoke(this.Bot, this.HeldItem);
    }

    /// <summary>
    /// Use the item the bot is currently holding in <paramref name="hand"/>
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    public Task UseItem(PlayerHand hand = PlayerHand.MainHand)
    {
        var packet = new UseItemPacket(hand, this.Bot.SequenceId++);

        return this.Bot.Client.SendPacket(packet);
    }

    private void CreateInventory()
    {
        var inventory = new Inventory(this._mainInventory, this._synchronizeWindowClick);

        lock (this._windowLock)
        {
            this._openWindows.TryAdd(inventory.WindowId, inventory);
        }

        this.Inventory = inventory;
        this.OnWindowOpened?.Invoke(this.Bot, inventory);
    }

    private Window OpenWindow(int id, WindowInfo windowInfo)
    {
        Logger.Debug("Opening window with id=" + id);

        if (this._openWindows.ContainsKey(id))
        {
            throw new ArgumentException("Window with id " + id + " already opened");
        }

        var window = new Window(
            (byte)id, 
            windowInfo.Title, 
            windowInfo.UniqueSlots, 
            windowInfo.ExcludeInventory ? null : this._mainInventory, 
            this._synchronizeWindowClick);
        
        lock(this._windowLock) 
        {
            if (!this._openWindows.TryAdd(id, window))
                Logger.Warn($"Could not add window with id {id}, it already existed.");
        }

        if (this._cachedWindowItemsPacket == null)
            return window;

        if (this._cachedWindowItemsPacket.WindowId == id 
            && DateTime.Now - this._cacheTimestamp! <= TimeSpan.FromSeconds(5))
        {
            // use cache
            Logger.Debug("Applying cached window items for window with id=" + id);
            this.HandleWindowItems(this._cachedWindowItemsPacket);
        }

        // delete cache
        this._cachedWindowItemsPacket = null;
        this._cacheTimestamp = null;
        
        this.OnWindowOpened?.Invoke(this.Bot, window);
        return window;
    }

    private async Task _synchronizeWindowClick(Window window, WindowClick click)
    {
        var windowClickPacket = new WindowClickPacket(
            window.WindowId, 
            window.StateId,
            click.Slot,
            (sbyte)click.Button,
            (sbyte)click.ClickMode,
            click.GetChangedSlots(), 
            window.GetSelectedSlot().Item);
        await this.Bot.Client.SendPacket(windowClickPacket);
        
        // limit the amount of clicks you can do per second
        // if the limit is too low, the client and server will
        // get desynced, and things like crafting won't work 
        // correctly. A value between 30-50ms seems to work,
        // but depends on the speed of the server and internet 
        // connection, I guess.
        await Task.Delay(40);
    }

    private void MainInventory_SlotUpdated(Window window, short index)
    {
        if (index == 3 * 9 + 1 + this.SelectedHotbarIndex)
        {
            this.OnHeldItemChanged?.Invoke(this.Bot, window.GetSlot(index).Item);
        }
    }
    
    private Task HandleSetSlot(WindowSetSlotPacket packet)
    {
        Window? window;
        if (packet.WindowId == -1)
        {
            window = this.CurrentlyOpenedWindow ?? this.Inventory;
        }
        else if (!this._openWindows.TryGetValue(packet.WindowId, out window))
        {
            Logger.Warn($"Received {nameof(WindowSetSlotPacket)} for windowId={packet.WindowId}, but its not opened");
            return Task.CompletedTask;
        }

        if (window == null)
        {
            Logger.Warn($"Received {nameof(WindowSetSlotPacket)} for windowId={packet.WindowId}, but its not opened, {this.CurrentlyOpenedWindow?.ToString() ?? "null"}, {this.Inventory?.ToString() ?? "null"}");
            return Task.CompletedTask;
        }
        
        Logger.Debug("Handle set slot: {Slot}", packet.Slot);
        
        window.StateId = packet.StateId;
        window.SetSlot(packet.Slot);

        return Task.CompletedTask;
    }
    
    private Task HandleWindowItems(WindowItemsPacket packet)
    {
        Window? window;
        lock (this._windowLock)
        {
            if (!this._openWindows.TryGetValue(packet.WindowId, out window))
            {
                Logger.Warn($"Received {packet.GetType().Name} for windowId={packet.WindowId}, but its not opened");
                // Cache items in case it gets opened in a bit
                this._cachedWindowItemsPacket = packet;
                this._cacheTimestamp = DateTime.Now;

                return Task.CompletedTask;
            }
            Logger.Debug($"HandleWindowItems for window {window.Title}");
        }
        
        var slots = packet.Items
            .Select((x, i) => new Slot(x, (short)i))
            .ToArray();
        window.StateId = packet.StateId;
        window.SetSlots(slots);

        if (window.WindowId == 0 && !this._inventoryLoadedTsc.Task.IsCompleted)
        {
            this._inventoryLoadedTsc.SetResult();
        }

        return Task.CompletedTask;
    }

    private Task HandleHeldItemChange(SetHeldItemPacket packet)
    {
        this.SelectedHotbarIndex = (byte)packet.Slot;
        this.OnHeldItemChanged?.Invoke(this.Bot, this.HeldItem);
        return Task.CompletedTask;
    }
}
