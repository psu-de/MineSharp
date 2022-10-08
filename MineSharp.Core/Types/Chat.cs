namespace MineSharp.Core.Types
{
    public class Chat
    {

        public Chat(string json)
        {
            this.JSON = json;
        }

        public string JSON {
            get;
        }

        public override string ToString() => this.JSON;
    }
}
