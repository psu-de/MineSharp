using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Bot {
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class BotFunctionAttribute : Attribute {

        public string Category;
        public string Description;
        public BotFunctionAttribute(string category, string description) {

            this.Category = category;
            this.Description = description;
        }
    }
}
