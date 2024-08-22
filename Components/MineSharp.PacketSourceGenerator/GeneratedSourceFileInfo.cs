using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace MineSharp.PacketSourceGenerator;

public sealed record GeneratedSourceFileInfo(string Name, string Content)
{
	private static readonly ImmutableArray<char> s_disallowedFileChars = ImmutableArray.Create('<', '>', ':', '"', '/', '\\', '|', '?', '*', // not allowed in file names
		','); // not nice to have in file names

	public static string EscapeFileName(string fileName)
	{
		return s_disallowedFileChars.Aggregate(new StringBuilder(fileName), (s, c) => s.Replace(c, '_')).ToString();
	}

	public static string BuildFileName(INamedTypeSymbol type)
	{
		var symbolDisplayString = type.ToDisplayString();
		var fileName = $"{EscapeFileName(symbolDisplayString)}.{SourceGenerator.SourceGeneratorShortName}.g.cs";
		return fileName;
	}
}
