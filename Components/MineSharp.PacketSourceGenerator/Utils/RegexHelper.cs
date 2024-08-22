using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MineSharp.PacketSourceGenerator.Utils;

public static class RegexHelper
{
	public static readonly Regex NewLineRegex = new(@"\r\n|\n|\r", RegexOptions.Compiled);

	public static Match? MatchEntireString(this Regex regex, string input)
	{
		var match = regex.Match(input);
		if (match.Success && match.Value.Length == input.Length)
		{
			return match;
		}
		return null;
	}

	/// <summary>
	/// This method tries to get a group with the name from <paramref name="groupName"/> and then tries to lookup that value in the <paramref name="valueLookup"/> dictionary.
	/// If the regex <paramref name="match"/> does not contain a group with that name then <see langword="null"/> will be returned.
	/// If the <paramref name="valueLookup"/> does not contain a value for the group's value then a exception will be thrown.
	/// </summary>
	/// <returns>The looked up value from the group's value or <see langword="null"/> if the group was not matched.</returns>
	public static TValue? GetRegexGroupAsValue<TValue>(this Match match, string groupName, IReadOnlyDictionary<string, TValue> valueLookup)
		where TValue : unmanaged
	{
		var group = match.Groups[groupName];
		return group.Success ? valueLookup[group.Value] : null;
	}

	public static TValue? GetRegexGroupAsValue<TValue>(this Match match, IReadOnlyDictionary<string, TValue> valueLookup)
		where TValue : unmanaged
	{
		return GetRegexGroupAsValue(match, typeof(TValue).Name, valueLookup);
	}

	public static string BuildRegexPatternForMultipleValues(IEnumerable<string> values)
	{
		var sb = new StringBuilder();
		foreach (var value in values)
		{
			var regexPattern = Regex.Escape(value);
			sb.Append(regexPattern);
			sb.Append('|');
		}

		if (sb.Length > 0)
		{
			sb.Length--; // Remove the last '|'
		}

		return sb.ToString();
	}

	public static string BuildRegexCaptureGroupPatternForType<TValue>(IEnumerable<string> values)
		where TValue : unmanaged
	{
		var valuesPattern = BuildRegexPatternForMultipleValues(values);

		return $"(?<{typeof(TValue).Name}>{valuesPattern})";
	}

	public static string BuildRegexCaptureGroupPatternForLookup<TValue>(IReadOnlyDictionary<string, TValue> valueLookup)
		where TValue : unmanaged
	{
		return BuildRegexCaptureGroupPatternForType<TValue>(valueLookup.Keys);
	}

}
