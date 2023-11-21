namespace MineSharp.SourceGenerator.Utils;

public static class DirectoryUtils
{
    public static string GetProjectDirectory()
    {
        return Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
    }

    public static string GetMinecraftDataDirectory()
    {
        return Path.Join(GetProjectDirectory(), "minecraft-data");
    }

    public static string GetCoreSourceDirectory()
    {
        var current = Environment.CurrentDirectory;
        var source = Path.Join(current, "CoreSource");
        
        if (!Directory.Exists(source))
            Directory.CreateDirectory(source);

        return source;
    }

    public static string GetDataSourceDirectory()
    {
        var current = Environment.CurrentDirectory;
        var source = Path.Join(current, "DataSource");
        
        if (!Directory.Exists(source))
            Directory.CreateDirectory(source);

        return source;
    }
    
    public static string GetDataSourceDirectory(string subdirectory)
    {
        var source = GetDataSourceDirectory();

        var path = Path.Join(source, subdirectory);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }
    
    public static string GetCoreSourceDirectory(string subdirectory)
    {
        var source = GetCoreSourceDirectory();

        var path = Path.Join(source, subdirectory);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public static string GetMineSharpDataProjectDirectory()
    {
        var project = GetProjectDirectory();
        return Path.Join(project, "..", "MineSharp.Data");
    }
    
    public static string GetMineSharpCoreProjectDirectory()
    {
        var project = GetProjectDirectory();
        return Path.Join(project, "..", "..", "MineSharp.Core");
    }
}
