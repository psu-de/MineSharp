using System.Text;

namespace MineSharp.Data;

public class MinecraftVersion
{
    
    public int Protocol { get; }
    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }
    

    public MinecraftVersion(string version, int protocol)
    {
        int[] versionNumbers = version.Split(".").Select(x => Convert.ToInt32(x)).ToArray();
        this.Major = versionNumbers[0];
        this.Minor = versionNumbers[1];
        this.Patch = versionNumbers.Length > 2 ? versionNumbers[2] : 0;
        this.Protocol = protocol;
    }
    
    public static bool operator >= (MinecraftVersion a, MinecraftVersion b)
    {
        if (a.Major != b.Major)
            return a.Major > b.Major;

        if (a.Minor != b.Minor)
            return a.Minor > b.Minor;

        return a.Patch >= b.Patch;
    }
    
    public static bool operator <= (MinecraftVersion a, MinecraftVersion b)
    {
        if (a.Major != b.Major)
            return a.Major < b.Major;

        if (a.Minor != b.Minor)
            return a.Minor < b.Minor;

        return a.Patch <= b.Patch;
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }
}