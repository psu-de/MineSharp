using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using MineSharp.PacketSourceGenerator.Protocol;
using MineSharp.PacketSourceGenerator.Utils;

namespace MineSharp.PacketSourceGenerator.Generators;

public sealed class BasePacketGenerator : AbstractPacketGenerator
{
	public readonly IReadOnlyList<INamedTypeSymbol> VersionSubTypes;

	public BasePacketGenerator(GeneratorArguments args, IReadOnlyList<INamedTypeSymbol> versionSubTypes)
		: base(args)
	{
		VersionSubTypes = versionSubTypes;
	}

	public override string? BuildBodyForType()
	{
		var packetFlowInterfaceType = Args.NamespaceParseResult.PacketFlow switch
		{
			PacketFlow.Serverbound => Args.CommonSymbolHolder.IPacketServerbound,
			PacketFlow.Clientbound => Args.CommonSymbolHolder.IPacketClientbound,
			_ => throw new NotImplementedException()
		};
		var packetFlowInterfaceTypeString = BuildTypeName(packetFlowInterfaceType);

		var extendsString = $": {packetFlowInterfaceTypeString}";
		List<string> methods = new();

		if (VersionSubTypes.Count > 0)
		{
			var packetVersionSubTypeLookupType = Args.CommonSymbolHolder.PacketVersionSubTypeLookup.Construct(Args.TypeSymbol);
			var packetVersionSubTypeLookupTypeString = BuildTypeName(packetVersionSubTypeLookupType);

			methods.Add(BuildPacketVersionSubTypeLookupField(packetVersionSubTypeLookupTypeString));
			methods.Add(BuildInitializeVersionPacketsMethod(packetVersionSubTypeLookupTypeString));

			methods.Add(BuildPacketVersionSubTypeWriteMethod());
			// ISerializableWithMinecraftData<> non explicit
			methods.Add(BuildPacketVersionSubTypeReadMethod());
		}

		// IPacketStatic<>.Read
		methods.Add(BuildPacketReadMethod(Args.CommonSymbolHolder.IPacketStatic_Read));

		return BuildTypeSource(methods, extendsString);
	}

	private string BuildPacketVersionSubTypeLookupField(string packetVersionSubTypeLookupTypeString)
	{
		return $$"""
			{{GeneratedCodeAttributeDeclaration}}
			public static readonly {{packetVersionSubTypeLookupTypeString}} PacketVersionSubTypeLookup = InitializeVersionPackets();
			""";
	}

	private string BuildInitializeVersionPacketsMethod(string packetVersionSubTypeLookupTypeString)
	{
		List<string> registerVersionPacketCalls = new();
		foreach (var versionSubType in VersionSubTypes)
		{
			var versionSubTypeString = BuildTypeName(versionSubType);
			var call = $"lookup.RegisterVersionPacket<{versionSubTypeString}>();";
			registerVersionPacketCalls.Add(call);
		}

		return $$"""
			{{GeneratedCodeAttributeDeclaration}}
			private static {{packetVersionSubTypeLookupTypeString}} InitializeVersionPackets()
			{
				{{packetVersionSubTypeLookupTypeString}} lookup = new();

				{{registerVersionPacketCalls.Join(Args.GeneratorOptions.NewLine).IndentLines(indentString: Args.GeneratorOptions.Indent, ignoreFirstLine: true)}}

				lookup.Freeze();
				return lookup;
			}
			""";
	}

	private string BuildPacketVersionSubTypeWriteMethod()
	{
		var packetBufferType = Args.CommonSymbolHolder.PacketBuffer;
		var minecraftDataType = Args.CommonSymbolHolder.MinecraftData;
		var packetBufferTypeString = BuildTypeName(packetBufferType);
		var minecraftDataTypeString = BuildTypeName(minecraftDataType);

		return $$"""
			{{InheritDocComment}}
			{{GeneratedCodeAttributeDeclaration}}
			public abstract void Write({{packetBufferTypeString}} buffer, {{minecraftDataTypeString}} data);
			""";
	}

	private string BuildPacketVersionSubTypeReadMethod()
	{
		var returnTypeString = BuildTypeName(Args.TypeSymbol);

		var packetBufferType = Args.CommonSymbolHolder.PacketBuffer;
		var minecraftDataType = Args.CommonSymbolHolder.MinecraftData;
		var packetBufferTypeString = BuildTypeName(packetBufferType);
		var minecraftDataTypeString = BuildTypeName(minecraftDataType);

		return $$"""
			{{InheritDocComment}}
			{{GeneratedCodeAttributeDeclaration}}
			public static {{returnTypeString}} Read({{packetBufferTypeString}} buffer, {{minecraftDataTypeString}} data)
			{
				return PacketVersionSubTypeLookup.Read(buffer, data);
			}
			""";
	}
}
