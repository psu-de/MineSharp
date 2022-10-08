namespace MineSharp.Core.Types
{
    public class Chat
    {

        public string JSON { get; private set; }

        public Chat(string json)
        {
            this.JSON = json;
        }

        public override string ToString() => this.JSON;
    }
}
