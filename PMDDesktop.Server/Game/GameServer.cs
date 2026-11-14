using System.Diagnostics.CodeAnalysis;
using PMDDesktop.Server.Assets;
using PMDDesktop.Server.Saving;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.Game;

public class GameServer : IAssetIndexable, ISaveDataIndexable, IUserIndexable, IRoleIndexable
{

	public GameServer()
	{

		shutdown = new(TaskCreationOptions.RunContinuationsAsynchronously);

		WebApplicationCreator.PrepareWebApplication(this);

	}

	[ExcludeFromCodeCoverage]
	public static async Task<GameServer> CreateAndLoadFiles()
	{

		GameServer server = new();

		await server.State.LoadAllFiles();

		return server;

	}

	private readonly TaskCompletionSource shutdown;
	public GameState State { get; } = new();
	public WebApplication WebApp { get; init; } = WebApplicationCreator.CreateWebApplication();

	public async Task<int> Run()
	{

		await WebApp.RunAsync();

		// And now we shut down.
		return await GracefulShutdown();

	}

	private async Task<int> GracefulShutdown()
	{

		Console.WriteLine("Saving then closing.");

		await State.Saves.SaveAllChanges();

		Console.WriteLine("All Good! Closing!");
		return 0;

	}

	public T GetAsset<T>(DataIDPath path) where T : Asset
	{
		return State.GetAsset<T>(path);
	}

	public bool TryGetAsset<T>(DataIDPath path, [NotNullWhen(true)] out T? asset) where T : Asset
	{
		return State.TryGetAsset(path, out asset);
	}

	public T? GetSave<T>(Guid GUID) where T : SaveData
	{
		return State.GetSave<T>(GUID);
	}

	public bool TryGetSave<T>(Guid GUID, [NotNullWhen(true)] out T? data) where T : SaveData
	{
		return State.TryGetSave(GUID, out data);
	}

	public bool TryGetUser(Guid GUID, [NotNullWhen(true)] out User? user)
	{
		return State.TryGetUser(GUID, out user);
	}

	public bool TryGetRole(Guid GUID, [NotNullWhen(true)] out UserRole? role)
	{
		return ((IRoleIndexable)State).TryGetRole(GUID, out role);
	}

}
