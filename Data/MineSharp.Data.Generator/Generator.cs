namespace MineSharp.Data.Generator
{
    internal abstract class Generator
    {

        public Generator(MinecraftDataHelper wrapper, string version)
        {
            this.Wrapper = wrapper;
            this.Version = version;
        }

        public MinecraftDataHelper Wrapper { get; set; }
        public string Version { get; set; }

        public abstract void WriteCode(CodeGenerator codeGenerator);
    }
}
