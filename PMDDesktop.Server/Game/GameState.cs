using PMDDesktop.Server.Assets;
using PMDDesktop.Server.Saving;
using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Server.Game;

public class GameState : IAssetIndexable, ISaveDataIndexable
{

	[ExcludeFromCodeCoverage]
	public static async Task<GameState> CreateAndLoadFiles()
	{

		GameState state = new();

		await state.LoadAllFiles();

		return state;

	}

	[ExcludeFromCodeCoverage]
	public async Task LoadAllFiles()
	{

		await Saves.LoadFromFilesAndEnableWriting();
		await Assets.LoadFromFiles();

	}

	public T GetAsset<T>(AssetLocation location) where T : Asset
	{
		return Assets.GetAsset<T>(location);
	}

	public bool TryGetAsset<T>(AssetLocation location, [NotNullWhen(true)] out T? asset) where T : Asset
	{
		return Assets.TryGetAsset<T>(location, out asset);
	}

	public T? GetSave<T>(Guid GUID) where T : SaveData
	{
		return Saves.GetSave<T>(GUID);
	}

	public bool TryGetSave<T>(Guid GUID, [NotNullWhen(true)] out T? data) where T : SaveData
	{
		return Saves.TryGetSave(GUID, out data);
	}

	public SaveDataManager Saves { get; } = new();
	public AssetManager Assets { get; } = new();

}
