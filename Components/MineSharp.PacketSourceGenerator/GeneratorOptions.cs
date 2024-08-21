namespace MineSharp.PacketSourceGenerator;

public readonly struct GeneratorOptions
{
	public readonly string Indent;
	public readonly string NewLine;

	public GeneratorOptions(string indent, string newLine)
	{
		Indent = indent;
		NewLine = newLine;
	}
}
