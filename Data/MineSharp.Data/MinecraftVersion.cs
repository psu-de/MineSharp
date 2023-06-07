using System.Text;

namespace MineSharp.Data;

public class MinecraftVersion
{
    private const int ANY = -1;
    
    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }

    public MinecraftVersion(int major, int minor, int patch)
    {
        this.Major = major;
        this.Minor = minor;
        this.Patch = patch;
    }

    public MinecraftVersion(string version)
    {
        bool isMajorVersion = false;
        
        if (version.EndsWith("_major"))
        {
            isMajorVersion = true;
            version = version.Replace("_major", "");
        }
        
        int[] versionNumbers = version.Split(".").Select(x => Convert.ToInt32(x)).ToArray();
        this.Major = versionNumbers[0];
        this.Minor = versionNumbers[1];
        this.Patch = versionNumbers.Length > 2 ? versionNumbers[2] : 0;

        if (isMajorVersion)
        {
            this.Patch = ANY;
        }
    }
    
    public static bool operator >= (MinecraftVersion a, MinecraftVersion b)
    {
        if (a.Major != b.Major)
            return a.Major > b.Major;

        if (a.Minor != b.Minor)
            return a.Minor > b.Minor;

        return a.Patch == ANY || a.Patch >= b.Patch;
    }
    
    public static bool  operator <= (MinecraftVersion a, MinecraftVersion b)
    {
        if (a.Major != b.Major)
            return a.Major < b.Major;

        if (a.Minor != b.Minor)
            return a.Minor < b.Minor;

        return a.Patch == ANY || a.Patch <= b.Patch;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"{Major}.{Minor}");

        if (Patch != ANY)
        {
            sb.Append($".{Patch}");
        }

        return sb.ToString();
    }
}

public class MinecraftVersionRange
{
    private readonly MinecraftVersion _from;
    private readonly MinecraftVersion _to;
    
    public MinecraftVersionRange(MinecraftVersion from, MinecraftVersion to)
    {
        this._from = from;
        this._to = to;
    }

    public bool ContainsVersion(MinecraftVersion version)
    {
        return version >= this._from && version <= this._to;
    }
}