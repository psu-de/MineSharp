namespace MineSharp.Data;

internal abstract class DataVersion<TEnum, T> where TEnum : Enum where T : class 
{
    public abstract Dictionary<TEnum, T> Palette { get; }
}
