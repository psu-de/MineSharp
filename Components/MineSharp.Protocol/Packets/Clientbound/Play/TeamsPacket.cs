namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class TeamsPacket : Packet {

        public string TeamName { get; private set; }
        public TeamsMode Mode { get; private set; }
        public object? Properties { get; private set; }

        public TeamsPacket() { }

        public TeamsPacket(string teamName, TeamsMode mode, object? properties) {
            TeamName = teamName;
            Mode = mode;
            Properties = properties;
        }

        public override void Read(PacketBuffer buffer) {
            this.TeamName = buffer.ReadString();
            this.Mode = (TeamsMode)buffer.ReadByte();
            switch (this.Mode) {
                case TeamsMode.CreateTeam:
                    this.Properties = new { TeamDisplayName = buffer.ReadChat(), 
                                            FriendlyFlags = buffer.ReadByte(), 
                                            NameTagVisibility = buffer.ReadString(), 
                                            CollisionRule = buffer.ReadString(), 
                                            TeamColor = buffer.ReadVarInt(), 
                                            TeamPrefix = buffer.ReadChat(), 
                                            TeamSuffix = buffer.ReadChat(), 
                                            Entities = buffer.ReadStringArray() };
                    break;
                case TeamsMode.RemoveTeam: break;
                case TeamsMode.UpdateTeamInfo:
                    this.Properties = new {
                        TeamDisplayName = buffer.ReadChat(),
                        FriendlyFlags = buffer.ReadByte(),
                        NameTagVisibility = buffer.ReadString(),
                        CollisionRule = buffer.ReadString(),
                        TeamColor = buffer.ReadVarInt(),
                        TeamPrefix = buffer.ReadChat(),
                        TeamSuffix = buffer.ReadChat()
                    };
                    break;
                case TeamsMode.AddEntitiesToTeam:
                case TeamsMode.RemoveEntitiesFromTeam:
                    this.Properties = new { Entities = buffer.ReadString() };
                    break;
            }
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }

        public enum TeamsMode {
            CreateTeam = 0,
            RemoveTeam = 1,
            UpdateTeamInfo = 2,
            AddEntitiesToTeam = 3,
            RemoveEntitiesFromTeam = 4
        }
    }
}
