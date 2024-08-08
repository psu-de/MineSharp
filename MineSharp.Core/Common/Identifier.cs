using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using MineSharp.Core.Serialization;

namespace MineSharp.Core.Common;

/// <summary>
/// Represents an identifier with a namespace and a name.
/// This class is immutable.
/// The <see cref="Equals(MineSharp.Core.Common.Identifier?)"/> and <see cref="GetHashCode"/> methods are overridden to compare two identifiers with the following rules:
/// The default namespace is used for the comparison if none is specified.
/// </summary>
public sealed partial record Identifier
{
    /// <summary>
    /// The namespace that is assumed by default when none is specified.
    /// </summary>
    public const string DefaultNamespace = "minecraft";
    /// <summary>
    /// The namespace that is used when no namespace is specified.
    /// </summary>
    public const string NoNamespace = "";

    /// <summary>
    /// Represents an empty identifier.
    /// It has no namespace and an empty name.
    /// </summary>
    public static readonly Identifier Empty = new("");

    /// <summary>
    /// The namespace part of the identifier.
    /// </summary>
    public readonly string Namespace;

    /// <summary>
    /// The name part of the identifier.
    /// </summary>
    public readonly string Name;

    // This constructor is private to prevent creating identifiers with invalid characters
    private Identifier(string @namespace, string name)
    {
        Namespace = @namespace;
        Name = name;
    }

    // This constructor is private to prevent creating identifiers with invalid characters
    private Identifier(string name)
        : this(DefaultNamespace, name)
    {
    }

    /// <summary>
    /// Gets a value indicating whether the identifier has a namespace.
    /// </summary>
    public bool HasNamespace => Namespace != NoNamespace;

    /// <summary>
    /// Gets a value indicating whether the identifier has the default namespace.
    /// </summary>
    public bool HasDefaultNamespace => Namespace == DefaultNamespace;

    /// <summary>
    /// Converts the identifier to a complete identifier with the default namespace if none is specified.
    /// </summary>
    /// <returns>A complete identifier with a namespace.</returns>
    public Identifier ToCompleteIdentifier()
    {
        return HasNamespace ? this : new Identifier(DefaultNamespace, Name);
    }

    /// <summary>
    /// Determines whether the specified <see cref="Identifier"/> is equal to the current <see cref="Identifier"/>.
    /// The comparison uses the default namespace if none is specified.
    /// </summary>
    /// <param name="other">The <see cref="Identifier"/> to compare with the current <see cref="Identifier"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="Identifier"/> is equal to the current <see cref="Identifier"/>; otherwise, <c>false</c>.</returns>
    /// <seealso cref="EqualsStrict(Identifier?)"/>
    public bool Equals(Identifier? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        var @namespace = HasNamespace ? Namespace : DefaultNamespace;
        var otherNamespace = other.HasNamespace ? other.Namespace : DefaultNamespace;
        return @namespace == otherNamespace && Name == other.Name;
    }

    /// <summary>
    /// Determines whether the specified <see cref="Identifier"/> is equal to the current <see cref="Identifier"/>.
    /// In contrast to <see cref="Equals(Identifier?)"/> this comparison is strict and does not use the default namespace if none is specified.
    /// </summary>
    /// <param name="other">The <see cref="Identifier"/> to compare with the current <see cref="Identifier"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="Identifier"/> is equal to the current <see cref="Identifier"/>; otherwise, <c>false</c>.</returns>
    /// <seealso cref="Equals(Identifier?)"/>
    public bool EqualsStrict(Identifier? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Namespace == other.Namespace && Name == other.Name;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var @namespace = HasNamespace ? Namespace : DefaultNamespace;
        return HashCode.Combine(@namespace, Name);
    }

    /// <summary>
    /// Returns a string representation of the identifier.
    /// </summary>
    /// <returns>A string in the format "namespace:name" or "name" if no namespace is specified.</returns>
    public override string ToString()
    {
        return HasNamespace ? $"{Namespace}:{Name}" : Name;
    }

    /// <summary>
    /// Parses a string into an <see cref="Identifier"/> object.
    /// </summary>
    /// <param name="identifierString">The string to parse.</param>
    /// <returns>An <see cref="Identifier"/> object.</returns>
    /// <exception cref="FormatException">Thrown when the string format is invalid.</exception>
    public static Identifier Parse(string identifierString)
    {
        var parseError = TryParseInternal(identifierString, out var identifier);
        if (parseError != null)
        {
            throw new FormatException($"Invalid identifier format: {parseError}");
        }
        return identifier!;
    }

    private static string? TryParseInternal(string identifierString, [NotNullWhen(true)] out Identifier? identifier)
    {
        if (string.IsNullOrEmpty(identifierString))
        {
            identifier = Empty;
            return null;
        }

        var colonIndex = identifierString.IndexOf(':');
        if (colonIndex == -1)
        {
            return TryCreateInternal(NoNamespace, identifierString, out identifier);
        }
        else if (colonIndex == 0 || colonIndex == identifierString.Length - 1)
        {
            identifier = null;
            return "Colon must not be at the very start or end";
        }
        return TryCreateInternal(identifierString.Substring(0, colonIndex), identifierString.Substring(colonIndex + 1), out identifier);
    }

    /// <summary>
    /// Tries to parse a string into an <see cref="Identifier"/> object.
    /// </summary>
    /// <param name="identifierString">The string to parse.</param>
    /// <param name="identifier">The resulting <see cref="Identifier"/> object if parsing is successful.</param>
    /// <returns><c>true</c> if parsing is successful; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string identifierString, [NotNullWhen(true)] out Identifier? identifier)
    {
        return TryParseInternal(identifierString, out identifier) == null;
    }

    private static string? TryCreateInternal(string @namespace, string name, out Identifier? identifier)
    {
        if (!IsValidNamespace(@namespace))
        {
            identifier = null;
            return "Invalid namespace";
        }
        if (!IsValidName(name))
        {
            identifier = null;
            return "Invalid name";
        }
        identifier = new Identifier(@namespace, name);
        return null;
    }

    /// <summary>
    /// Tries to create an <see cref="Identifier"/> object with the specified namespace and name.
    /// </summary>
    /// <param name="namespace">The namespace part of the identifier.</param>
    /// <param name="name">The name part of the identifier.</param>
    /// <param name="identifier">The resulting <see cref="Identifier"/> object if creation is successful.</param>
    /// <returns><c>true</c> if creation is successful; otherwise, <c>false</c>.</returns>
    public static bool TryCreate(string @namespace, string name, [NotNullWhen(true)] out Identifier? identifier)
    {
        var parseError = TryCreateInternal(@namespace, name, out identifier);
        return parseError == null;
    }

    /// <summary>
    /// Creates an <see cref="Identifier"/> object with the specified namespace and name.
    /// </summary>
    /// <param name="namespace">The namespace part of the identifier.</param>
    /// <param name="name">The name part of the identifier.</param>
    /// <returns>An <see cref="Identifier"/> object.</returns>
    /// <exception cref="FormatException">Thrown when the namespace or name is invalid.</exception>
    public static Identifier Create(string @namespace, string name)
    {
        var parseError = TryCreateInternal(@namespace, name, out var identifier);
        if (parseError != null)
        {
            throw new FormatException(parseError);
        }
        return identifier!;
    }

    [GeneratedRegex("[a-z0-9.-_]*")]
    private static partial Regex NamespaceRegex();

    /// <summary>
    /// Determines whether the specified namespace is valid.
    /// </summary>
    /// <param name="namespace">The namespace to validate.</param>
    /// <returns><c>true</c> if the namespace is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidNamespace(string @namespace)
    {
        return NamespaceRegex().MatchEntireString(@namespace) != null;
    }

    [GeneratedRegex("[a-z0-9.-_/]+")]
    private static partial Regex NameRegex();

    /// <summary>
    /// Determines whether the specified name is valid.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <returns><c>true</c> if the name is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidName(string name)
    {
        return NameRegex().MatchEntireString(name) != null;
    }
}
