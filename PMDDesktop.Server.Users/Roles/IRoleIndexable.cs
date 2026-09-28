using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Server.Users.Roles;

/// <summary>
/// Interface for objects that can fetch a <see cref="UserRole"/> from a <see cref="UserRoleManager"/>.
/// </summary>
public interface IRoleIndexable
{

	bool TryGetRole(Guid GUID, [NotNullWhen(true)] out UserRole? role);

}
