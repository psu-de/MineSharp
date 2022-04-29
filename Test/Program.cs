// See https://aka.ms/new-console-template for more information
using MineSharp.Core.Logging;
using MineSharp.MojangAuth;
using MineSharp.Protocol;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Status;
using MineSharp.Bot;
using MineSharp.Core.Types;
using MineSharp.Data.Entities;
using MineSharp.Core.Types.Enums;
using MineSharp.Protocol.Packets;
using MineSharp.Data.Blocks;
using MineSharp.Data.Biomes;
using MineSharp.Data;
using MineSharp.Data.Windows;

using ConInp;
using System.Reflection;



Console.ReadKey();


Logger Logger = Logger.GetLogger("Main");
MinecraftData.Load();

//MineSharp.World.World world = new MineSharp.World.World();

//var chunk = new MineSharp.World.Chunks.Chunk(0, 0, File.ReadAllBytes("chunk_-11_-4.dump"));
//var pos = new Position(0, 200, 0);
//var blockInfo = BlockData.Blocks[(int)BlockType.BlockofDiamond];
//var block = new Block(blockInfo, pos, blockInfo.MinStateId);

//Block[] blocks = chunk.FindBlocksAsync(BlockType.Air).GetAwaiter().GetResult();
//Console.WriteLine(chunk.GetBlockAt(pos).ToString());
//Console.WriteLine("Found " + blocks.Length + " blocks");

//chunk.SetBlock(block);
//Console.WriteLine(chunk.GetBlockAt(pos).ToString());
//blocks = chunk.FindBlocksAsync(BlockType.Air).GetAwaiter().GetResult();
//Console.WriteLine("Found " + blocks.Length + " blocks");


//Console.ReadKey();
//Environment.Exit(0);

MineSharp.Bot.Bot bot;

Task.Run(async () => {
     bot = new Bot(new Bot.BotOptions() {
        Host = "127.0.0.1",
        Offline = true,
        UsernameOrEmail = "afk_paul4",
        Version = "1.18.1"
    });

    //bot = new Bot(new Bot.BotOptions() {
    //    Host = "34.159.227.82",
    //    UsernameOrEmail = "franurrutiap@gmail.com",
    //    Password = "franlala123",
    //    Version = "1.18.1"
    //});


    //bot.Joined += Event_Joined;
    //bot.HealthChanged += Event_HealthChanged;
    bot.Died += Event_BotDied;
    //bot.Respawned += Event_Respawned;
    //bot.PlayerLeft += Eventn_PlayerLeft;
    //bot.PlayerJoined += Event_PlayerJoined;
    //bot.PlayerLoaded += Event_PlayerLoaded;

    bot.HeldItemChanged += Event_HeldItemChanged;
    bot.EntitySpawned += Event_EntitySpawned;
    //bot.EntityDespawned += Event_EntityDespawned;




    await bot.Connect();
    await bot.WaitUntilLoaded();

    await Task.Delay(1000);
    await Task.Delay(3000);

    await bot.MineBlock(bot.World.GetBlockAt(new MineSharp.Core.Types.Position(331, 72, 793)));

    Logger.Info("Sent " + nameof(PlayerDiggingPacket));
    //Logger.Info("Picking up item");
    //bot.Inventory.Click((short)InventoryWindow.InvSlots.HotBarStart, WindowOperationMode.MouseLeftRight, 0);
    //await Task.Delay(1000);
    //Logger.Info("Dropping items");
    //bot.Inventory.Click(-999, WindowOperationMode.MouseLeftRight, 1);
    //Logger.Info("Putting back in slot");
    //bot.Inventory.Click((short)InventoryWindow.InvSlots.HotBarStart, WindowOperationMode.MouseLeftRight, 0);

    //Logger.Info("Shift leftclick");
    //bot.Inventory.Click((short)bot.Inventory.GetHotbarSlotIndex(bot.SelectedHotbarIndex), WindowOperationMode.ShiftMouseLeftRight, 0);
    //await Task.Delay(1000);
    //Logger.Info("Current helmet: " + bot.Inventory.Slots[(int)InventoryWindow.InvSlots.ArmorHead].Item.ToString());
});


void Event_EntitySpawned(Entity entity) {

    if (bot.BotEntity == null) return; // TODO: PlayerLoaded awaiten

    if (entity.Position.DistanceSquared(bot.BotEntity.Position) < 36) {

        Logger.Info("Attack entity " + entity.EntityInfo.Name + " at " + entity.Position);
        bot.Attack(entity);
    }
}

new Thread(() => {
    while (true) {
        Thread.Sleep(1000);
        break;
        string? str = Console.ReadLine();
        if (str != null) {


            try {
                string[] parts = str.Split(" ");
                Console.WriteLine(parts.Length);

                switch (parts.Length) {
                    case 1:
                        //BlockType type = (BlockType)Enum.Parse(typeof(BlockType), str);
                        //if (type != null) {
                        //    var block = bot.World.FindBlocksAsync(type).GetAwaiter().GetResult();
                        //    if (block == null) {
                        //        Logger.Info("No block was found");
                        //        continue;
                        //    }
                        //    Logger.Info($"Found {block.Length} Blocks: " + String.Join(",", block.Select(b => b.ToString()).Take(block.Length > 10 ? 10 : block.Length)));
                        //}
                        //int slot = int.Parse(str);
                        //Logger.Info("Switching to slot " + slot);
                        //bot.SelectHotbarSlot((byte)slot).GetAwaiter().GetResult();
                        break;
                    case 3:
                        int x = int.Parse(parts[0]);
                        int y = int.Parse(parts[1]);
                        int z = int.Parse(parts[2]);
                        var pos = new Position(x, y, z);
                        Logger.Info(pos.ToString());
                        var block = bot.World.GetBlockAt(pos);
                        Logger.Info("Breaking: " + block.ToString() + " Hardness=" + block.Info.Hardness);
                        var result = bot.MineBlock(block).GetAwaiter().GetResult();
                        Logger.Info("Finished: " + Enum.GetName(typeof(MineSharp.Bot.Enums.MineBlockStatus), result));
                        break;
                }


            } catch (Exception e) {
                Logger.Error("Could not get block: " + e);
            }
        }
    }
}).Start();

void Event_BotDied(Chat message) {
    Logger.Warning("Bot Died: " + message);
    Task.Delay(1000).Wait();
    Logger.Info("Respawning");
    bot.Respawn();
}

void Event_HeldItemChanged(MineSharp.Data.Items.Item item) {
    Logger.Info("Held item: " + item);
}

while (true) {
    Console.ReadKey();
}