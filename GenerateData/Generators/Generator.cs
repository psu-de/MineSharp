using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenerateData.Generators {
    internal class Generator {
        public static string MakeCSharpSafe(string str) {

            Regex rgx = new Regex(@"^\d+");
            Match match = rgx.Match(str);
            if (match.Success) {
                str = str.Substring(match.Value.Length);
                str += match.Value;
            }

            rgx = new Regex("[^a-zA-Z0-9 -]");
            str = rgx.Replace(str, "");
            return str;
        }

        public static string Uppercase(string input) {
            return input[0].ToString().ToUpper() + input.Substring(1);
        }
    }
}
