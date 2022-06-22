namespace MineSharp.Core.Versions {
    public class MinecraftVersion {

        public string Version { get; private set; } 
        public int ProtocolId { get; private set; }

        public MinecraftVersion(string version)
        {
            this.Version = version;
            this.ProtocolId = ProtocolVersion.GetVersionNumber(version);

        }

    }
}
