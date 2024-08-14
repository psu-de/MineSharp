using System.Collections.Concurrent;
using MineSharp.Bot.Exceptions;
using MineSharp.Bot.Windows;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Events;
using MineSharp.Core.Geometry;
using MineSharp.Data.Windows;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.Windows;
using MineSharp.Windows.Clicks;
using MineSharp.Windows.Specific;
using NLog;
using CBHeldItemPacket = MineSharp.Protocol.Packets.Clientbound.Play.SetHeldItemPacket;
using CloseWindowPacket = MineSharp.Protocol.Packets.Serverbound.Play.CloseWindowPacket;
using SBHeldItemPacket = MineSharp.Protocol.Packets.Serverbound.Play.SetHeldItemPacket;

namespace MineSharp.Bot.Plugins;

/// <summary>
///     The Window plugin takes care of Minecraft window's system.
///     It handles the Bot's Inventory, window slot updates and provides
///     methods to open blocks like chests, crafting tables, ...
/// </summary>
public class WindowPlugin : Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly TaskCompletionSource inventoryLoadedTsc;
    private readonly Window mainInventory;
    private readonly IDictionary<int, Window> openWindows;
    private readonly object windowLock;
    private WindowItemsPacket? cachedWindowItemsPacket;
    private DateTime? cacheTimestamp;
    private TaskCompletionSource<Window>? openContainerTsc;

    private PlayerPlugin? playerPlugin;

    /// <summary>
    ///     Create a new WindowPlugin instance
    /// </summary>
    /// <param name="bot"></param>
    public WindowPlugin(MineSharpBot bot) : base(bot)
    {
        inventoryLoadedTsc = new();
        openWindows = new ConcurrentDictionary<int, Window>();
        windowLock = new();

        mainInventory = new(
            255,
            "MainInventory",
            4 * 9,
            null,
            _synchronizeWindowClick);
        mainInventory.OnSlotChanged += MainInventory_SlotUpdated;

        CreativeInventory = new(bot);

        // OnPacketAfterInitialization is required to ensure that the plugin is initialized
        // before handling packets. Otherwise we have race conditions that might cause errors
        OnPacketAfterInitialization<WindowItemsPacket>(HandleWindowItems, true);
        OnPacketAfterInitialization<WindowSetSlotPacket>(HandleSetSlot, true);
        OnPacketAfterInitialization<CBHeldItemPacket>(HandleHeldItemChange, true);
        OnPacketAfterInitialization<OpenWindowPacket>(HandleOpenWindow, true);
    }

    /// <summary>
    ///     The window currently opened by the bot. Null if none is open.
    /// </summary>
    public Window? CurrentlyOpenedWindow { get; private set; }

    /// <summary>
    ///     The bots inventory window
    /// </summary>
    public Inventory? Inventory { get; private set; }

    /// <summary>
    ///     Creative inventory
    /// </summary>
    public CreativeInventory CreativeInventory { get; private set; }

    /// <summary>
    ///     The item the bot is currently holding
    /// </summary>
    public Item? HeldItem => Inventory!
                            .GetSlot((short)(PlayerWindowSlots.HotbarStart + SelectedHotbarIndex))
                            .Item;

    /// <summary>
    ///     Index of the selected hot bar slot
    /// </summary>
    public byte SelectedHotbarIndex { get; private set; }

    /// <summary>
    ///     Fires whenever a window is opened (fe: Chest opened)
    /// </summary>
    public AsyncEvent<MineSharpBot, Window> OnWindowOpened = new();

    /// <summary>
    ///     Fires whenever the bots held item changed.
    /// </summary>
    public AsyncEvent<MineSharpBot, Item?> OnHeldItemChanged = new();

    /// <inheritdoc />
    protected override Task Init()
    {
        playerPlugin = Bot.GetPlugin<PlayerPlugin>();

        CreateInventory();
        return base.Init();
    }

    /// <summary>
    ///     Wait until the bot's inventory items are loaded
    /// </summary>
    /// <returns></returns>
    public Task WaitForInventory()
    {
        return inventoryLoadedTsc.Task;
    }

    /// <summary>
    ///     Try to open the given block and return the opened window
    /// </summary>
    /// <param name="block"></param>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Window> OpenContainer(Block block, int timeoutMs = 10 * 1000)
    {
        if (!Bot.Data.Windows.AllowedBlocksToOpen.Contains(block.Info.Type))
        {
            throw new ArgumentException("Cannot open block of type " + block.Info.Name);
        }

        openContainerTsc = new();

        var packet = new PlaceBlockPacket(
            (int)PlayerHand.MainHand,
            block.Position,
            BlockFace.Top,
            0.5f,
            0.5f,
            0.5f,
            false,
            ++Bot.SequenceId);

        _ = Bot.Client.SendPacket(packet);
        _ = playerPlugin?.SwingArm();

        var result = await openContainerTsc.Task;

        openContainerTsc = null;
        return result;
    }

    /// <summary>
    ///     Close the window with the given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task CloseWindow(int id)
    {
        if (id == 0)
        {
            return Task.CompletedTask;
        }

        if (!openWindows.Remove(id, out var window))
        {
            Logger.Warn("Tried to close window which is not open!");
            return Task.CompletedTask;
        }

        if (CurrentlyOpenedWindow?.WindowId == window.WindowId)
        {
            CurrentlyOpenedWindow = null;
        }

        // TODO: window.Close();
        return Bot.Client.SendPacket(new CloseWindowPacket((byte)id));
    }

    /// <summary>
    ///     Close the given window
    /// </summary>
    /// <param name="window"></param>
    /// <returns></returns>
    public Task CloseWindow(Window window)
    {
        return CloseWindow(window.WindowId);
    }

    /// <summary>
    ///     Set the selected hot bar slot
    /// </summary>
    /// <param name="hotbarIndex"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task SelectHotbarIndex(byte hotbarIndex)
    {
        if (hotbarIndex > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(hotbarIndex) + " must be between 0 and 8");
        }

        var packet = new SBHeldItemPacket((sbyte)hotbarIndex);
        await Bot.Client.SendPacket(packet);

        SelectedHotbarIndex = hotbarIndex;

        _ = OnHeldItemChanged.Dispatch(Bot, HeldItem);
    }

    /// <summary>
    ///     Use the item the bot is currently holding in <paramref name="hand" />
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    public Task UseItem(PlayerHand hand = PlayerHand.MainHand)
    {
        var packet = new UseItemPacket(hand, Bot.SequenceId++);

        return Bot.Client.SendPacket(packet);
    }

    /// <summary>
    ///     Search inventory for item of type <paramref name="type" />, and put it
    ///     in the bots <paramref name="hand" />.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hand"></param>
    /// <returns></returns>
    public async Task EquipItem(ItemType type, PlayerHand hand = PlayerHand.MainHand)
    {
        await WaitForInventory();

        var handSlot = hand == PlayerHand.MainHand
            ? Inventory!.GetSlot((short)(PlayerWindowSlots.HotbarStart + SelectedHotbarIndex))
            : Inventory!.GetSlot(Inventory.Slot.OffHand);

        _EquipItem(type, handSlot);
    }

    /// <summary>
    ///     Search inventory for item of type <paramref name="type" />, and put it
    ///     in the specified equipment slot.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="equipSlot"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task EquipItem(ItemType type, Inventory.Slot equipSlot)
    {
        if (equipSlot == Inventory.Slot.CraftingResult)
        {
            throw new InvalidOperationException("cannot put something in CraftingResult slot");
        }

        await WaitForInventory();

        var slot = Inventory!.GetSlot(equipSlot);

        _EquipItem(type, slot);
    }

    private void _EquipItem(ItemType type, Slot destination)
    {
        if (destination.Item?.Info.Type == type)
        {
            return;
        }

        var slot = Inventory?.FindInventoryItems(type).FirstOrDefault();

        if (slot == null)
        {
            throw new ItemNotFoundException(type);
        }

        var wasEmpty = destination.IsEmpty();

        Inventory!.DoSimpleClick(WindowMouseButton.MouseLeft, slot.SlotIndex);
        Inventory!.DoSimpleClick(WindowMouseButton.MouseLeft, destination.SlotIndex);

        if (!wasEmpty)
        {
            Inventory.DoSimpleClick(WindowMouseButton.MouseLeft, slot.SlotIndex);
        }
    }

    private void CreateInventory()
    {
        var inventory = new Inventory(mainInventory, _synchronizeWindowClick);

        lock (windowLock)
        {
            openWindows.TryAdd(inventory.WindowId, inventory);
        }

        Inventory = inventory;
        OnWindowOpened.Dispatch(Bot, inventory);
    }

    private Window OpenWindow(int id, WindowInfo windowInfo)
    {
        Logger.Debug("Opening window with id={WindowId}", id);

        if (openWindows.ContainsKey(id))
        {
            throw new ArgumentException($"Window with id {id} already opened");
        }

        var window = new Window(
            (byte)id,
            windowInfo.Title,
            windowInfo.UniqueSlots,
            windowInfo.ExcludeInventory ? null : mainInventory,
            _synchronizeWindowClick);

        lock (windowLock)
        {
            if (!openWindows.TryAdd(id, window))
            {
                Logger.Warn("Could not add window with id {WindowId}, it already existed.", id);
            }
        }

        if (cachedWindowItemsPacket == null)
        {
            return window;
        }

        if (cachedWindowItemsPacket.WindowId == id
            && DateTime.Now - cacheTimestamp! <= TimeSpan.FromSeconds(5))
        {
            // use cache
            Logger.Debug("Applying cached window items for window with id={WindowId}", id);
            HandleWindowItems(cachedWindowItemsPacket);
        }

        // delete cache
        cachedWindowItemsPacket = null;
        cacheTimestamp = null;

        OnWindowOpened.Dispatch(Bot, window);
        openContainerTsc?.TrySetResult(window);

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
        await Bot.Client.SendPacket(windowClickPacket);

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
        if (index == (3 * 9) + 1 + SelectedHotbarIndex)
        {
            OnHeldItemChanged.Dispatch(Bot, window.GetSlot(index).Item);
        }
    }

    private Task HandleSetSlot(WindowSetSlotPacket packet)
    {
        Window? window;
        if (packet.WindowId == -1)
        {
            window = CurrentlyOpenedWindow ?? Inventory;
        }
        else if (!openWindows.TryGetValue(packet.WindowId, out window))
        {
            Logger.Warn("Received {PacketType} for windowId={WindowId}, but it's not opened", nameof(WindowSetSlotPacket), packet.WindowId);
            return Task.CompletedTask;
        }

        if (window == null)
        {
            Logger.Warn("Received {PacketType} for windowId={WindowId}, but it's not opened, {CurrentlyOpenedWindow}, {Inventory}", nameof(WindowSetSlotPacket), packet.WindowId, CurrentlyOpenedWindow?.ToString() ?? "null", Inventory?.ToString() ?? "null");
            return Task.CompletedTask;
        }

        // Logger.Debug("Handle set slot: {Slot}", packet.Slot);

        window.StateId = packet.StateId;
        window.SetSlot(packet.Slot);

        return Task.CompletedTask;
    }

    private Task HandleWindowItems(WindowItemsPacket packet)
    {
        Window? window;
        lock (windowLock)
        {
            if (!openWindows.TryGetValue(packet.WindowId, out window))
            {
                Logger.Warn("Received {PacketType} for windowId={WindowId}, but it's not opened", packet.GetType().Name, packet.WindowId);

                // Cache items in case it gets opened in a bit
                cachedWindowItemsPacket = packet;
                cacheTimestamp = DateTime.Now;

                return Task.CompletedTask;
            }

            Logger.Debug("HandleWindowItems for window {WindowTitle}", window.Title);
        }

        var slots = packet.Items
                          .Select((x, i) => new Slot(x, (short)i))
                          .ToArray();
        window.StateId = packet.StateId;
        window.SetSlots(slots);

        if (window.WindowId == 0)
        {
            inventoryLoadedTsc.TrySetResult();
        }

        return Task.CompletedTask;
    }

    private Task HandleHeldItemChange(CBHeldItemPacket packet)
    {
        SelectedHotbarIndex = (byte)packet.Slot;
        OnHeldItemChanged.Dispatch(Bot, HeldItem);
        return Task.CompletedTask;
    }

    private Task HandleOpenWindow(OpenWindowPacket packet)
    {
        var windowInfo = Bot.Data.Windows.ById(packet.InventoryType);

        windowInfo = windowInfo with { Title = packet.WindowTitle };
        Logger.Debug("Received Open Window Packet id={WindowId}", packet.WindowId);
        OpenWindow(packet.WindowId, windowInfo);

        return Task.CompletedTask;
    }
}
