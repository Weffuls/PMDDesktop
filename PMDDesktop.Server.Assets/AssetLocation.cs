using PMDDesktop.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets;

/// <summary>
/// <para>A struct for holding the location of an asset on the server side.</para>
/// <para>Works like a relative file path, but starts at the 'assets' folder.</para>
/// <para>Does not include the filename as part of the path.</para>
/// <para>Locations should be unique for each asset, AssetManager will throw if they're not.</para>
/// </summary>
public struct AssetLocation
{

	static AssetLocation()
	{

		IEnumerable<char> allInvalid = [.. Path.GetInvalidFileNameChars()];

		IEnumerable<char> invalidNoSplitter = allInvalid.Where((character) => character != SPLIT_CHAR);

		INVALID_CHARS = [.. invalidNoSplitter];

	}

	private static readonly char[] INVALID_CHARS;
	private static readonly char SPLIT_CHAR = '/';

	/// <summary>
	/// WARNING: LAZILY IMPLEMENTED!!!
	/// </summary>
	/// <param name="path">The filepath of the asset.</param>
	/// <returns>An AssetLocation pointing to the asset at this path.</returns>
	public static AssetLocation LocationFromPath(string path)
	{

		path = Path.GetDirectoryName(path) ?? throw new Exception("yeah this function sucks.");

		// TODO: this has literally NO safety checks.

		string full = Path.GetRelativePath(GetAssetsDirectory(), path);

		string[] parts = full.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar]);

		return new(parts);

	}

	/// <summary>
	/// Create an AssetLocation pointing to the root of the assets folder.
	/// </summary>
	/// <remarks>
	/// It is okay for an asset to be at the assets folder root.
	/// </remarks>
	public AssetLocation()
	{
		String = string.Empty;
	}

	/// <summary>
	/// Create an asset location pointing to the following directories. Using a '/' to seperate directories in a single string is also okay!
	/// </summary>
	/// <param name="directories">Array </param>
	public AssetLocation(params string[] directories)
	{
		String = string.Join(SPLIT_CHAR, directories);
	}

	[JsonInclude]
	private string String { get; set => field = CheckLocationString(value); }

	public static string GetAssetsDirectory()
	{

		return Path.Join(AppContext.BaseDirectory, "assets");

	}

	public readonly string GetDirectory()
	{

		return Path.Join([GetAssetsDirectory(), .. GetComponents()]);

	}

	internal readonly string GetFilePath(Asset asset)
	{

		Type type = asset.GetType();

		AssetFileNameAttribute? fileNameAttribute = type.GetCustomAttribute<AssetFileNameAttribute>()
			?? throw new MissingAttributeException(type, typeof(AssetFileNameAttribute));

		string fileName = fileNameAttribute.FileName;

		return Path.Join([GetDirectory(), fileName + ".json"]);

	}

	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{

		if (obj is AssetLocation location)
			return String == location.String;

		return base.Equals(obj);

	}

	public static bool operator ==(AssetLocation left, AssetLocation right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(AssetLocation left, AssetLocation right)
	{
		return left.Equals(right);
	}

	private static string CheckLocationString(string input)
	{

		foreach (char invalid in INVALID_CHARS)
			if (input.Contains(invalid))
				throw new FormatException($"Asset Location \"{input}\" cannot contain {invalid}");

		if (input.StartsWith(SPLIT_CHAR))
			throw new FormatException($"Asset Location '{input}' cannot start with a {SPLIT_CHAR}");

		if (input.EndsWith(SPLIT_CHAR))
			throw new FormatException($"Asset Location '{input}' cannot end with a {SPLIT_CHAR}");

		if (input.Contains("//"))
			throw new FormatException($"Asset Location '{input}' cannot contain two back-to-back slashes. (There must be a directory name)");

		return input;

	}

	public readonly string[] GetComponents()
	{

		return String.Split(SPLIT_CHAR);

	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(String);
	}

	public override readonly string ToString()
	{
		return String;
	}

	/// <summary>
	/// Cast an AssetLocation to a string.
	/// </summary>
	/// <param name="location">The location to cast to string.</param>
	public static implicit operator string(AssetLocation location)
	{

		return location.String;

	}

}
