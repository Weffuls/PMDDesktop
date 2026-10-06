using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Server.Users;

/// <summary>
/// Interface for objects that can fetch a <see cref="User"/> from a <see cref="UserManager"/>.
/// </summary>
public interface IUserIndexable
{

	/// <summary>
	/// Try to get a <see cref="User"/> by their <see cref="Guid"/>. Will return false and <paramref name="user"/> will be null if not found.
	/// </summary>
	/// <param name="GUID">The <see cref="Guid"/> of the <see cref="User"/> you're looking for.</param>
	/// <param name="user">The found <see cref="User"/>, if any was found. Will be null if the return was false.</param>
	/// <returns>True if the <see cref="User"/> is found, and <paramref name="user"/> will not be null.</returns>
	bool TryGetUser(Guid GUID, [NotNullWhen(true)] out User? user);

}
