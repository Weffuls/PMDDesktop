using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users.Roles;

[JsonConverter(typeof(RoleReferenceConverter))]
public sealed class RoleReference
{

	public Guid GUID { get; private set; }


	public RoleReference(UserRole initalValue)
	{

		GUID = initalValue.GUID;

	}

	internal RoleReference(Guid directGuid)
	{

		GUID = directGuid;

	}

	public bool TryGetRole(IRoleIndexable indexable, [NotNullWhen(true)] out UserRole? role)
	{

		return indexable.TryGetRole(GUID, out role);

	}

}
