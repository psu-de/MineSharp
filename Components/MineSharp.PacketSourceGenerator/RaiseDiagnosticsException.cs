using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace MineSharp.PacketSourceGenerator;

public class RaiseDiagnosticsException : Exception
{
	public readonly ImmutableArray<Diagnostic> Diagnostics;

	public RaiseDiagnosticsException(ImmutableArray<Diagnostic> diagnostics)
		: base($"{diagnostics.Length} diagnostics were raised.")
	{
		Diagnostics = diagnostics;
	}

	public RaiseDiagnosticsException(Diagnostic diagnostic)
		: this(ImmutableArray.Create(diagnostic))
	{
	}
}
