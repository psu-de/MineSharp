using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Types {
    public class Identifier {

        public const string DefaultNamespace = "minecraft";

        public string Namespace { get; private set; }
        public string Value { get; private set; }

        public Identifier(string identifier) {
            int namespaceSplit = identifier.IndexOf(":");
            if (namespaceSplit == -1) {
                this.Namespace = DefaultNamespace;
                this.Value = identifier;
            } else {
                this.Namespace = identifier.Substring(0, namespaceSplit);
                this.Value = identifier.Substring(namespaceSplit + 1);
            }
        }

        public Identifier(string Namespace, string value) {
            this.Namespace = Namespace;
            this.Value = value;
        }

        public override string ToString() {
            return this.Namespace + ":" + this.Value;
        }

        public static implicit operator string(Identifier identifier) { return identifier.ToString(); }
        public static explicit operator Identifier(string identifier) { return new Identifier(identifier); }
    }
}
