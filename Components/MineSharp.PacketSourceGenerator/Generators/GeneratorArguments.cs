using System.Threading;
using Microsoft.CodeAnalysis;

namespace MineSharp.PacketSourceGenerator.Generators;

public record GeneratorArguments(
	CommonSymbolHolder CommonSymbolHolder,
	INamedTypeSymbol TypeSymbol,
	GeneratorOptions GeneratorOptions,
	CancellationToken CancellationToken
);
