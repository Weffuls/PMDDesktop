namespace PMDDesktop.Server.Users;

/// <summary>
/// <para>Interface to compare if a user has access to certain data.</para>
/// <para>Lower numeric-values are higher permissions; a user must have a higher permission to edit other users' data.</para>
/// </summary>
public interface IUserHierarchyComparable
{

	/// <summary>
	/// <para>Returns a number indicating the highest place in the hierarchy this user has.</para>
	/// <para>0 should be the highest value obtainable by a normal user. -1 is obtainable to admins only.</para>
	/// <para>Lower numeric values are higher permissions; this should usually be calculated based on role order.</para>
	/// </summary>
	/// <returns>Number indicating the highest place in the hierarchy this user has.</returns>
	int GetHierarchyPosition();

}
