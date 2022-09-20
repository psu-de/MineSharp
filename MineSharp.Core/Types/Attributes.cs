using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Types
{
    public class Attribute
    {
        public string Key { get; set; }
        public double Base { get; set; }
        public Dictionary<UUID, Modifier> Modifiers { get; set; }
        public double Value => Modifiers.GroupBy(m => m.Value.Operation)
                                        .OrderBy(x => x.Key)
                                        .Aggregate(Base, (x, t) => {
                                            var op = t.Key;
                                            var modifiers = t.Select(x => x.Value);
                                            return op switch {
                                                0 => modifiers.Aggregate(x, (y, t) => y += t.Amount),
                                                1 => x * (1 + modifiers.Select(x => x.Amount).Sum()),
                                                2 => modifiers.Aggregate(x, (y, t) => y *= 1 + t.Amount),
                                                _ => throw new NotSupportedException($"Modifier operation {op} not supported")
                                            };
                                        });

        public Attribute(string key, double @base, List<Modifier> modifiers)
        {
            this.Key = key;
            this.Base = @base;
            this.Modifiers = modifiers.ToDictionary(x => x.UUID);
        }
    }

    public struct Modifier
    {
        public UUID UUID { get; set; }
        public double Amount { get; set; }
        public byte Operation { get; set; }

        public Modifier(UUID uuid, double amount, byte operation)
        {
            this.UUID = uuid;
            this.Amount = amount;
            this.Operation = operation;
        }
    }
}
