using System;
using System.Collections.Generic;

namespace MineSharp.PacketSourceGenerator.Generators;

public sealed class SubTypePacketGenerator : AbstractPacketGenerator
{
	public SubTypePacketGenerator(GeneratorArguments args)
		: base(args)
	{
	}

	public override string? BuildBodyForType()
	{
		var extendsString = "";
		List<string> methods = new();

		// FirstVersionUsed and FirstVersionUsedStatic
		methods.Add(BuildFirstVersionUsedProperties());

		// IPacketVersionSubTypeStatic.Read
		methods.Add(BuildPacketReadMethod(Args.CommonSymbolHolder.IPacketVersionSubTypeStatic_Read));
		// IPacketVersionSubTypeStatic<>.Read
		var basePacketType = Args.TypeSymbol.BaseType!; // must not be null otherwise we shouldn't be here
		var genericIPacketVersionSubTypeStatic = Args.CommonSymbolHolder.IPacketVersionSubTypeStaticOfTBasePacket.Construct(basePacketType);
		var genericIPacketVersionSubTypeStatic_Read = CommonSymbolHolder.GetReadMethodSymbol(genericIPacketVersionSubTypeStatic, basePacketType);
		methods.Add(BuildPacketReadMethod(genericIPacketVersionSubTypeStatic_Read ?? throw new InvalidOperationException($"Read method not found for {genericIPacketVersionSubTypeStatic}")));

		return BuildTypeSource(methods, extendsString);
	}

	private string BuildFirstVersionUsedProperties()
	{
		var protocolVersionTypeString = BuildTypeName(Args.CommonSymbolHolder.ProtocolVersion);
		var parseResult = PacketValidator.ParsePacketSubType(Args.TypeSymbol.Name);
		// parseResult must not be null otherwise we shouldn't be here
		var protocolVersionString = parseResult!.Value.MinecraftVersion.ToString();

		return $$"""
			{{InheritDocComment}}
			{{GeneratedCodeAttributeDeclaration}}
			public {{protocolVersionTypeString}} FirstVersionUsed => FirstVersionUsedStatic;
			{{InheritDocComment}}
			{{GeneratedCodeAttributeDeclaration}}
			public static {{protocolVersionTypeString}} FirstVersionUsedStatic => {{protocolVersionTypeString}}.{{protocolVersionString}};
			""";
	}

}
