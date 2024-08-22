using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using MineSharp.PacketSourceGenerator.Utils;

namespace MineSharp.PacketSourceGenerator;

public record CommonSymbolHolder(
	Compilation Compilation,

	INamedTypeSymbol PacketBuffer,
	INamedTypeSymbol MinecraftData,
	INamedTypeSymbol PacketType,
	INamedTypeSymbol ProtocolVersion,

	INamedTypeSymbol IPacket,
	INamedTypeSymbol IPacketStatic,
	INamedTypeSymbol IPacketStaticOfTSelf,
	IMethodSymbol IPacketStatic_Read,
	IPropertySymbol IPacketStatic_StaticType,

	INamedTypeSymbol IPacketClientbound,
	INamedTypeSymbol IPacketServerbound,

	INamedTypeSymbol IPacketVersionSubType,
	INamedTypeSymbol IPacketVersionSubTypeOfTBasePacket,
	INamedTypeSymbol IPacketVersionSubTypeStatic,
	INamedTypeSymbol IPacketVersionSubTypeStaticOfTBasePacket,
	INamedTypeSymbol IPacketVersionSubTypeStaticOfTSelfAndTBasePacket,
	IMethodSymbol IPacketVersionSubTypeStatic_Read,

	INamedTypeSymbol PacketVersionSubTypeLookup,

	INamedTypeSymbol ISerializable,
	INamedTypeSymbol ISerializableWithMinecraftData,

	INamedTypeSymbol Object,
	INamedTypeSymbol GeneratedCodeAttribute
)
{
	private static T CheckNotNull<T>(T? symbol, [CallerArgumentExpression(nameof(symbol))] string? symbolArgumentExpression = null)
	{
		if (symbol is null)
		{
			throw new ArgumentNullException(symbolArgumentExpression);
		}
		return symbol;
	}

	public const string PacketNamespace = "MineSharp.Protocol.Packets";
	public const string SerializationNamespace = "MineSharp.Core.Serialization";

	public static CommonSymbolHolder Create(Compilation compilation)
	{
		var PacketType = CheckNotNull(compilation.GetTypeByMetadataName($"MineSharp.Data.Protocol.PacketType"));
		var IPacket = CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.IPacket"));
		var IPacketStatic = CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.IPacketStatic"));
		var IPacketVersionSubTypeStatic = CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.IPacketVersionSubTypeStatic"));

		return new CommonSymbolHolder(
			compilation,

			CheckNotNull(compilation.GetTypeByMetadataName($"{SerializationNamespace}.PacketBuffer")),
			CheckNotNull(compilation.GetTypeByMetadataName($"MineSharp.Data.MinecraftData")),
			PacketType,
			CheckNotNull(compilation.GetTypeByMetadataName($"MineSharp.Core.ProtocolVersion")),

			CheckNotNull(IPacket),
			IPacketStatic,
			CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.IPacketStatic`1")),
			CheckNotNull(GetReadMethodSymbol(IPacketStatic, IPacket)),
			CheckNotNull(GetStaticTypePropertySymbol(IPacketStatic, PacketType)),

			CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.IPacketClientbound")),
			CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.IPacketServerbound")),

			CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.IPacketVersionSubType")),
			CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.IPacketVersionSubType`1")),
			IPacketVersionSubTypeStatic,
			CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.IPacketVersionSubTypeStatic`1")),
			CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.IPacketVersionSubTypeStatic`2")),
			CheckNotNull(GetReadMethodSymbol(IPacketVersionSubTypeStatic, IPacket)),

			CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.PacketVersionSubTypeLookup`1")),

			CheckNotNull(compilation.GetTypeByMetadataName($"{SerializationNamespace}.ISerializable`1")),
			CheckNotNull(compilation.GetTypeByMetadataName($"{PacketNamespace}.NetworkTypes.ISerializableWithMinecraftData`1")),

			CheckNotNull(compilation.GetSpecialType(SpecialType.System_Object)),
			CheckNotNull(compilation.GetTypeByMetadataName(typeof(GeneratedCodeAttribute).FullName))
		);
	}

	public static IMethodSymbol? GetReadMethodSymbol(INamedTypeSymbol type, INamedTypeSymbol packet)
	{
		return type.GetMembers("Read").OfType<IMethodSymbol>().FirstOrDefault(p => SymbolEqualityComparer.IncludeNullability.Equals(p.ReturnType, packet.WithNullable(false)));
	}

	private static IPropertySymbol? GetStaticTypePropertySymbol(INamedTypeSymbol type, INamedTypeSymbol packetType)
	{
		return type.GetMembers("StaticType").OfType<IPropertySymbol>().FirstOrDefault(p => SymbolEqualityComparer.IncludeNullability.Equals(p.Type, packetType.WithNullable(false)));
	}
}
