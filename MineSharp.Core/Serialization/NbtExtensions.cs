using fNbt;
using MineSharp.Core.Common;

namespace MineSharp.Core.Serialization;

/// <summary>
/// Provides extension methods for NbtCompound.
/// </summary>
public static class NbtExtensions
{
    /// <summary>
    /// Normalizes the top-level identifiers in the given NbtCompound.
    /// This is required when using the registry of a 1.21 server via ViaProxy.
    /// </summary>
    /// <param name="compound">The NbtCompound to normalize.</param>
    public static NbtCompound NormalizeRegistryDataTopLevelIdentifiers(this NbtCompound compound)
    {
        // we need to make copies of every tag because fNbt doesn't allow modifying the collection while iterating
        // and there is no way to clear the parent of a tag
        var newCompound = new NbtCompound();
        foreach (var tag in compound)
        {
            var identifier = Identifier.Parse(tag.Name);
            var newTag = (NbtTag)tag.Clone();
            newTag.Name = identifier.ToCompleteIdentifier().ToString();
            newCompound.Add(newTag);
        }

        return newCompound;
    }
}
