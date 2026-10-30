using System.Diagnostics.CodeAnalysis;
using PMDDesktop.Requests.Roles;
using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Assets;
using PMDDesktop.Server.Saving;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers;

internal static class CommonResponseHandlerChecks
{

	/// <summary>
	/// <para>Try to authenticate a <paramref name="user"/>, and authorize them against certain permissions.</para>
	/// <para>To pass, the <paramref name="user"/> needs to both be not null, and have all the <see cref="UserPermission"/>s in <paramref name="permissionsNeeded"/>.</para>
	/// </summary>
	/// <param name="user">The <see cref="User"/> to check the authentication and authorization of.</param>
	/// <param name="permissionsNeeded">A list of all permissions needed to be fully authorized.</param>
	/// <param name="error">A <see cref="ResponseResultAuthenticationError"/> or <see cref="ResponseResultPermissionError"/> describing why authentication/authorization failed, to be returned.</param>
	/// <returns>True if authenticated and authorized; if false, <paramref name="error"/> contains the reason the authentication/authorization failed.</returns>
	internal static bool TryAuthorizeUser([NotNullWhen(true)] User? user, IEnumerable<UserPermission> permissionsNeeded, [NotNullWhen(false)] out ResponseResultError? error)
	{

		if (user is null)
		{
			error = new ResponseResultAuthenticationError(null);
			return false;
		}

		IEnumerable<UserPermission> missingPermissions = permissionsNeeded.Where(permission => !user.HasPermission(permission));

		if (missingPermissions.Any())
		{
			error = new ResponseResultPermissionError(missingPermissions);
			return false;
		}

		error = null;
		return true;

	}

	internal static bool TryGetUser(IUserIndexable indexable, Guid guid, [NotNullWhen(true)] out User? user, [NotNullWhen(false)] out ResponseResultError? error)
	{

		if (indexable.TryGetUser(guid, out user))
		{
			error = null;
			return true;
		}

		error = new ResponseResultIndexMissingError<User>(guid.ToString());
		return false;

	}

	internal static bool TryGetRole(IRoleIndexable indexable, Guid guid, [NotNullWhen(true)] out UserRole? role, [NotNullWhen(false)] out ResponseResultError? error)
	{

		if (indexable.TryGetRole(guid, out role))
		{
			error = null;
			return true;
		}

		error = new ResponseResultIndexMissingError<UserRole>(guid.ToString());
		return false;

	}

	internal static bool TryGetPermission(string dataName, [NotNullWhen(true)] out UserPermission? permission, [NotNullWhen(false)] out ResponseResultError? error)
	{

		if (UserPermission.TryGetPermissionByDataName(dataName, out permission))
		{
			error = null;
			return true;
		}

		error = new ResponseResultIndexMissingError<UserPermission>(dataName);
		return false;

	}

	internal static bool TryGetSaveData<TData>(ISaveDataIndexable indexable, Guid guid, [NotNullWhen(true)] out TData? data, [NotNullWhen(false)] out ResponseResultError? error) where TData : SaveData
	{

		if (indexable.TryGetSave(guid, out data))
		{
			error = null;
			return true;
		}

		error = new ResponseResultIndexMissingError<TData>(guid.ToString());
		return false;

	}

	internal static bool TryGetAsset<TAsset>(IAssetIndexable indexable, AssetLocation location, [NotNullWhen(true)] out TAsset? asset, [NotNullWhen(false)] out ResponseResultError? error) where TAsset : Asset
	{

		if (indexable.TryGetAsset(location, out asset))
		{
			error = null;
			return true;
		}

		error = new ResponseResultIndexMissingError<TAsset>(location);
		return false;

	}

	internal static bool TryCompareHierarchy(User requestingUser, IUserHierarchyComparable expectedLower, [NotNullWhen(false)] out ResponseResultError? error)
	{

		if (!requestingUser.HigherThan(expectedLower))
		{
			error = new ResponseResultHierarchyError(requestingUser, expectedLower);
			return false;
		}

		error = null;
		return true;

	}

}
