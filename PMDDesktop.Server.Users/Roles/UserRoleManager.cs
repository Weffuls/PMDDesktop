using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace PMDDesktop.Server.Users.Roles;

/// <summary>
/// Manages <see cref="UserRole"/>s. Allows the creation of new <see cref="UserRole"/> objects and handles the saving/loading of them, as well as maintaining the order of roles.
/// </summary>
public sealed class UserRoleManager : IRoleIndexable, IEnumerable<UserRole>
{

	/// <summary>
	/// <para>Creates a new <see cref="UserRoleManager"/> that's paied with <paramref name="userManager"/>.</para>
	/// <para>This <see cref="UserRoleManager"/> will be tightly linked to the given <see cref="UserManager"/></para>
	/// </summary>
	/// <param name="userManager">The <see cref="UserManager"/> to pair with.</param>
	internal UserRoleManager(UserManager userManager)
	{

		Manager = userManager;

		UserRole.CreateDefaultRole().AttachToManager(this);

	}

	/// <summary>
	/// The <see cref="UserManager"/> that's paired with this <see cref="UserRoleManager"/>.
	/// </summary>
	public UserManager Manager { get; private init; }

	/// <summary>
	/// <para>Does this <see cref="UserRoleManager"/> actually write to files?</para>
	/// <para>If this is false, file writes to the <b>filesystem/disk</b> will be skipped.</para>
	/// <para>This property checks <see cref="UserManager.WritingEnabled"/> to get its value.</para>
	/// </summary>
	/// <remarks>
	/// This is property as "false" is very useful in unit testing. Should probably be "true" during runtime.
	/// </remarks>
	public bool WritingEnabled { get => Manager.WritingEnabled; }

	/// <summary>
	/// <para>This is a dictionary with all the <see cref="UserRole"/>s mapped to their <see cref="UserRole.GUID"/>.</para>
	/// </summary>
	/// <remarks>
	/// <para>This dictionary's entries are not managed by the <see cref="UserRoleManager"/>, but instead by the <see cref="UserRole"/>s as they are attached/detached. <b>Do not let <see cref="UserRoleManager"/> write to this dictionary.</b></para>
	/// </remarks>
	internal Dictionary<Guid, UserRole> guids = [];

	/// <summary>
	/// Default <see cref="UserRole"/> applied to all users when checking for <see cref="UserPermission"/>s.
	/// Cannot be added or removed from <see cref="User"/>s.
	/// </summary>
	internal UserRole DefaultRole { get => guids[Guid.Empty]; }

	/// <summary>
	/// <para>Order to check <see cref="guids"/> when evaluating <see cref="UserPermission"/>s based on <see cref="UserRole"/>s.</para>
	/// <para>Should never contain <see cref="DefaultRole"/> (<see cref="Guid.Empty"/>).</para>
	/// </summary>
	internal Guid[] roleOrder = [];

	/// <summary>
	/// Loads from the "roles" directory and "roleOrder.json" file to retrieve saved role data.
	/// </summary>
	/// <returns>Task completes when loading is finished.</returns>
	internal async Task LoadFromFiles()
	{

		await LoadAllRoleData();

		await LoadRoleOrder();

	}

	/// <summary>
	/// This manages creating <see cref="Role"/> objects by loading their data from the "roles" folder.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal async Task LoadAllRoleData()
	{

		string roleRootDirPath = Path.Combine(AppContext.BaseDirectory, "roles");

		if (!Directory.Exists(roleRootDirPath))
			Directory.CreateDirectory(roleRootDirPath);

		foreach (string filePath in Directory.EnumerateFiles(roleRootDirPath))
		{

			string guidParsable = Path.GetFileNameWithoutExtension(filePath);

			if (!Guid.TryParse(guidParsable, out Guid loadedGUID))
				throw new Exception($"Couldn't parse {guidParsable} as a GUID at {filePath}");

			string userFilePath = Path.Join(filePath, User.USER_FILE_NAME);

			using FileStream readStream = File.OpenRead(userFilePath);

			await LoadAndAddRoleJson(readStream, loadedGUID);

		}

	}

	/// <summary>
	/// Internal function for loading a JSON stream and adding the resulting <see cref="UserRole"/> to this <see cref="UserRoleManager"/>.
	/// </summary>
	/// <param name="stream">A JSON stream with <see cref="UserRole"/> data inside.</param>
	/// <returns></returns>
	/// <remarks>
	/// This can be safely unit tested, as it takes in a stream instead of reading from a file.
	/// </remarks>
	internal async Task<UserRole> LoadAndAddRoleJson(Stream stream, Guid guid)
	{

		UserRole deserialized = JsonSerializer.Deserialize<UserRole>(stream, AppInfo.JSON_OPTIONS)
			?? throw new Exception($"Deserialized user role data from {stream} was null.");

		deserialized.GUID = guid; // GUID will be randomized by default, we need to load the previous GUID.

		deserialized.AttachToManager(this);

		return deserialized;

	}

	/// <summary>
	/// Returns the path to the "roleOrder.json" file, intended to be used to save/load the order of roles.
	/// </summary>
	/// <returns>A path poiting to the "roleOrder.json" file.</returns>
	internal static string GetOrderJSONPath()
	{

		return Path.Combine(AppContext.BaseDirectory, "roleOrder.json");

	}

	/// <summary>
	/// Loads the role order from roleOrder.json. Skips if the file doesn't exist; otherwise, the file is also checked for validity and may throw.
	/// </summary>
	/// <returns>Task completes when role order is set or doesn't load.</returns>
	/// <remarks>
	/// Calls <see cref="LoadRoleOrder(Stream)"/> to read the stream.
	/// </remarks>
	[ExcludeFromCodeCoverage]
	private async Task LoadRoleOrder()
	{

		string path = GetOrderJSONPath();

		if (!File.Exists(path))
			return; // No biggie! Just assume default order.

		using Stream stream = File.OpenRead(path);

		await LoadRoleOrder(stream);

	}

	/// <summary>
	/// Loads the role order from the provided stream in Guid[] JSON format. JSON and order is checked for validity and may throw.
	/// </summary>
	/// <returns>Task completes when role order is set.</returns>
	internal async Task LoadRoleOrder(Stream stream)
	{

		Guid[] order = await JsonSerializer.DeserializeAsync<Guid[]>(stream, AppInfo.JSON_OPTIONS)
			?? throw new Exception($"Deserialized user role order from {stream} was null.");

		await SetOrder(order);

	}

	/// <summary>
	/// <para>Attempt to set the order of roles by providing the <see cref="UserRole.GUID"/>s of <b>every</b> <see cref="UserRole"/> in this <see cref="UserRoleManager"/>.</para>
	/// <para><see cref="Guid"/>s should be provided in the order of most important (highest) to least important (lowest). <see cref="UserRole"/>s that are higher (provided first) will take priority over lower roles when they have clashing <see cref="UserPermission"/>s.</para>
	/// <para>If provided, <see cref="DefaultRole"/> (<see cref="Guid.Empty"/>) will be ignored.</para>
	/// <para>Will fail if duplicated GUIDs are provided, or GUIDs that do not correspond to <see cref="UserRole"/>s in this <see cref="UserRoleManager"/>.</para>
	/// </summary>
	/// <param name="guidOrder"></param>
	/// <returns>Task completes once order is set and changes are saved.</returns>
	public async Task SetOrder(IEnumerable<Guid> guidOrder)
	{

		Guid[] orderArray = [.. guidOrder.Where(guid => guid != Guid.Empty)];

		// Each role's GUID should be provided exactly once.
		foreach (UserRole role in guids.Values.Where(role => !role.IsDefaultRole))
			if (orderArray.Count(role.GUID) != 1)
				throw new ArgumentException($"{role}'s GUID ({role.GUID}) was listed {orderArray.Count(role.GUID)} times in the provided order. There needs to be exactly 1 GUID for each role.");

		// All GUIDs should match a role.
		foreach (Guid guid in orderArray)
			if (!guids.ContainsKey(guid))
				throw new ArgumentException($"There is no matching {nameof(UserRole)} in {this} that has GUID {guid}.");

		roleOrder = orderArray;

		if (WritingEnabled)
			await WriteNewRoleOrder();

		return;

	}

	/// <summary>
	/// If <see cref="WritingEnabled"/> is true, will write the current <see cref="roleOrder"/> to "roleOrder.json".
	/// </summary>
	/// <returns></returns>
	public async Task WriteNewRoleOrder()
	{

		if (!WritingEnabled)
			return;

		using FileStream jsonFile = File.Create(GetOrderJSONPath());

		await JsonSerializer.SerializeAsync(jsonFile, roleOrder, AppInfo.JSON_OPTIONS);

	}

	public bool TryGetRole(Guid GUID, [NotNullWhen(true)] out UserRole? role)
	{
		return guids.TryGetValue(GUID, out role);
	}

	/// <summary>
	/// Try to create a new <see cref="UserRole"/>, will return null if it could not be done. It will have default settings and no permissions set.
	/// </summary>
	/// <returns>The created <see cref="UserRole"/>, or null if it failed.</returns>
	public async Task<UserRole?> TryCreateRole()
	{

		UserRole role = new();

		role.AttachToManager(this);

		await role.WriteNewData();

		await WriteNewRoleOrder();

		return role;

	}

	public IEnumerator<UserRole> GetEnumerator()
	{

		foreach (Guid guid in roleOrder)
			yield return guids[guid];

		yield return DefaultRole;

	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

}
