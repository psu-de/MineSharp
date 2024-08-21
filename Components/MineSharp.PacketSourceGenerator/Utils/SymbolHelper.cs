using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MineSharp.PacketSourceGenerator.Utils;

public static class SymbolHelper
{
	private static readonly SymbolDisplayFormat s_qualifiedNameOnlyFormat = new(
			globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
			typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

	public static readonly SymbolDisplayFormat FullyQualifiedFormatWithoutGlobalPrefix =
		SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

	public static string ToFqn(this ISymbol symbol, bool addGlobalPrefix = true)
	{
		var format = SymbolDisplayFormat.FullyQualifiedFormat;
		format = format.WithGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters)
			.WithGlobalNamespaceStyle(addGlobalPrefix ? SymbolDisplayGlobalNamespaceStyle.Included : SymbolDisplayGlobalNamespaceStyle.Omitted);

		return symbol.ToDisplayString(format);
	}

	public static ImportItem? GetFqnPath(this ISymbol symbol, bool addGlobalPrefix = true)
	{
		var format = s_qualifiedNameOnlyFormat
			.WithGlobalNamespaceStyle(addGlobalPrefix ? SymbolDisplayGlobalNamespaceStyle.Included : SymbolDisplayGlobalNamespaceStyle.Omitted);

		var parts = symbol.ToDisplayParts(format).Select(x => x.ToString()).ToList();
		var lastIndexOfDot = parts.LastIndexOf(".");
		if (lastIndexOfDot == -1)
		{
			return null;
		}
		var path = parts.Take(lastIndexOfDot).Join("");

		var namespaceName = symbol.ContainingNamespace.ToDisplayString(format);
		return new(path, path != namespaceName);
	}

	public static INamedTypeSymbol? GetNullableBaseType(this INamedTypeSymbol symbol)
	{
		if (symbol.IsGenericType && symbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
		{
			return symbol.TypeArguments[0] as INamedTypeSymbol;
		}

		return null;
	}

	public static ITypeSymbol WithNullable(this ITypeSymbol typeSymbol, bool nullable)
	{
		var nullableAnnotation = nullable ? NullableAnnotation.Annotated : NullableAnnotation.NotAnnotated;
		return typeSymbol.WithNullableAnnotation(nullableAnnotation);
	}

	public static ITypeSymbol WithNullable(this ITypeSymbol typeSymbol, bool nullable, bool globalUseNullableReferenceTypes)
	{
		var nullableAnnotation = !globalUseNullableReferenceTypes ? NullableAnnotation.None : nullable ? NullableAnnotation.Annotated : NullableAnnotation.NotAnnotated;
		return typeSymbol.WithNullableAnnotation(nullableAnnotation);
	}

	public static string GetTypeKeywordFromSymbol(this ITypeSymbol typeSymbol)
	{
		var typeDeclarationSyntax = typeSymbol.DeclaringSyntaxReferences
			.Select(x => x.GetSyntax())
			.OfType<TypeDeclarationSyntax>()
			.First();

		if (typeDeclarationSyntax.IsKind(SyntaxKind.RecordStructDeclaration))
		{
			return "record struct";
		}

		return typeDeclarationSyntax.Keyword.ValueText;
	}

	public static IEnumerable<INamedTypeSymbol> GetAllNamedTypeSymbols(this INamespaceOrTypeSymbol startSymbol, bool includeSelf)
	{
		var stack = new Stack<INamespaceOrTypeSymbol>();
		stack.Push(startSymbol);

		if (includeSelf && startSymbol is INamedTypeSymbol startTypeSymbol)
		{
			yield return startTypeSymbol;
		}

		while (stack.Count > 0)
		{
			var symbol = stack.Pop();

			foreach (var member in symbol.GetMembers())
			{
				if (member is INamespaceSymbol memberAsNamespace)
				{
					stack.Push(memberAsNamespace);
				}
				else if (member is INamedTypeSymbol memberAsNamedTypeSymbol)
				{
					stack.Push(memberAsNamedTypeSymbol);
					yield return memberAsNamedTypeSymbol;
				}
			}
		}
	}

	public static IEnumerable<INamespaceOrTypeSymbol> GetContainingNamespaceAndTypes(this ISymbol symbol, bool includeSelf)
	{
		foreach (var item in symbol.GetAllContainingNamespacesAndTypes(includeSelf))
		{
			yield return item;

			if (item.IsNamespace)
			{
				yield break;
			}
		}
	}

	public static IEnumerable<INamespaceOrTypeSymbol> GetAllContainingNamespacesAndTypes(this ISymbol symbol, bool includeSelf)
	{
		if (includeSelf && symbol is INamespaceOrTypeSymbol self)
		{
			yield return self;
		}

		while (true)
		{
			symbol = symbol.ContainingSymbol;

			if (symbol is not INamespaceOrTypeSymbol namespaceOrTypeSymbol)
			{
				yield break;
			}

			yield return namespaceOrTypeSymbol;
		}
	}
}
