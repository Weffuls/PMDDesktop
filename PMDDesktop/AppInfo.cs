using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop;

public static class AppInfo
{

	/// <summary>
	/// Global static JSON options for serialization and deserialization.
	/// </summary>
	public static readonly JsonSerializerOptions JSON_OPTIONS = new()
	{

		// Preference imo.
		// Could be removed, but it makes it easier for people to tinker with save data, and doesn't use that much extra space.
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		// End of formatting block.

		ReadCommentHandling = JsonCommentHandling.Skip

	};

	/// <summary>
	/// Global static JSON options for serialization and deserialization over the network.
	/// </summary>
	public static readonly JsonSerializerOptions NETWORK_JSON_OPTIONS = new()
	{

		RespectRequiredConstructorParameters = true,
		RespectNullableAnnotations = true,
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow

	};

	/// <summary>
	/// <para>Current version of the app.</para>
	///
	/// <para>Major numbers are breaking-changes and won't inter-op with the client of a different major version.</para>
	/// <para>Minor numbers are non-breaking changes, and represent improvements to the client or server that doesn't require new network protocols.</para>
	/// <para>Build numbers are for bug-fixes.</para>
	/// 
	/// <para>Due to the nature of this program, major number increments will likely be frequent.</para>
	/// </summary>
	public static readonly Version VERSION = new(0, 0, 0);

}
