using System.Diagnostics.CodeAnalysis;
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

	/// <summary>
	/// <para>Helper function to call <see cref="IUserIndexable.TryGetUser(Guid, out User?)"/>, but additionally output a <see cref="ResponseResultError"/> if no <see cref="User"/> is found.</para>
	/// <para>Intended to be used in senarios where a response cannot continue if the <see cref="User"/> cannot be found.</para>
	/// </summary>
	/// <param name="indexable">A <see cref="IUserIndexable"/> to get the <see cref="User"/> from.</param>
	/// <param name="guid">The <see cref="Guid"/> of the <see cref="User"/> you're trying to find.</param>
	/// <param name="user">The <see cref="User"/>, if one was found. </param>
	/// <param name="error">An <see cref="ResponseResultError"/>, if no <see cref="User"/> was found.</param>
	/// <returns>True if <paramref name="user"/> is not null; False if <paramref name="error"/> is not null.</returns>
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

	/// <summary>
	/// <para>Helper function to call <see cref="IRoleIndexable.TryGetRole(Guid, out UserRole?)"/>, but additionally output a <see cref="ResponseResultError"/> if no <see cref="UserRole"/> is found.</para>
	/// <para>Intended to be used in senarios where a response cannot continue if the <see cref="UserRole"/> cannot be found.</para>
	/// </summary>
	/// <param name="indexable">A <see cref="IUserIndexable"/> to get the <see cref="UserRole"/> from.</param>
	/// <param name="guid">The <see cref="Guid"/> of the <see cref="UserRole"/> you're trying to find.</param>
	/// <param name="role">The <see cref="UserRole"/>, if one was found. </param>
	/// <param name="error">An <see cref="ResponseResultError"/>, if no <see cref="UserRole"/> was found.</param>
	/// <returns>True if <paramref name="role"/> is not null; False if <paramref name="error"/> is not null.</returns>
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

	/// <summary>
	/// <para>Helper function to find a <see cref="UserPermission"/> by its <see cref="UserPermission.DataName"/>, but additionally output a <see cref="ResponseResultError"/> if no <see cref="UserPermission"/> is found.</para>
	/// <para>Intended to be used in senarios where a response cannot continue if the <see cref="UserPermission"/> cannot be found.</para>
	/// </summary>
	/// <param name="dataName">The <see cref="UserPermission.DataName"/> corresponding to the <see cref="UserPermission"/> you're looking for </param>
	/// <param name="permission">The <see cref="UserPermission"/>, if one was found. </param>
	/// <param name="error">An <see cref="ResponseResultError"/>, if no <see cref="UserPermission"/> was found.</param>
	/// <returns>True if <paramref name="permission"/> is not null; False if <paramref name="error"/> is not null.</returns>
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

	/// <summary>
	/// <para>Helper function to call <see cref="ISaveDataIndexable.TryGetSave{T}(Guid, out T?)"/>, but additionally output a <see cref="ResponseResultError"/> if no <see cref="SaveData"/> is found.</para>
	/// <para>Intended to be used in senarios where a response cannot continue if the <see cref="SaveData"/> cannot be found.</para>
	/// </summary>
	/// <param name="indexable">A <see cref="ISaveDataIndexable"/> to get the <typeparamref name="TData"/> from.</param>
	/// <param name="guid">The <see cref="Guid"/> of the <typeparamref name="TData"/> you're trying to find.</param>
	/// <param name="data">The <typeparamref name="TData"/> if one was found. </param>
	/// <param name="error">An <see cref="ResponseResultError"/>, if no <typeparamref name="TData"/> was found.</param>
	/// <returns>True if <paramref name="data"/> is not null; False if <paramref name="error"/> is not null.</returns>
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

	/// <summary>
	/// <para>Helper function to call <see cref="IAssetIndexable.TryGetAsset{T}(AssetLocation, out T?)"/>, but additionally output a <see cref="ResponseResultError"/> if no <see cref="Asset"/> is found.</para>
	/// <para>Intended to be used in senarios where a response cannot continue if the <see cref="Asset"/> cannot be found.</para>
	/// </summary>
	/// <param name="indexable">A <see cref="IAssetIndexable"/> to get the <typeparamref name="TAsset"/> from.</param>
	/// <param name="location">The <see cref="AssetLocation"/> of the <typeparamref name="TAsset"/> you're trying to find.</param>
	/// <param name="asset">The <typeparamref name="TAsset"/> if one was found. </param>
	/// <param name="error">An <see cref="ResponseResultError"/>, if no <typeparamref name="TAsset"/> was found.</param>
	/// <returns>True if <paramref name="data"/> is not null; False if <paramref name="error"/> is not null.</returns>
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

	/// <summary>
	/// Compares the <paramref name="requestingUser"/> to the provided <see cref="IUserHierarchyComparable"/>, returning true if <paramref name="requestingUser"/> is higher, and false + a <see cref="ResponseResultError"/> otherwise.
	/// </summary>
	/// <param name="requestingUser">The <see cref="User"/> who needs to be higher than <paramref name="expectedLower"/>.</param>
	/// <param name="expectedLower">The <see cref="IUserHierarchyComparable"/> that needs to be lower than <paramref name="requestingUser"/>.</param>
	/// <param name="error">An <see cref="ResponseResultError"/>, if <paramref name="requestingUser"/> was equal or lower than <paramref name="expectedLower"/>.</param>
	/// <returns>True if user is higher; False if user is equal or lower + <paramref name="error"/> will not be null. </returns>
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
