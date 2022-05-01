using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenerateData.Generators {
    internal class ClassGenerator {

        private string ClassTemplate = @"

%using%

namespace MineSharp.%namespace% {

    public static class %objectName% {

    private static bool isLoaded  = false;

    %classVariables%

        public static void Load () {

            if (isLoaded) return;

            %loader%

            isLoaded = true;
        }

    %register%

    }
}

";

        public string RegisterFunctionBlock = "";
        public List<string> ClassVariables = new List<string>();
        public List<string> UsingStatements = new List<string>();
        public List<string> LoadStatements = new List<string>();
        public string CurrentTemplate { get; set; }

        public ClassGenerator() {
            this.CurrentTemplate = ClassTemplate;
        }

        public ClassGenerator WithRegisterFunction (string functionBlock) {
            this.RegisterFunctionBlock = functionBlock;
            return this;
        }

        public ClassGenerator WithNamespace(string @namespace) {
            this.CurrentTemplate = this.CurrentTemplate.Replace("%namespace%", @namespace);
            return this;
        }

        public ClassGenerator Use(string @namespace) {
            this.UsingStatements.Add($"using ${@namespace};");
            return this;
        }
        
        public ClassGenerator WithName(string name) {
            this.CurrentTemplate = this.CurrentTemplate.Replace("%objectName%", name);
            return this;
        }

        public ClassGenerator AddClassVariable (string expr) {
            this.ClassVariables.Add(expr);
            return this;
        }

        public ClassGenerator AddLoaderExpression(string expr) {
            this.LoadStatements.Add(expr);
            return this;
        }

        public void Write (string outputFile) {

            this.CurrentTemplate = this.CurrentTemplate.Replace("%using%", string.Join("\n", this.UsingStatements));
            this.CurrentTemplate = this.CurrentTemplate.Replace("%classVariables%", string.Join("\n", this.ClassVariables));
            this.CurrentTemplate = this.CurrentTemplate.Replace("%loader%", string.Join("\n", this.LoadStatements));
            this.CurrentTemplate = this.CurrentTemplate.Replace("%register%", this.RegisterFunctionBlock);


            File.WriteAllText(outputFile, this.CurrentTemplate);    
        }

    }
}
