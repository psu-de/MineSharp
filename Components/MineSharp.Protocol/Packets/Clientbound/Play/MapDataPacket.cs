using MineSharp.ChatComponent;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using static MineSharp.Protocol.Packets.Clientbound.Play.MapDataPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Updates a rectangular area on a map item.
/// </summary>
/// <param name="MapId">Map ID of the map being modified</param>
/// <param name="Scale">From 0 for a fully zoomed-in map (1 block per pixel) to 4 for a fully zoomed-out map (16 blocks per pixel)</param>
/// <param name="Locked">True if the map has been locked in a cartography table</param>
/// <param name="HasIcons">Indicates if the map has icons</param>
/// <param name="Icons">Array of icons on the map</param>
/// <param name="ColorPatch">Details of the color patch update</param>
public sealed record MapDataPacket(int MapId, byte Scale, bool Locked, bool HasIcons, Icon[]? Icons, ColorPatchInfo ColorPatch) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_Map;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(MapId);
        buffer.WriteByte(Scale);
        buffer.WriteBool(Locked);
        buffer.WriteBool(HasIcons);

        if (HasIcons)
        {
            buffer.WriteVarInt(Icons?.Length ?? 0);
            if (Icons != null)
            {
                foreach (var icon in Icons)
                {
                    icon.Write(buffer);
                }
            }
        }

        ColorPatch.Write(buffer);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var mapId = buffer.ReadVarInt();
        var scale = buffer.ReadByte();
        var locked = buffer.ReadBool();
        var hasIcons = buffer.ReadBool();

        Icon[]? icons = null;
        if (hasIcons)
        {
            var iconCount = buffer.ReadVarInt();
            icons = new Icon[iconCount];
            for (int i = 0; i < iconCount; i++)
            {
                icons[i] = Icon.Read(buffer);
            }
        }

        var colorPatch = ColorPatchInfo.Read(buffer);

        return new MapDataPacket(mapId, scale, locked, hasIcons, icons, colorPatch);
    }

    /// <summary>
    ///     Represents an icon on the map.
    /// </summary>
    /// <param name="Type">Type of the icon</param>
    /// <param name="X">X coordinate of the icon</param>
    /// <param name="Z">Z coordinate of the icon</param>
    /// <param name="Direction">Direction of the icon</param>
    /// <param name="HasDisplayName">Indicates if the icon has a display name</param>
    /// <param name="DisplayName">Display name of the icon</param>
    public sealed record Icon(MapIconType Type, byte X, byte Z, byte Direction, bool HasDisplayName, Chat? DisplayName) : ISerializable<Icon>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteVarInt((int)Type);
            buffer.WriteByte(X);
            buffer.WriteByte(Z);
            buffer.WriteByte(Direction);
            buffer.WriteBool(HasDisplayName);
            if (HasDisplayName)
            {
                buffer.WriteChatComponent(DisplayName!);
            }
        }

        /// <inheritdoc />
        public static Icon Read(PacketBuffer buffer)
        {
            var type = (MapIconType)buffer.ReadVarInt();
            var x = buffer.ReadByte();
            var z = buffer.ReadByte();
            var direction = buffer.ReadByte();
            var hasDisplayName = buffer.ReadBool();
            Chat? displayName = null;
            if (hasDisplayName)
            {
                displayName = buffer.ReadChatComponent();
            }

            return new Icon(type, x, z, direction, hasDisplayName, displayName);
        }
    }

    /// <summary>
    ///     Represents the color patch update on the map.
    /// </summary>
    /// <param name="Columns">Number of columns updated</param>
    /// <param name="Rows">Number of rows updated</param>
    /// <param name="X">X offset of the westernmost column</param>
    /// <param name="Z">Z offset of the northernmost row</param>
    /// <param name="Length">Length of the following array</param>
    /// <param name="Data">Array of updated data</param>
    public sealed record ColorPatchInfo(byte Columns, byte? Rows, byte? X, byte? Z, int? Length, byte[]? Data) : ISerializable<ColorPatchInfo>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer)
        {
            buffer.WriteByte(Columns);
            if (Columns > 0)
            {
                buffer.WriteByte(Rows ?? 0);
                buffer.WriteByte(X ?? 0);
                buffer.WriteByte(Z ?? 0);
                buffer.WriteVarInt(Length ?? 0);
                if (Data != null)
                {
                    buffer.WriteBytes(Data);
                }
            }
        }

        /// <inheritdoc />
        public static ColorPatchInfo Read(PacketBuffer buffer)
        {
            var columns = buffer.ReadByte();
            byte? rows = null, x = null, z = null;
            int? length = null;
            byte[]? data = null;

            if (columns > 0)
            {
                rows = buffer.ReadByte();
                x = buffer.ReadByte();
                z = buffer.ReadByte();
                length = buffer.ReadVarInt();
                data = buffer.ReadBytes(length.Value);
            }

            return new ColorPatchInfo(columns, rows, x, z, length, data);
        }
    }

    /// <summary>
    ///     Represents the types of icons on a map.
    /// </summary>
    public enum MapIconType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        /// <summary>
        /// Alternative meaning: players
        /// </summary>
        WhiteArrow = 0,
        /// <summary>
        /// Alternative meaning: item frames
        /// </summary>
        GreenArrow = 1,
        RedArrow = 2,
        BlueArrow = 3,
        WhiteCross = 4,
        RedPointer = 5,
        /// <summary>
        /// Alternative meaning: off-map players
        /// </summary>
        WhiteCircle = 6,
        /// <summary>
        /// Alternative meaning: far-off-map players
        /// </summary>
        SmallWhiteCircle = 7,
        Mansion = 8,
        Monument = 9,
        WhiteBanner = 10,
        OrangeBanner = 11,
        MagentaBanner = 12,
        LightBlueBanner = 13,
        YellowBanner = 14,
        LimeBanner = 15,
        PinkBanner = 16,
        GrayBanner = 17,
        LightGrayBanner = 18,
        CyanBanner = 19,
        PurpleBanner = 20,
        BlueBanner = 21,
        BrownBanner = 22,
        GreenBanner = 23,
        RedBanner = 24,
        BlackBanner = 25,
        TreasureMarker = 26
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
