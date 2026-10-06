using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Server.Users.Roles;

/// <summary>
/// Interface for objects that can fetch a <see cref="UserRole"/> from a <see cref="UserRoleManager"/>.
/// </summary>
public interface IRoleIndexable
{

	/// <summary>
	/// Try to get a <see cref="UserRole"/> by its <see cref="Guid"/>. Will return false and <paramref name="role"/> will be null if not found.
	/// </summary>
	/// <param name="GUID">The <see cref="Guid"/> of the <see cref="UserRole"/> you're looking for.</param>
	/// <param name="role">The found <see cref="UserRole"/>, if any was found. Will be null if the return was false.</param>
	/// <returns>True if the <see cref="UserRole"/> is found, and <paramref name="role"/> will not be null.</returns>
	bool TryGetRole(Guid GUID, [NotNullWhen(true)] out UserRole? role);

}
