using System.Text.RegularExpressions;

namespace MineSharp.Core.Serialization;

/// <summary>
/// Provides helper methods for working with regular expressions.
/// </summary>
public static class RegexHelper
{
    /// <summary>
    /// Matches the entire input string against the regular expression.
    /// </summary>
    /// <param name="regex">The regular expression to match against.</param>
    /// <param name="input">The input string to match.</param>
    /// <returns>A <see cref="Match"/> object if the entire input string matches the regular expression; otherwise, <c>null</c>.</returns>
    public static Match? MatchEntireString(this Regex regex, string input)
    {
        var match = regex.Match(input);
        if (match.Success && match.Value.Length == input.Length)
        {
            return match;
        }
        return null;
    }
}
