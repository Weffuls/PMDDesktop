using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users;

[JsonConverter(typeof(UserReferenceConverter))]
public sealed class UserReference
{

	public Guid GUID { get; private set; }


	public UserReference(User initalValue)
	{

		GUID = initalValue.GUID;

	}


	internal UserReference(Guid directGuid)
	{

		GUID = directGuid;

	}


	public bool TryGetUser(IUserIndexable indexable, [NotNullWhen(true)] out User? user)
	{

		return indexable.TryGetUser(GUID, out user);

	}

}
