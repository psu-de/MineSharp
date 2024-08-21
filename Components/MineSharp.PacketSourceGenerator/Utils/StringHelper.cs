using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace MineSharp.PacketSourceGenerator.Utils;

public static class StringHelper
{
	public static readonly ImmutableArray<string> NewLineStrings = ImmutableArray.Create("\r\n", "\r", "\n");

	public static string ReplaceLineEndings(this string text, string newLineEndings)
	{
		return RegexHelper.NewLineRegex.Replace(text, match => newLineEndings);
	}

	public static string Join(this IEnumerable<string> enumerable, string separator, bool addSeparatorAtEnd = false, bool addSeparatorAtStart = false)
	{
		var s = string.Join(separator, enumerable);
		if (s.Length > 0)
		{
			if (addSeparatorAtStart)
			{
				s = separator + s;
			}
			if (addSeparatorAtEnd && s.Length > 0)
			{
				s += separator;
			}
		}
		return s;
	}

	public static string Repeat(this string input, int count)
	{
		if (string.IsNullOrEmpty(input) || count == 1)
		{
			return input;
		}

		if (count == 0)
		{
			return "";
		}

		var builder = new StringBuilder(input.Length * count);

		for (var i = 0; i < count; i++)
		{
			builder.Append(input);
		}

		return builder.ToString();
	}

	private static readonly char[] s_defaultEmptyLineAllowedChars = new char[] { ' ', '\t' };

	public static string AddBeforeEachLine(this string str, string linePrefix, bool ignoreEmptyLines, bool ignoreFirstLine = false, Regex? newLineRegex = null, Regex? emptyLineContentRegex = null)
	{
		if (string.IsNullOrEmpty(str))
		{
			return str;
		}

		newLineRegex ??= RegexHelper.NewLineRegex;

		bool CheckIgnoreLine(int nextLineStartIndex, Match nextNewLineMatch)
		{
			if (ignoreEmptyLines)
			{
				var nextLineEndIndex = nextNewLineMatch.Success ? nextNewLineMatch.Index : str.Length;
				var nextLineStr = str.Substring(nextLineStartIndex, nextLineEndIndex - nextLineStartIndex);

				var isEmptyLine = false;
				if (emptyLineContentRegex is null)
				{
					isEmptyLine = nextLineStr.AsSpan().ContainsOnlyAllowedItems(s_defaultEmptyLineAllowedChars);
				}
				else
				{
					isEmptyLine = emptyLineContentRegex.MatchEntireString(nextLineStr)?.Success ?? false;
				}

				return isEmptyLine;
			}
			return false;
		}

		var retStr = newLineRegex.Replace(str, (match) =>
		{
			var nextNewLineMatch = match.NextMatch();
			var nextLineStartIndex = match.Index + match.Length;
			var ignoreLine = CheckIgnoreLine(nextLineStartIndex, nextNewLineMatch);
			return ignoreLine ? match.Value : match.Value + linePrefix;
		});

		if (!ignoreFirstLine)
		{
			var firstMatch = newLineRegex.Match(str);
			var ignoreLine = CheckIgnoreLine(0, firstMatch);
			if (!ignoreLine)
			{
				retStr = linePrefix + retStr;
			}
		}

		return retStr;
	}
}
