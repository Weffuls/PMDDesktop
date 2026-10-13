using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users.Roles;

[JsonConverter(typeof(RoleReferenceConverter))]
public sealed class RoleReference
{

	/// <summary>
	/// Stored <see cref="Guid"/> corresponding to <see cref="UserRole.GUID"/>.
	/// </summary>
	public Guid GUID { get; private set; }

	/// <summary>
	/// <para>Create a new <see cref="RoleReference"/> pointing to <paramref name="initalValue"/>'s <see cref="Guid"/>.</para>
	/// <para>This allows you to find the <see cref="UserRole"/> again after serialization and deserialization.</para>
	/// </summary>
	/// <param name="initalValue">The <see cref="UserRole"/> you'd like to keep a reference to via <see cref="Guid"/>.</param>
	public RoleReference(UserRole initalValue)
	{

		GUID = initalValue.GUID;

	}

	/// <summary>
	/// <para>Create a new <see cref="RoleReference"/> pointing exactly to <paramref name="directGuid"/></para>
	/// <para>The existance of any <see cref="UserRole"/> with this <see cref="Guid"/> is not checked until <see cref="TryGetRole(IRoleIndexable, out UserRole?)"/> is called.</para>
	/// </summary>
	/// <param name="directGuid">The <see cref="Guid"/> you'd like to point to.</param>
	internal RoleReference(Guid directGuid)
	{

		GUID = directGuid;

	}

	/// <summary>
	/// Try to get a <see cref="UserRole"/> from its <see cref="UserRole.GUID"/>.
	/// </summary>
	/// <param name="indexable">The <see cref="IRoleIndexable"/> to use to find <paramref name="role"/>.</param>
	/// <param name="role">The result of the search. Will be null if false is returned, and contain a <see cref="UserRole"/> if true.</param>
	/// <returns>True if the <see cref="UserRole"/> was found and <paramref name="role"/> has a value, otherwise false.</returns>
	public bool TryGetRole(IRoleIndexable indexable, [NotNullWhen(true)] out UserRole? role)
	{

		return indexable.TryGetRole(GUID, out role);

	}

}
