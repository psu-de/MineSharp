using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace MineSharp.PacketSourceGenerator.Utils;

public static class DebugHelper
{
	/// <summary>
	/// Can be disabled to use short names in the generated code.
	/// Disabling this will make the generated code more readable, but may cause conflicts with other types.
	/// Because of this, it is for debugging purposes only.
	/// </summary>
	public static readonly bool UseFullyQualifiedNames = true;

#pragma warning disable RS1035 // Do not used banned APIs
	[Conditional("DEBUG")]
	public static void LaunchDebugger([CallerFilePath] string? sourceFilePath = null)
	{
		// required to debug the source generator

		// this check filters whether the source gen is called from the IDE or from the compiler
		// compiler = 4
		// IDE = 6 (or higher)
		if (Environment.Version.Major == 4
			&& DoesDebugFileExist(sourceFilePath))
		{
			Debugger.Launch();
		}
	}

	private const string DebugFileName = $"debugSourceGenerator.txt.user";

	private static bool DoesDebugFileExist(string? sourceFilePath)
	{
		if (sourceFilePath is null)
		{
			return false;
		}

		var debugFilePath = Path.Combine(Path.GetDirectoryName(sourceFilePath), DebugFileName);
		if (debugFilePath is null)
		{
			return false;
		}
		return File.Exists(debugFilePath);
	}
#pragma warning restore RS1035
}
