using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using MineSharp.PacketSourceGenerator.Utils;

namespace MineSharp.PacketSourceGenerator;

[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{
	public const string SourceGeneratorShortName = "PacketSourceGenerator";
	public const string SourceGeneratorName = $"MineSharp.{SourceGeneratorShortName}";
	public const string SourceGeneratorVersionString = "1.0.0.0";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		DebugHelper.LaunchDebugger();

		// using the Compilation deep in the pipeline is bad for performance
		// because every changed character will trigger a full run of the generator
		// but we need the compilation by using it with dynamic values
		// (it might be possible to precompute all possible values that might be used on the compilation. But this is for a later optimization phase)
		context.RegisterSourceOutput(context.CompilationProvider, static (spc, compilation) =>
		{
			var generatorOptions = new GeneratorOptions(GeneratorHelper.DefaultIndent, GeneratorHelper.SourceCodeNewLine);

			Execute(spc, compilation, generatorOptions).Wait();
		});
	}

	private static IEnumerable<(INamedTypeSymbol Type, PacketValidator.PacketNamespaceParseResult NamespaceParseResult)> GetAllTypesInPacketNamespace(CommonSymbolHolder symbolHolder)
	{
		var globalNamespace = symbolHolder.Compilation.SourceModule.GlobalNamespace;
		var typeMembers = globalNamespace.GetAllNamedTypeSymbols(false);

		// filter out all types that are not in the packet namespace
		foreach (var type in typeMembers)
		{
			var typeNamespace = type.ContainingNamespace;
			var namespaceName = typeNamespace.ToDisplayString();
			var packetNamespaceParseResult = PacketValidator.ParsePacketNamespace(namespaceName);
			if (packetNamespaceParseResult is null)
			{
				continue;
			}
			yield return (type, packetNamespaceParseResult.Value);
		}
	}

	private static async Task Execute(SourceProductionContext productionContext, Compilation compilation, GeneratorOptions generatorOptions)
	{
		var symbolHolder = CommonSymbolHolder.Create(compilation);

		var packetSourceGenerator = new PacketSourceGenerator(symbolHolder, generatorOptions, productionContext.CancellationToken);

		List<(Task<GeneratedSourceFileInfo?> SourceTask, INamedTypeSymbol Type)> sourceTasks = new();

		foreach (var (type, namespaceParseResult) in GetAllTypesInPacketNamespace(symbolHolder))
		{
			productionContext.CancellationToken.ThrowIfCancellationRequested();
			await HandleGeneratorActionErrors(productionContext, type, async () =>
			{
				var sourceTask = await packetSourceGenerator.ProcessPacketNamespaceTypeAsync(type, namespaceParseResult);
				sourceTasks.Add((sourceTask, type));
			});
		}
		packetSourceGenerator.ProcessPacketNamespaceTypeComplete();

		await foreach (var tuple in TaskHelper.AsTheyComplete(sourceTasks, t => t.SourceTask))
		{
			productionContext.CancellationToken.ThrowIfCancellationRequested();
			await HandleGeneratorActionErrors(productionContext, tuple.Type, async () =>
			{
				var type = tuple.Type;
				var source = await tuple.SourceTask;
				if (source is not null)
				{
					productionContext.AddSource(source.Name, source.Content);
				}
			});
		}
	}

	private static async Task HandleGeneratorActionErrors(SourceProductionContext productionContext, INamedTypeSymbol type, Func<Task> action)
	{
		try
		{
			await action();
		}
		catch (RaiseDiagnosticsException e)
		{
			foreach (var diagnostic in e.Diagnostics)
			{
				productionContext.ReportDiagnostic(diagnostic);
			}
		}
		catch (Exception e)
		{
			var symbolDisplayString = type.ToDisplayString();
			Debugger.Log(0, "Source Generator Error", $"Error while processing type '{symbolDisplayString}': {e}{GeneratorHelper.SourceCodeNewLine}");
		}
	}
}
