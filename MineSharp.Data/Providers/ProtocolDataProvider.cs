using MineSharp.Data.Exceptions;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace MineSharp.Data.Providers;

public class ProtocolDataProvider : IDataProvider
{
    private Dictionary<string, int> _clientHandshakeIds;
    private Dictionary<string, int> _clientLoginIds;
    private Dictionary<string, int> _clientStatusIds;
    private Dictionary<string, int> _clientPlayIds;

    private Dictionary<int, string> _clientHandshakeNames;
    private Dictionary<int, string> _clientLoginNames;
    private Dictionary<int, string> _clientStatusNames;
    private Dictionary<int, string> _clientPlayNames;
    
    private Dictionary<string, int> _serverHandshakeIds;
    private Dictionary<string, int> _serverLoginIds;
    private Dictionary<string, int> _serverStatusIds;
    private Dictionary<string, int> _serverPlayIds;
    
    private Dictionary<int, string> _serverHandshakeNames;
    private Dictionary<int, string> _serverLoginNames;
    private Dictionary<int, string> _serverStatusNames;
    private Dictionary<int, string> _serverPlayNames;

    private readonly string _path;
    
    public int Version { get; }
    public bool IsLoaded { get; private set; }

    public ProtocolDataProvider(string protocolPath, int protocolVersion)
    {
        this._path = protocolPath;
        this.Version = protocolVersion;
        
        this._clientHandshakeIds = new Dictionary<string, int>();
        this._clientLoginIds = new Dictionary<string, int>();
        this._clientStatusIds = new Dictionary<string, int>();
        this._clientPlayIds = new Dictionary<string, int>();

        this._clientHandshakeNames = new Dictionary<int, string>();
        this._clientLoginNames = new Dictionary<int, string>();
        this._clientStatusNames = new Dictionary<int, string>();
        this._clientPlayNames = new Dictionary<int, string>();
        
        this._serverHandshakeIds = new Dictionary<string, int>();
        this._serverLoginIds = new Dictionary<string, int>();
        this._serverStatusIds = new Dictionary<string, int>();
        this._serverPlayIds = new Dictionary<string, int>();
        
        this._serverHandshakeNames = new Dictionary<int, string>();
        this._serverLoginNames = new Dictionary<int, string>();
        this._serverStatusNames = new Dictionary<int, string>();
        this._serverPlayNames = new Dictionary<int, string>();

        this.IsLoaded = false;
    }
    
    public int GetClientHandshakePacketId(string name)
    {
        if (!this._clientHandshakeIds.TryGetValue(name, out var id))
        {
            throw new MineSharpDataException($"Packet with Name {name} not found.");
        }

        return id;
    }
    
    public int GetClientLoginPacketId(string name)
    {
        if (!this._clientLoginIds.TryGetValue(name, out var id))
        {
            throw new MineSharpDataException($"Packet with Name {name} not found.");
        }

        return id;
    }
    
    public int GetClientStatusPacketId(string name)
    {
        if (!this._clientStatusIds.TryGetValue(name, out var id))
        {
            throw new MineSharpDataException($"Packet with Name {name} not found.");
        }

        return id;
    }
    
    public int GetClientPlayPacketId(string name)
    {
        if (!this._clientPlayIds.TryGetValue(name, out var id))
        {
            throw new MineSharpDataException($"Packet with Name {name} not found.");
        }

        return id;
    }
    
    
    public int GetServerHandshakePacketId(string name)
    {
        if (!this._serverHandshakeIds.TryGetValue(name, out var id))
        {
            throw new MineSharpDataException($"Packet with Name {name} not found.");
        }

        return id;
    }
    
    public int GetServerLoginPacketId(string name)
    {
        if (!this._serverLoginIds.TryGetValue(name, out var id))
        {
            throw new MineSharpDataException($"Packet with Name {name} not found.");
        }

        return id;
    }
    
    public int GetServerStatusPacketId(string name)
    {
        if (!this._serverStatusIds.TryGetValue(name, out var id))
        {
            throw new MineSharpDataException($"Packet with Name {name} not found.");
        }

        return id;
    }
    
    public int GetServerPlayPacketId(string name)
    {
        if (!this._serverPlayIds.TryGetValue(name, out var id))
        {
            throw new MineSharpDataException($"Packet with Name {name} not found.");
        }

        return id;
    }


    public string GetClientHandshakePacketName(int id)
    {
        if (!this._clientHandshakeNames.TryGetValue(id, out var name))
        {
            throw new MineSharpDataException($"Packet with id {id} not found.");
        }

        return name;
    }
    
    public string GetClientLoginPacketName(int id)
    {
        if (!this._clientLoginNames.TryGetValue(id, out var name))
        {
            throw new MineSharpDataException($"Packet with id {id} not found.");
        }

        return name;
    }
    
    public string GetClientStatusPacketName(int id)
    {
        if (!this._clientStatusNames.TryGetValue(id, out var name))
        {
            throw new MineSharpDataException($"Packet with id {id} not found.");
        }

        return name;
    }
    
    public string GetClientPlayPacketName(int id)
    {
        if (!this._clientPlayNames.TryGetValue(id, out var name))
        {
            throw new MineSharpDataException($"Packet with id {id} not found.");
        }

        return name;
    }
    
    
    public string GetServerHandshakePacketName(int id)
    {
        if (!this._serverHandshakeNames.TryGetValue(id, out var name))
        {
            throw new MineSharpDataException($"Packet with id {id} not found.");
        }

        return name;
    }
    
    public string GetServerLoginPacketName(int id)
    {
        if (!this._serverLoginNames.TryGetValue(id, out var name))
        {
            throw new MineSharpDataException($"Packet with id {id} not found.");
        }

        return name;
    }
    
    public string GetServerStatusPacketName(int id)
    {
        if (!this._serverStatusNames.TryGetValue(id, out var name))
        {
            throw new MineSharpDataException($"Packet with id {id} not found.");
        }

        return name;
    }
    
    public string GetServerPlayPacketName(int id)
    {
        if (!this._serverPlayNames.TryGetValue(id, out var name))
        {
            throw new MineSharpDataException($"Packet with id {id} not found.");
        }

        return name;
    }
    
    
    public void Load()
    {
        if (IsLoaded)
        {
            return;
        }
        
        this.LoadData();

        IsLoaded = true;
    }

    private void LoadData()
    {
        JObject jObject = JObject.Parse(File.ReadAllText(this._path));

        this._clientHandshakeIds = this.GetPacketMapping(jObject.SelectToken(GetJPath("handshaking", "toClient"))!);
        this._clientLoginIds = this.GetPacketMapping(jObject.SelectToken(GetJPath("login", "toClient"))!);
        this._clientStatusIds = this.GetPacketMapping(jObject.SelectToken(GetJPath("status", "toClient"))!);
        this._clientPlayIds = this.GetPacketMapping(jObject.SelectToken(GetJPath("play", "toClient"))!);
        
        this._serverHandshakeIds = this.GetPacketMapping(jObject.SelectToken(GetJPath("handshaking", "toServer"))!);
        this._serverLoginIds = this.GetPacketMapping(jObject.SelectToken(GetJPath("login", "toServer"))!);
        this._serverStatusIds = this.GetPacketMapping(jObject.SelectToken(GetJPath("status", "toServer"))!);
        this._serverPlayIds = this.GetPacketMapping(jObject.SelectToken(GetJPath("play", "toServer"))!);

        this._clientHandshakeNames = this._clientHandshakeIds.ToDictionary(x => x.Value, x => x.Key);
        this._clientLoginNames = this._clientLoginIds.ToDictionary(x => x.Value, x => x.Key);
        this._clientStatusNames = this._clientStatusIds.ToDictionary(x => x.Value, x => x.Key);
        this._clientPlayNames = this._clientPlayIds.ToDictionary(x => x.Value, x => x.Key);
        
        this._serverHandshakeNames = this._serverHandshakeIds.ToDictionary(x => x.Value, x => x.Key);
        this._serverLoginNames = this._serverLoginIds.ToDictionary(x => x.Value, x => x.Key);
        this._serverStatusNames = this._serverStatusIds.ToDictionary(x => x.Value, x => x.Key);
        this._serverPlayNames = this._serverPlayIds.ToDictionary(x => x.Value, x => x.Key);
    }

    private Dictionary<string, int> GetPacketMapping(JToken token)
    {
        if (token.Type != JTokenType.Object)
        {
            throw new MineSharpDataException("Invalid protocol data.");
        }

        JObject obj = (JObject)token;

        var dict = new Dictionary<string, int>();
        foreach (var prop in obj.Properties())
        {
            dict.Add((string)prop.Value!, int.Parse(prop.Name.Substring(2), NumberStyles.HexNumber));
        }

        return dict;
    }
    
    private string GetJPath(string state, string direction)
    {
        return $"$.{state}.{direction}.types.packet[1][0].type[1].mappings";
    }
}
