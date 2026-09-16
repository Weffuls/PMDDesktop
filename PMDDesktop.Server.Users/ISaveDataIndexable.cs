using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Server.Users;

/// <summary>
/// Interface for objects that can fetch a <see cref="User"/> from a <see cref="UserManager"/>.
/// </summary>
public interface IUserIndexable
{

	
	User GetUser(Guid GUID);

	
	bool TryGetUser(Guid GUID, [NotNullWhen(true)] out User user);

}
