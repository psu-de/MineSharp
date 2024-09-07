using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     Reset Score packet
/// </summary>
/// <param name="EntityName">The entity whose score this is. For players, this is their username; for other entities, it is their UUID.</param>
/// <param name="HasObjectiveName">Whether the score should be removed for the specified objective, or for all of them.</param>
/// <param name="ObjectiveName">The name of the objective the score belongs to. Only present if the previous field is true.</param>
public sealed record ResetScorePacket(string EntityName, bool HasObjectiveName, string? ObjectiveName) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_ResetScore;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteString(EntityName);
        buffer.WriteBool(HasObjectiveName);
        if (HasObjectiveName)
        {
            buffer.WriteString(ObjectiveName!);
        }
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var entityName = buffer.ReadString();
        var hasObjectiveName = buffer.ReadBool();
        string? objectiveName = null;
        if (hasObjectiveName)
        {
            objectiveName = buffer.ReadString();
        }

        return new ResetScorePacket(entityName, hasObjectiveName, objectiveName);
    }
}
