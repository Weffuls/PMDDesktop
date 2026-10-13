using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users;

[JsonConverter(typeof(UserReferenceConverter))]
public sealed class UserReference
{

	/// <summary>
	/// Stored <see cref="Guid"/> corresponding to <see cref="User.GUID"/>.
	/// </summary>
	public Guid GUID { get; private set; }

	/// <summary>
	/// <para>Create a new <see cref="UserReference"/> pointing to <paramref name="initalValue"/>'s <see cref="Guid"/>.</para>
	/// <para>This allows you to find the <see cref="User"/> again after serialization and deserialization.</para>
	/// </summary>
	/// <param name="initalValue">The <see cref="User"/> you'd like to keep a reference to via <see cref="Guid"/>.</param>
	public UserReference(User initalValue)
	{

		GUID = initalValue.GUID;

	}

	/// <summary>
	/// <para>Create a new <see cref="UserReference"/> pointing exactly to <paramref name="directGuid"/></para>
	/// <para>The existance of any <see cref="User"/> with this <see cref="Guid"/> is not checked until <see cref="TryGetUser(IUserIndexable, out User?)"/> is called.</para>
	/// </summary>
	/// <param name="directGuid">The <see cref="Guid"/> you'd like to point to.</param>
	internal UserReference(Guid directGuid)
	{

		GUID = directGuid;

	}

	/// <summary>
	/// Try to get a <see cref="User"/> from its <see cref="User.GUID"/>.
	/// </summary>
	/// <param name="indexable">The <see cref="IUserIndexable"/> to use to find <paramref name="user"/>.</param>
	/// <param name="user">The result of the search. Will be null if false is returned, and contain a <see cref="User"/> if true.</param>
	/// <returns>True if the <see cref="User"/> was found and <paramref name="user"/> has a value, otherwise false.</returns>
	public bool TryGetUser(IUserIndexable indexable, [NotNullWhen(true)] out User? user)
	{

		return indexable.TryGetUser(GUID, out user);

	}

}
