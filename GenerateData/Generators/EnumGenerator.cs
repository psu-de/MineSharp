using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenerateData.Generators {
    internal class EnumGenerator {

        private string EnumTemplate = @"
namespace MineSharp.%namespace% {

    public enum %objectName% {

        %enumValues%

    }
}
";


        public List<string> EnumValues = new List<string>();
        public string CurrentTemplate { get; set; }

        public EnumGenerator() {
            this.CurrentTemplate = EnumTemplate;
        }



        public EnumGenerator WithNamespace(string @namespace) {
            this.CurrentTemplate = this.CurrentTemplate.Replace("%namespace%", @namespace);
            return this;
        }

        public EnumGenerator EnumAddValue(string name, int? id = null) {
            name = Generator.MakeCSharpSafe(name);
            if (id == null) {
                this.EnumValues.Add(name);
            } else {
                this.EnumValues.Add($"{name} = {id}");
            }
            return this;
        }
        
        public EnumGenerator WithName(string name) {
            this.CurrentTemplate = this.CurrentTemplate.Replace("%objectName%", name);
            return this;
        }

        public void Write (string outputFile) { 
            this.CurrentTemplate = this.CurrentTemplate.Replace("%enumValues%", string.Join(",\n", this.EnumValues));

            File.WriteAllText(outputFile, this.CurrentTemplate);    
        }

    }
}
