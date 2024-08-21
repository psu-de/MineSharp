namespace System.Runtime.CompilerServices;

internal class IsExternalInit { }

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class CallerArgumentExpressionAttribute : Attribute
{
	public CallerArgumentExpressionAttribute(string parameterName)
	{
		ParameterName = parameterName;
	}

	public string ParameterName { get; }
}
