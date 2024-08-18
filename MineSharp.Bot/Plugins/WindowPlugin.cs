using System.Collections.Concurrent;
using MineSharp.Bot.Exceptions;
using MineSharp.Bot.Windows;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Blocks;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Concurrency;
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
        inventoryLoadedTsc = new(TaskCreationOptions.RunContinuationsAsynchronously);
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
        OnPacketAfterInitialization<WindowItemsPacket>(HandleWindowItems);
        OnPacketAfterInitialization<WindowSetSlotPacket>(HandleSetSlot);
        OnPacketAfterInitialization<CBHeldItemPacket>(HandleHeldItemChange);
        OnPacketAfterInitialization<OpenWindowPacket>(HandleOpenWindow);
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
    protected override async Task Init()
    {
        playerPlugin = Bot.GetPlugin<PlayerPlugin>();
        await playerPlugin.WaitForInitialization().WaitAsync(Bot.CancellationToken);

        CreateInventory();
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

        openContainerTsc = new(TaskCreationOptions.RunContinuationsAsynchronously);

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
    ///     Confirmation flags for eating a food item.
    /// </summary>
    [Flags]
    public enum EatFoodItemConfirmation
    {
        /// <summary>
        ///     No confirmation.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Confirm if food or saturation value increased.
        /// </summary>
        FoodOrSaturationValueIncreased = 1 << 0,

        /// <summary>
        ///     Confirm if food item count decreased.
        /// </summary>
        FoodItemCountDecreased = 1 << 1,

        /// <summary>
        ///     Confirm all conditions.
        /// </summary>
        All = FoodOrSaturationValueIncreased | FoodItemCountDecreased
    }

    /// <summary>
    ///     Reasons why eating food was canceled.
    /// </summary>
    public enum EatFoodItemResult
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        /// <summary>
        /// This value is internal and should never be returned.
        /// </summary>
        Unknown,
        SuccessfullyEaten,
        CancelledUserRequested,
        CancelledBotDisconnected,
        CancelledBotDied,
        CancelledFoodItemSlotChanged,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    ///     Makes the bot eat a food item from the specified hand.
    /// </summary>
    /// <param name="hand">The hand from which to eat the food item.</param>
    /// <param name="eatFoodItemConfirmation">The confirmation flags determining what checks should be made to ensure the food was eaten (server side).</param>
    /// <param name="cancellationToken">The cancellation token to cancel the task.</param>
    /// <returns>A task that completes once the food was eaten or eating the food was cancelled.</returns>
    public async Task<EatFoodItemResult> EatFoodItem(PlayerHand hand = PlayerHand.MainHand, EatFoodItemConfirmation eatFoodItemConfirmation = EatFoodItemConfirmation.All, CancellationToken cancellationToken = default)
    {
        var foodSlot = await GetSlotForPlayerHand(hand);

        // TODO: Also check whether the item is eatable
        if (foodSlot is null)
        {
            throw new Exception("No food item found in hand");
        }

        var result = EatFoodItemResult.Unknown;

        var cts = CancellationTokenSource.CreateLinkedTokenSource(Bot.CancellationToken, cancellationToken);
        var token = cts.Token;
        var botDiedTask = Bot.Client.WaitForPacketWhere<SetHealthPacket>(packet => packet.Health <= 0, token)
            .ContinueWith(_ =>
            {
                InterlockedHelper.CompareExchangeIfNot(ref result, EatFoodItemResult.CancelledBotDied, EatFoodItemResult.SuccessfullyEaten);
                Logger.Debug("Bot died while eating food.");
                return cts.CancelAsync();
            }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);

        var currentFood = playerPlugin!.Food;
        var currentSaturation = playerPlugin!.Saturation;
        var foodValueIncreasedTask = eatFoodItemConfirmation.HasFlag(EatFoodItemConfirmation.FoodOrSaturationValueIncreased)
            ? Bot.Client.WaitForPacketWhere<SetHealthPacket>(packet => packet.Food > currentFood || packet.Saturation > currentSaturation, token)
            // for debugging race conditions
            //.ContinueWith(t =>
            //{
            //    var packet = t.Result;
            //    Logger.Debug("Eat finish SetHealthPacket: {Health} {Food} {Saturation}", packet.Health, packet.Food, packet.Saturation);
            //}, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously)
            : Task.CompletedTask;

        TaskCompletionSource? foodItemCountDecreasedTcs = null;
        CancellationTokenRegistration cancellationTokenRegistration = default;
        Task foodSlotUpdatedTask = Task.CompletedTask;
        if (eatFoodItemConfirmation.HasFlag(EatFoodItemConfirmation.FoodItemCountDecreased))
        {
            foodItemCountDecreasedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            cancellationTokenRegistration = token.Register(() => foodItemCountDecreasedTcs.TrySetCanceled());
            foodSlotUpdatedTask = foodItemCountDecreasedTcs.Task;
        }
        Inventory!.OnSlotChanged += HandleSlotUpdate;
        Task HandleSlotUpdate(Window window, short index)
        {
            var slot = window.GetSlot(index);
            if (slot.SlotIndex == foodSlot.SlotIndex)
            {
                var itemTypeMatches = slot.Item?.Info.Type == foodSlot.Item!.Info.Type;
                var currentCount = slot.Item?.Count ?? 0;
                if (currentCount == 0 || itemTypeMatches)
                {
                    if (eatFoodItemConfirmation.HasFlag(EatFoodItemConfirmation.FoodItemCountDecreased))
                    {
                        if (currentCount > foodSlot.Item!.Count)
                        {
                            // player picked up more food
                            // we need to update our expected food count
                            foodSlot.Item!.Count = currentCount;
                        }
                        else if (currentCount < foodSlot.Item!.Count)
                        {
                            // food item got removed from slot
                            foodItemCountDecreasedTcs!.TrySetResult();
                        }
                    }
                }
                // not else if. Do both
                if (!itemTypeMatches)
                {
                    // food item got removed or replaced from slot
                    // the case that the food count reached zero can be both an cancel reason and a success reason
                    InterlockedHelper.CompareExchange(ref result, EatFoodItemResult.CancelledFoodItemSlotChanged, EatFoodItemResult.Unknown);
                    return cts.CancelAsync();
                }
            }
            return Task.CompletedTask;
        }
        await UseItem(hand);

        EatFoodItemResult finalResult = EatFoodItemResult.Unknown;
        try
        {
            // wait for food to be eaten
            // no WaitAsync here because both tasks will get canceled
            await Task.WhenAll(foodValueIncreasedTask, foodSlotUpdatedTask);
            // TODO: Maybe here is a race condition when the foodSlotUpdatedTask finishes (item == null) so that foodValueIncreasedTask gets canceled before the packet gets handled
            // or that the player died but the died packet came in later
            // ^^ only solution would be a packetReceiveIndex or synchronous packet handling

            // here we do not use interlocked because it commonly happens that the result is set to CancelledFoodItemSlotChanged (item == null)
            // but that is a this a success case
            result = EatFoodItemResult.SuccessfullyEaten;
            Logger.Debug("Bot ate food of type {FoodType} at slot {SlotIndex}.", foodSlot.Item?.Info.Type, foodSlot.SlotIndex);
        }
        catch (OperationCanceledException)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                InterlockedHelper.CompareExchange(ref result, EatFoodItemResult.CancelledUserRequested, EatFoodItemResult.Unknown);
            }
            else if (Bot.CancellationToken.IsCancellationRequested)
            {
                InterlockedHelper.CompareExchange(ref result, EatFoodItemResult.CancelledBotDisconnected, EatFoodItemResult.Unknown);
            }
        }
        finally
        {
            Inventory!.OnSlotChanged -= HandleSlotUpdate;
            cancellationTokenRegistration.Dispose();

            // cancel the other WaitForPacket tasks (like botDiedTask)
            await cts.CancelAsync();
            cts.Dispose();
            finalResult = result;
            if (finalResult != EatFoodItemResult.SuccessfullyEaten)
            {
                Logger.Debug("Eating food of type {FoodType} at slot {SlotIndex} was cancelled with reason: {CancelReason}.", foodSlot.Item?.Info.Type, foodSlot.SlotIndex, finalResult);
            }
        }
        // if it occurs that the cancelReason is not EatFoodCancelReason.None but the "Bot ate food" is logged than there is a race condition
        return finalResult;
    }

    /// <summary>
    ///     Gets the slot for the specified player hand.
    /// </summary>
    /// <param name="hand">The hand to get the slot for.</param>
    /// <returns>The slot corresponding to the specified hand.</returns>
    public async Task<Slot> GetSlotForPlayerHand(PlayerHand hand)
    {
        await WaitForInventory();

        var slot = hand == PlayerHand.MainHand
            ? Inventory!.GetSlot((short)(PlayerWindowSlots.HotbarStart + SelectedHotbarIndex))
            : Inventory!.GetSlot(Inventory.Slot.OffHand);

        return slot;
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
        var handSlot = await GetSlotForPlayerHand(hand);

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
