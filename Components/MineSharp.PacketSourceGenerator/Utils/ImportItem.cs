using System;

namespace MineSharp.PacketSourceGenerator.Utils;

public record struct ImportItem(string Namespace, bool Static) : IComparable<ImportItem>
{
	public int CompareTo(ImportItem other)
	{
		var staticInt = Static ? 1 : 0;
		var staticIntOther = other.Static ? 1 : 0;
		var staticIntResult = staticInt.CompareTo(staticIntOther);
		if (staticIntResult != 0)
		{
			return staticIntResult;
		}
		return Namespace.CompareTo(other.Namespace);
	}

	public override readonly string ToString()
	{
		return $"using {(Static ? "static " : "")}{Namespace};";
	}
}
