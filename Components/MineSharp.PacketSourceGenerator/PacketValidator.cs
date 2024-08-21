using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MineSharp.PacketSourceGenerator.Protocol;
using MineSharp.PacketSourceGenerator.Utils;

namespace MineSharp.PacketSourceGenerator;

public static class PacketValidator
{
	public static readonly Regex PacketNamespaceRegex;
	public static readonly Regex PacketTypeRegex;
	public static readonly Regex PacketNameRegex;
	public static readonly Regex PacketSubTypeNameRegex;
	public static readonly Regex EnumHelperClassNameRegex;

	static PacketValidator()
	{
		PacketNamespaceRegex = new($@"^{Regex.Escape(CommonSymbolHolder.PacketNamespace)}\.{RegexHelper.BuildRegexCaptureGroupPatternForLookup(PacketFlowHelper.ByNameLookup)}\.{RegexHelper.BuildRegexCaptureGroupPatternForLookup(GameStateHelper.ByNameLookup)}$", RegexOptions.Compiled);
		PacketTypeRegex = new($@"^{RegexHelper.BuildRegexCaptureGroupPatternForLookup(PacketFlowHelper.ByShortNameLookup)}_{RegexHelper.BuildRegexCaptureGroupPatternForLookup(GameStateHelper.ByNameLookup)}_(?<{nameof(PacketTypeParseResult.PacketTypeName)}>\w+)$", RegexOptions.Compiled);
		var packetNamePattern = $@"(?<PacketName>(?<PacketBaseName>\w+)Packet)";
		PacketNameRegex = new($@"^{packetNamePattern}$", RegexOptions.Compiled);
		PacketSubTypeNameRegex = new($@"^{packetNamePattern}(?<MinecraftVersion>V_(?<Major>\d+)_(?<Minor>\d+)_(?<Patch>\d+))$", RegexOptions.Compiled);
		EnumHelperClassNameRegex = new($@"^(?<EnumName>\w+)(?<HelperNameSuffix>Helper|Extensions)$", RegexOptions.Compiled);
	}

	public record struct PacketNamespaceParseResult(PacketFlow PacketFlow, GameState GameState);

	public static PacketNamespaceParseResult? ParsePacketNamespace(string packetNamespace)
	{
		var match = PacketNamespaceRegex.Match(packetNamespace);
		if (!match.Success)
		{
			return null;
		}

		// We can safely assume that the groups are always present because of the regex pattern
		var packetFlow = match.GetRegexGroupAsValue(PacketFlowHelper.ByNameLookup)!.Value;
		var gameState = match.GetRegexGroupAsValue(GameStateHelper.ByNameLookup)!.Value;
		return new(packetFlow, gameState);
	}

	public record struct PacketTypeParseResult(PacketFlow PacketFlow, GameState GameState, string PacketTypeName);

	public static PacketTypeParseResult? ParsePacketType(string packetType)
	{
		var match = PacketTypeRegex.Match(packetType);
		if (!match.Success)
		{
			return null;
		}

		// We can safely assume that the groups are always present because of the regex pattern
		var packetFlow = match.GetRegexGroupAsValue(PacketFlowHelper.ByShortNameLookup)!.Value;
		var gameState = match.GetRegexGroupAsValue(GameStateHelper.ByNameLookup)!.Value;
		var packetTypeName = match.Groups[nameof(PacketTypeParseResult.PacketTypeName)].Value;
		return new(packetFlow, gameState, packetTypeName);
	}

	public record struct PacketSubTypeVersion(ushort Major, ushort Minor, ushort Patch)
	{
		public override string ToString()
		{
			return $"V_{Major}_{Minor}_{Patch}";
		}
	}
	public record struct PacketSubTypeParseResult(string PacketBaseName, PacketSubTypeVersion MinecraftVersion);

	public static PacketSubTypeParseResult? ParsePacketSubType(string packetSubTypeName)
	{
		var match = PacketSubTypeNameRegex.Match(packetSubTypeName);
		if (!match.Success)
		{
			return null;
		}

		// We can safely assume that the groups are always present because of the regex pattern
		var packetBaseName = match.Groups[nameof(PacketSubTypeParseResult.PacketBaseName)].Value;
		var major = match.Groups[nameof(PacketSubTypeVersion.Major)].Value;
		var minor = match.Groups[nameof(PacketSubTypeVersion.Minor)].Value;
		var patch = match.Groups[nameof(PacketSubTypeVersion.Patch)].Value;
		var minecraftVersion = new PacketSubTypeVersion(ushort.Parse(major), ushort.Parse(minor), ushort.Parse(patch));
		return new(packetBaseName, minecraftVersion);
	}


	private static async Task<string?> GetPacketTypeFromSymbol(ISymbol implementingSymbol, CancellationToken cancellationToken)
	{
		foreach (var syntaxReference in implementingSymbol.DeclaringSyntaxReferences)
		{
			var syntax = await syntaxReference.GetSyntaxAsync(cancellationToken);
			var arrowExprSyntax = syntax.DescendantNodes().OfType<ArrowExpressionClauseSyntax>().FirstOrDefault();
			if (arrowExprSyntax is not null && arrowExprSyntax.Expression is MemberAccessExpressionSyntax expression
				&& expression.Expression is IdentifierNameSyntax leftIdentifierSyntax
				&& expression.Name is IdentifierNameSyntax rightIdentifierSyntax)
			{
				return rightIdentifierSyntax.Identifier.ToString();
			}
		}
		return null;
	}

	public sealed record CheckPacketNamespaceArguments(CommonSymbolHolder SymbolHolder, INamedTypeSymbol PacketNamespaceType, PacketNamespaceParseResult NamespaceParseResult, CancellationToken CancellationToken);

	public static async Task<(PacketTypeParseResult? PacketTypeParseResult, ImmutableArray<string> Errors)> CheckValidPacketType(CheckPacketNamespaceArguments arguments)
	{
		var (symbolHolder, packetType, namespaceParseResult, cancellationToken) = arguments;

		List<string> errorMessages = new();
		PacketTypeParseResult? packetTypeParseResult = null;

		if (PacketNameRegex.MatchEntireString(packetType.Name) is null)
		{
			errorMessages.Add($"The packet type '{packetType}' does not match the expected packet name pattern");
		}

		if (packetType.DeclaredAccessibility != Accessibility.Public)
		{
			errorMessages.Add($"The packet type '{packetType}' must be public");
		}

		if (!packetType.IsAbstract && !packetType.IsSealed)
		{
			errorMessages.Add($"The packet type '{packetType}' must be either abstract or sealed");
		}

		var expectedPacketInterfaceType = symbolHolder.IPacketStaticOfTSelf.Construct(packetType);
		if (!packetType.AllInterfaces.Contains(expectedPacketInterfaceType, SymbolEqualityComparer.Default))
		{
			errorMessages.Add($"The packet type '{packetType}' does not implement the expected interface '{expectedPacketInterfaceType}'");
		}

		var implementingSymbol = packetType.FindImplementationForInterfaceMember(symbolHolder.IPacketStatic_StaticType);
		if (implementingSymbol is null)
		{
			errorMessages.Add($"The packet type '{packetType}' does not implement the static property '{symbolHolder.IPacketStatic_StaticType.ToDisplayString()}'");
		}
		else
		{
			var packetTypeName = await GetPacketTypeFromSymbol(implementingSymbol, cancellationToken);
			packetTypeParseResult = ParsePacketType(packetTypeName ?? "");
			if (packetTypeParseResult is null)
			{
				errorMessages.Add($"The packet type '{packetType}' does not return a valid value for the static property '{symbolHolder.IPacketStatic_StaticType.ToDisplayString()}'");
			}
			else
			{
				if (packetTypeParseResult.Value.PacketFlow != namespaceParseResult.PacketFlow)
				{
					errorMessages.Add($"The packet type '{packetType}' has a PacketType value with different packet flow than the packet namespace '{namespaceParseResult.PacketFlow}'");
				}
				if (packetTypeParseResult.Value.GameState != namespaceParseResult.GameState)
				{
					errorMessages.Add($"The packet type '{packetType}' has a PacketType value with different game state than the packet namespace '{namespaceParseResult.GameState}'");
				}
			}
		}
		return (packetTypeParseResult, errorMessages.ToImmutableArray());
	}

	// caller must only pass in packet sub types that have a valid packet type as base type
	public static Task<ImmutableArray<string>> CheckValidPacketSubType(CheckPacketNamespaceArguments arguments)
	{
		var (symbolHolder, packetSubType, namespaceParseResult, cancellationToken) = arguments;

		List<string> errorMessages = new();

		if (PacketSubTypeNameRegex.MatchEntireString(packetSubType.Name) is null)
		{
			errorMessages.Add($"The packet sub type '{packetSubType}' does not match the expected packet sub type name pattern");
		}

		if (packetSubType.DeclaredAccessibility != Accessibility.Public)
		{
			errorMessages.Add($"The packet sub type '{packetSubType}' must be public");
		}

		if (!packetSubType.IsSealed)
		{
			errorMessages.Add($"The packet sub type '{packetSubType}' must be sealed");
		}

		// we do not check for the interface IPacketVersionSubTypeStatic<,> because we generate the implementation

		return Task.FromResult(errorMessages.ToImmutableArray());
	}

	public static Task<ImmutableArray<string>> CheckValidDataContainerType(CheckPacketNamespaceArguments arguments)
	{
		var (symbolHolder, dataContainerType, namespaceParseResult, cancellationToken) = arguments;

		var basePacketType = dataContainerType.ContainingType;

		bool HasISerializableInterface()
		{
			var expectedInterface = symbolHolder.ISerializable.Construct(dataContainerType);
			return dataContainerType.AllInterfaces.Contains(expectedInterface, SymbolEqualityComparer.Default);
		}
		bool HasISerializableWithMinecraftDataInterface()
		{
			var expectedInterface = symbolHolder.ISerializableWithMinecraftData.Construct(dataContainerType);
			return dataContainerType.AllInterfaces.Contains(expectedInterface, SymbolEqualityComparer.Default);
		}
		bool IsRegistryClass()
		{
			return dataContainerType.TypeKind == TypeKind.Class && dataContainerType.IsStatic && dataContainerType.Name.EndsWith("Registry");
		}
		bool IsEnumHelperClass()
		{
			if (!(dataContainerType.TypeKind == TypeKind.Class && dataContainerType.IsStatic))
			{
				return false;
			}

			var match = EnumHelperClassNameRegex.Match(dataContainerType.Name);
			if (!match.Success)
			{
				return false;
			}
			var enumName = match.Groups["EnumName"].Value;
			return basePacketType.GetTypeMembers(enumName).Any(t => t.TypeKind == TypeKind.Enum);
		}

		List<string> errorMessages = new();

		var allowedDataContainerType = dataContainerType.TypeKind == TypeKind.Enum
			|| dataContainerType.TypeKind == TypeKind.Interface
			|| IsRegistryClass()
			|| IsEnumHelperClass()
			|| HasISerializableInterface() || HasISerializableWithMinecraftDataInterface();

		if (!allowedDataContainerType)
		{
			errorMessages.Add($"The sub type '{dataContainerType}' is not valid as data container type. It must be an enum, interface, static class ending with 'Registry', static helper class for an enum or a class/record implementing either {symbolHolder.ISerializable} or {symbolHolder.ISerializableWithMinecraftData}");
		}

		if (dataContainerType.DeclaredAccessibility != Accessibility.Public)
		{
			errorMessages.Add($"The sub type '{dataContainerType}' must be public");
		}

		return Task.FromResult(errorMessages.ToImmutableArray());
	}
}
