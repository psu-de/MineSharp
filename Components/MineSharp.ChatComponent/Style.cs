using fNbt;
using Newtonsoft.Json.Linq;

namespace MineSharp.ChatComponent;

/// <summary>
/// Represents the text style of a chat component
/// </summary>
public class Style
{
    /// <summary>
    /// The default style
    /// </summary>
    public static Style DefaultStyle => new();

    /// <summary>
    /// The text color
    /// </summary>
    public string? Color  { get; set; } = null;
    
    /// <summary>
    /// The font
    /// </summary>
    public string? Font   { get; set; } = null;
    
    /// <summary>
    /// Whether the text is bold
    /// </summary>
    public bool?   Bold   { get; set; } = null;
    
    /// <summary>
    /// Whether the text is italic
    /// </summary>
    public bool?   Italic { get; set; } = null;
    
    /// <summary>
    /// Whether the text is underlined
    /// </summary>
    public bool?   Underlined    { get; set; } = null;
    
    /// <summary>
    /// Whether the text is struck through
    /// </summary>
    public bool?   Strikethrough { get; set; } = null;
    
    /// <summary>
    /// Whether the text is obfuscated
    /// </summary>
    public bool?   Obfuscated    { get; set; } = null;
    
    
    internal JObject? ToJson()
    {
        JObject? token = null;

        if (this.Color is not null)
        {
            token          ??= new JObject();
            token["color"] =   this.Color;
        }
        
        if (this.Font is not null)
        {
            token         ??= new JObject();
            token["font"] =   this.Font;
        }
        
        if (this.Bold is not null)
        {
            token         ??= new JObject();
            token["bold"] =   this.Bold;
        }        
        
        if (this.Italic is not null)
        {
            token           ??= new JObject();
            token["italic"] =   this.Italic;
        }
        
        if (this.Underlined is not null)
        {
            token               ??= new JObject();
            token["underlined"] =   this.Underlined;
        }
        
        if (this.Strikethrough is not null)
        {
            token                  ??= new JObject();
            token["strikethrough"] =   this.Strikethrough;
        }
        
        if (this.Obfuscated is not null)
        {
            token               ??= new JObject();
            token["obfuscated"] =   this.Obfuscated;
        }
        
        return token;
    }
    
    internal NbtCompound? ToNbt()
    {
        NbtCompound? nbt = null;

        if (this.Color is not null)
        {
            nbt          ??= new NbtCompound();
            nbt["color"] =   new NbtString("color", this.Color);
        }
        
        if (this.Font is not null)
        {
            nbt         ??= new NbtCompound();
            nbt["font"] =   new NbtString("font", this.Font);
        }
        
        if (this.Bold is not null)
        {
            nbt         ??= new NbtCompound();
            nbt["bold"] =   new NbtByte("bold", this.Bold.Value ? (byte)1 : (byte)0);
        }        
        
        if (this.Italic is not null)
        {
            nbt           ??= new NbtCompound();
            nbt["italic"] =   new NbtByte("italic", this.Italic.Value ? (byte)1 : (byte)0);
        }
        
        if (this.Underlined is not null)
        {
            nbt               ??= new NbtCompound();
            nbt["underlined"] =   new NbtByte("underlined", this.Underlined.Value ? (byte)1 : (byte)0);
        }
        
        if (this.Strikethrough is not null)
        {
            nbt                  ??= new NbtCompound();
            nbt["strikethrough"] =   new NbtByte("strikethrough", this.Strikethrough.Value ? (byte)1 : (byte)0);
        }
        
        if (this.Obfuscated is not null)
        {
            nbt               ??= new NbtCompound();
            nbt["obfuscated"] =   new NbtByte("obfuscated", this.Obfuscated.Value ? (byte)1 : (byte)0);
        }
        
        return nbt;
    }
}
