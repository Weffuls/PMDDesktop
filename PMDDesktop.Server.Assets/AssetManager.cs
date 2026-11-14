using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using PMDDesktop.Exceptions;
using PMDDesktop.Utils;

namespace PMDDesktop.Server.Assets;

public class AssetManager : IEnumerable<Asset>, IAssetIndexable
{

	public AssetManager()
	{

		SetAssetFileTypes();

	}

	/// <summary>
	/// This lists all asset file names with their corresponding Types.
	/// </summary>
	/// <remarks>
	/// Only available after Initialize() is called.
	/// </remarks>
	private readonly Dictionary<string, Type> assetFileTypes = [];

	/// <summary>
	/// This lists all known assets. Assets can be accessed with Get<T>() and AssetReferences.
	/// </summary>
	private readonly Dictionary<DataIDPath, Asset> allAssets = [];

	public IEnumerable<KeyValuePair<string, Type>> EnumerateAssetTypes()
	{

		return assetFileTypes;

	}

	private void SetAssetFileTypes()
	{

		if (assetFileTypes.Count > 0)
			throw new InvalidOperationException("assetFileTypes already has data in it and SetAssetFileTypes() was called again!");

		// This should find all classes implementing "Asset."
		IEnumerable<Type> assetTypes = TypeUtils.GetInstanceableClassesAssignableTo(typeof(Asset));

		foreach (Type type in assetTypes)
		{

			AssetFileNameAttribute fileNameAttribute = type.GetCustomAttribute<AssetFileNameAttribute>()
			?? throw new MissingAttributeException(type, typeof(AssetFileNameAttribute));

			string key = fileNameAttribute.FileName;

			if (assetFileTypes.TryGetValue(key, out Type? overlappingType))
				throw new DuplicateAttributeDataException(type, overlappingType, key, typeof(AssetFileNameAttribute));

			assetFileTypes.Add(key, type);

		}

	}

	internal void Add(Asset asset)
	{

		if (asset.Manager != null)
		{
			if (asset.Manager == this)
				throw new InvalidOperationException($"{asset} is already assigned to {asset.Manager}, the same manager it's trying to be added to.");
			else
				throw new InvalidOperationException($"{asset} already has Manager {asset.Manager} assigned to it.");
		}

		if (allAssets.TryGetValue(asset.Path, out Asset? blockingAsset))
		{

			if (blockingAsset == asset)
				throw new InvalidOperationException($"{blockingAsset} is already in the AssetManager!");
			else
				throw new InvalidOperationException($"Unable to add {asset}, which has the same Location as {blockingAsset}, which is already added in the AssetManager!");

		}

		allAssets.Add(asset.Path, asset);

		asset.Manager = this;

		Console.WriteLine($"Added Asset ({asset.GetType().Name}): {asset}");

	}

	public async Task WriteAllAssets()
	{

		foreach (Asset asset in allAssets.Values)
			await WriteAsset(asset);

	}

	private static async Task WriteAsset(Asset asset)
	{

		string dirPath = GetAssetDirectory(asset.Path);

		if (!Directory.Exists(dirPath))
			Directory.CreateDirectory(dirPath);

		string filePath = GetAssetFilePath(asset);
		Type type = asset.GetType();

		using FileStream stream = File.Create(filePath);

		await JsonSerializer.SerializeAsync(stream, asset, type, AppInfo.JSON_OPTIONS);

		Console.WriteLine($"Wrote {asset} of type {type.FullName} to {filePath}");

		return;

	}

	public T GetAsset<T>(DataIDPath location) where T : Asset
	{

		if (TryGetAsset(location, out T? asset))
			return asset;

		if (TryGetAsset(location, out Asset? genericAsset))
			throw new InvalidCastException($"Asset found at {location} was of type {genericAsset.GetType().FullName} and not {typeof(T).FullName}");

		throw new KeyNotFoundException($"No asset was found for {location}.");

	}

	public bool TryGetAsset<T>(DataIDPath path, [NotNullWhen(true)] out T? asset) where T : Asset
	{

		allAssets.TryGetValue(path, out Asset? gotAsset);

		asset = gotAsset as T;

		return asset is not null;

	}

	public async Task LoadFromFiles()
	{

		await LoadAllAssets();

	}

	private async Task LoadAllAssets()
	{

		Dictionary<string, DataID> IDs = [];

		foreach (string filePath in Directory.EnumerateDirectories(GetAssetDirectory()))
		{

			string directory = Path.GetFileName(filePath);

			// Skip "hidden" directories!!
			if (directory.StartsWith('.'))
				continue;

			DataID node = (DataID)directory;

			await LoadAllAssetsRecursive(new(node), IDs);

		}

	}

	private async Task LoadAssetAtIfExists(DataIDPath currentPath)
	{

		string[] files = [.. Directory.GetFiles(GetAssetDirectory(currentPath), "*.json").Where(path => !path.StartsWith('.'))];

		if (files.Length == 0)
			return;

		if (files.Length != 1)
			throw new Exception($"There can only be one .json file in {currentPath}");

		string filePath = files[0];

		string name = Path.GetFileNameWithoutExtension(filePath).ToLowerInvariant();

		if (!assetFileTypes.TryGetValue(name, out Type? importType))
			throw new Exception($"Couldn't find {name} as an asset type for {filePath}.");

		using FileStream readStream = File.OpenRead(filePath);

		object deserialized = JsonSerializer.Deserialize(readStream, importType, AppInfo.JSON_OPTIONS)
			?? throw new Exception($"Deserialized asset data from {filePath} was null.");

		if (deserialized is not Asset asset)
			throw new InvalidCastException($"{deserialized} couldn't be cast to Asset");

		asset.Path = currentPath;

		Add(asset);

	}

	private async Task LoadAllAssetsRecursive(DataIDPath currentPath, Dictionary<string, DataID> IDs)
	{

		await LoadAssetAtIfExists(currentPath);

		foreach (string filePath in Directory.EnumerateDirectories(GetAssetDirectory(currentPath)))
		{

			string directory = Path.GetFileName(filePath);

			// Skip "hidden" directories!!
			if (directory.StartsWith('.'))
				continue;

			DataID node = (DataID)directory;

			await LoadAllAssetsRecursive(currentPath + node, IDs);

		}

	}

	internal static string GetAssetDirectory()
	{

		return Path.Join(AppContext.BaseDirectory, "assets");

	}

	internal static string GetAssetDirectory(DataIDPath path)
	{

		return Path.Join([GetAssetDirectory(), (string)path]);

	}

	internal static string GetAssetFilePath(Asset asset)
	{

		Type type = asset.GetType();

		AssetFileNameAttribute? fileNameAttribute = type.GetCustomAttribute<AssetFileNameAttribute>()
			?? throw new MissingAttributeException(type, typeof(AssetFileNameAttribute));

		string fileName = fileNameAttribute.FileName;

		return Path.Join([GetAssetDirectory(asset.Path), fileName + ".json"]);

	}

	IEnumerator<Asset> IEnumerable<Asset>.GetEnumerator()
	{
		return allAssets.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return allAssets.Values.GetEnumerator();
	}

}
