using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Requests;

[AttributeUsage(AttributeTargets.Class)]
public class ServerRequestAttribute : Attribute
{

	internal ServerRequestAttribute([StringSyntax("Route")] string path) : base()
	{

		Path = path;

	}

	public string Path { get; init; }

}
