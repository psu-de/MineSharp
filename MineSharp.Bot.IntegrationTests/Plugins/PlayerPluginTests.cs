using MineSharp.Bot.Plugins;

namespace MineSharp.Bot.IntegrationTests.Plugins;

public class Tests
{
    private MinecraftBot bot;
    private PlayerPlugin player;

    [SetUp]
    public void Setup()
    {
        this.bot = GlobalSetup.CreateBotAndConnect().GetAwaiter().GetResult();
        this.player = this.bot.GetPlugin<PlayerPlugin>();
    }

    [Test]
    public void TestHealth()
    {
        this.player.OnHealthChanged += (bot) =>
        {
            
        };
        
        
    }
}
