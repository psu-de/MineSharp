using System.Threading;
using Microsoft.CodeAnalysis;

namespace MineSharp.PacketSourceGenerator.Generators;

public record GeneratorArguments(
	CommonSymbolHolder CommonSymbolHolder,
	INamedTypeSymbol TypeSymbol,
	PacketValidator.PacketNamespaceParseResult NamespaceParseResult,
	GeneratorOptions GeneratorOptions,
	CancellationToken CancellationToken
);
