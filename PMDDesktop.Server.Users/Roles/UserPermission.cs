using System.Collections.Immutable;
using System.Reflection;

namespace PMDDesktop.Server.Users.Roles;

public sealed class UserPermission
{

	static UserPermission()
	{

		FieldInfo[] fields = typeof(UserPermission).GetFields(BindingFlags.Static | BindingFlags.Public);

		IEnumerable<FieldInfo> permissionFields = fields.Where(field => field.FieldType.IsAssignableTo(typeof(UserPermission)));

		IEnumerable<UserPermission> permissionObjects = permissionFields.Select(field =>
			(UserPermission?) field.GetValue(ALL)
				?? throw new NullReferenceException($"Unable to convert {field} to {nameof(UserPermission)}"));

		ALL = ImmutableArray.Create([.. permissionObjects]);

	}

	public static readonly ImmutableArray<UserPermission> ALL;

	public static readonly UserPermission MANAGE_USERS = new()
	{
		DataName = "manage-users",
		FriendlyName = "Manage Users",
		FriendlyDescription = "Allow creation, modification, and deletion of user accounts.",
		AlwaysForAdmin = true,
		EnabledForDefault = false
	};

	public static readonly UserPermission MANAGE_ROLES = new()
	{
		DataName = "manage-roles",
		FriendlyName = "Manage Roles",
		FriendlyDescription = "Allow creation, modification, and removal of roles and their permissions.",
		AlwaysForAdmin = true,
		EnabledForDefault = false
	};

	/// <summary>
	/// Name that is stored and compared to track permission in a <see cref="UserRole"/>.
	/// </summary>
	public required string DataName {get; init;}

	/// <summary>
	/// User-facing name that is displayed to users when viewing permissions.
	/// </summary>
	public required string FriendlyName {get; init;}

	/// <summary>
	/// User-facing description for what a permission does.
	/// </summary>
	public required string FriendlyDescription {get; init;}

	/// <summary>
	/// Weather to always grant this permission for admin users, to prevent losing ability to regain the permission.
	/// </summary>
	public required bool AlwaysForAdmin {get; init;}

	public required bool EnabledForDefault {get; init;}

	private UserPermission() { }

	public override int GetHashCode()
	{
		return HashCode.Combine(DataName);
	}

	public override bool Equals(object? obj)
	{

		if (obj is UserPermission permission)
			return permission.DataName == DataName;

		return base.Equals(obj);
	}

	public override string ToString()
	{
		return $"{nameof(UserPermission)}:{DataName}";
	}

}
