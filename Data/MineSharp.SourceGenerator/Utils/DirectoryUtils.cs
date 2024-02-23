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

    public static string GetSourceDirectory()
    {
        var current = Environment.CurrentDirectory;
        var source  = Path.Join(current, "CoreSource");

        if (!Directory.Exists(source))
            Directory.CreateDirectory(source);

        return source;
    }

    public static string GetSourceDirectory(string subdirectory)
    {
        var source = GetSourceDirectory();

        var path = Path.Join(source, subdirectory);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public static string GetMineSharpCoreProjectDirectory()
    {
        var project = GetProjectDirectory();
        return Path.Join(project, "..", "..", "MineSharp.Core");
    }
}
