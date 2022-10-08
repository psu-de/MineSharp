namespace MineSharp.Bot
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class BotFunctionAttribute : Attribute
    {

        public string Category;
        public string Description;
        public BotFunctionAttribute(string category, string description)
        {

            this.Category = category;
            this.Description = description;
        }
    }
}
