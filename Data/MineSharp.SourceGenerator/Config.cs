namespace MineSharp.SourceGenerator;

public static class Config
{
    public static string OutputDirectory = "out";
    public static string[] IncludedVersions = new[] {
        "1.18",
        "1.18.1", 
        "1.18.2", 
        "1.19", 
        "1.19.2", 
        "1.19.3", 
        "1.19.4", 
        "1.20", 
        "1.20.1", 
        "1.20.2",
        "1.20.3",
        "1.20.4"
    };
    public static string LatestVersion = "1.20.4";
}