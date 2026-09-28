using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Server.Users.Roles;

public sealed class UserRoleManager : IRoleIndexable, IEnumerable<UserRole>
{

	internal UserRoleManager(UserManager userManager)
	{

		Manager = userManager;

		UserRole.CreateDefaultRole().AttachToManager(this);

	}

	public UserManager Manager { get; private init; }

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
	/// <para>Attempt to set the order of roles by providing the <see cref="UserRole.GUID"/>s of <b>every</b> <see cref="UserRole"/> in this <see cref="UserRoleManager"/>.</para>
	/// <para><see cref="Guid"/>s should be provided in the order of most important (highest) to least important (lowest). <see cref="UserRole"/>s that are higher (provided first) will take priority over lower roles when they have clashing <see cref="UserPermission"/>s.</para>
	/// <para>If provided, <see cref="DefaultRole"/> (<see cref="Guid.Empty"/>) will be ignored.</para>
	/// <para>Will fail if duplicated GUIDs are provided, or GUIDs that do not correspond to <see cref="UserRole"/>s in this <see cref="UserRoleManager"/>.</para>
	/// </summary>
	/// <param name="guidOrder"></param>
	/// <returns>Task resolves once order is set and changes are saved.</returns>
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

		return;

	}

	public bool TryGetRole(Guid GUID, [NotNullWhen(true)] out UserRole? role)
	{
		return guids.TryGetValue(GUID, out role);
	}

	public async Task<UserRole?> TryCreateRole()
	{

		UserRole role = new();

		role.AttachToManager(this);

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
