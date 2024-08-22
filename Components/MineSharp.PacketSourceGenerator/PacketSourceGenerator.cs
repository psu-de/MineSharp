using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MineSharp.PacketSourceGenerator.Generators;
using MineSharp.PacketSourceGenerator.Utils;

namespace MineSharp.PacketSourceGenerator;

public class PacketSourceGenerator
{
	public readonly CommonSymbolHolder CommonSymbolHolder;
	public readonly GeneratorOptions GeneratorOptions;
	public readonly CancellationToken CancellationToken;

	public readonly HashSet<INamedTypeSymbol> KnownValidPacketTypes = new(SymbolEqualityComparer.Default);
	// base packet type -> list of subtypes
	public readonly Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> KnownValidPacketVersionSubTypes = new(SymbolEqualityComparer.Default);

	public PacketSourceGenerator(CommonSymbolHolder commonSymbolHolder, GeneratorOptions generatorOptions, CancellationToken cancellationToken)
	{
		CommonSymbolHolder = commonSymbolHolder;
		GeneratorOptions = generatorOptions;
		CancellationToken = cancellationToken;
	}

	private (INamedTypeSymbol BasePacketType, TaskCompletionSource<IReadOnlyList<INamedTypeSymbol>> Tcs)? _lastBasePacketGeneratorArgumentsTcsBundle;

	private void CompleteLastBasePacketGeneratorArgumentsTcsBundle()
	{
		if (_lastBasePacketGeneratorArgumentsTcsBundle is not null)
		{
			var (basePacketType, tcs) = _lastBasePacketGeneratorArgumentsTcsBundle.Value;
			if (!KnownValidPacketVersionSubTypes.TryGetValue(basePacketType, out var versionSubTypes))
			{
				// If the dictionary doesn't contain the base packet type, we use an empty list
				versionSubTypes = new();
			}
			tcs.TrySetResult(versionSubTypes);
		}
		_lastBasePacketGeneratorArgumentsTcsBundle = null;
	}

	/// <summary>
	/// IMPORTANT: This method must be called after all packetNamespaceType are processed.
	/// </summary>
	public void ProcessPacketNamespaceTypeComplete()
	{
		CompleteLastBasePacketGeneratorArgumentsTcsBundle();
	}

	/// <summary>
	/// IMPORTANT: This method must not be run concurrently.
	/// If this method is called multiple time that the packetNamespaceType are in root to leaf order.
	/// Meaning the subtype packet must be processed after the base packet.
	/// </summary>
	public async Task<Task<GeneratedSourceFileInfo?>> ProcessPacketNamespaceTypeAsync(INamedTypeSymbol packetNamespaceType, PacketValidator.PacketNamespaceParseResult namespaceParseResult)
	{
		var checkPacketNamespaceArguments = new PacketValidator.CheckPacketNamespaceArguments(CommonSymbolHolder, packetNamespaceType, namespaceParseResult, CancellationToken);

		if (packetNamespaceType.ContainingType is null)
		{
			CompleteLastBasePacketGeneratorArgumentsTcsBundle();

			// All types that are not nested should be valid packet types
			// So we check them here

			var (packetTypeParseResult, errors) = await PacketValidator.CheckValidPacketType(checkPacketNamespaceArguments);
			if (errors.Length > 0 || packetTypeParseResult is null)
			{
				ProcessCheckErrors(packetNamespaceType, errors);
				return Task.FromResult<GeneratedSourceFileInfo?>(null);
			}

			KnownValidPacketTypes.Add(packetNamespaceType);
			var tcs = new TaskCompletionSource<IReadOnlyList<INamedTypeSymbol>>(TaskCreationOptions.RunContinuationsAsynchronously);
			_lastBasePacketGeneratorArgumentsTcsBundle = (packetNamespaceType, tcs);
			return CreateGeneratorTask(checkPacketNamespaceArguments, async args => new BasePacketGenerator(args, await tcs.Task));
		}
		else
		{
			// Nested types are either version specific subtypes of the packet ...
			var baseType = packetNamespaceType.BaseType;
			if (baseType is not null && KnownValidPacketTypes.Contains(baseType))
			{
				var errors = await PacketValidator.CheckValidPacketSubType(checkPacketNamespaceArguments);
				if (errors.Length > 0)
				{
					ProcessCheckErrors(packetNamespaceType, errors);
					return Task.FromResult<GeneratedSourceFileInfo?>(null);
				}

				AddPacketSubType(baseType, packetNamespaceType);

				return CreateGeneratorTask(checkPacketNamespaceArguments, args => Task.FromResult(new SubTypePacketGenerator(args)));
			}
			else
			{
				// ... or they are data container types
				var errors = await PacketValidator.CheckValidDataContainerType(checkPacketNamespaceArguments);
				if (errors.Length > 0)
				{
					ProcessCheckErrors(packetNamespaceType, errors);
					return Task.FromResult<GeneratedSourceFileInfo?>(null);
				}
			}
		}

		return Task.FromResult<GeneratedSourceFileInfo?>(null);
	}

	private void AddPacketSubType(INamedTypeSymbol baseType, INamedTypeSymbol subType)
	{
		if (!KnownValidPacketVersionSubTypes.TryGetValue(baseType, out var subTypes))
		{
			subTypes = new();
			KnownValidPacketVersionSubTypes.Add(baseType, subTypes);
		}

		subTypes.Add(subType);
	}

	private Task<GeneratedSourceFileInfo?> CreateGeneratorTask<TGenerator>(PacketValidator.CheckPacketNamespaceArguments checkPacketNamespaceArguments, Func<GeneratorArguments, Task<TGenerator>> generatorFactory)
		where TGenerator : AbstractPacketGenerator
	{
		var type = checkPacketNamespaceArguments.PacketNamespaceType;
		var generatorArguments = new GeneratorArguments(CommonSymbolHolder, type, checkPacketNamespaceArguments.NamespaceParseResult, GeneratorOptions, CancellationToken);
		return Task.Run(async () => (await generatorFactory(generatorArguments)).GenerateFile(), CancellationToken);
	}

	// TODO: throw RaiseDiagnosticsException with fancy Diagnostics instead of logging errors
	private void ProcessCheckErrors(INamedTypeSymbol type, ImmutableArray<string> errors)
	{
		if (errors.Length > 0)
		{
			Debugger.Log(0, "Type Error", $"Error for type '{type.ToDisplayString()}': {errors.Join(GeneratorOptions.NewLine)}{GeneratorOptions.NewLine}");
		}
	}
}
