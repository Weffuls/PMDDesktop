using PMDDesktop.Results;

namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// <para>Represents that the request failed in some way; and details our response about why it failed.</para>
/// <para>Override <see cref="GetStatusCode()"/> to specify which return code to return.</para>
/// </summary>
public abstract class ResponseResultError : ResultError
{

	/// <summary>
	/// The HTTP status code to return for this error. Usually somewhere in the 400-599 range.
	/// </summary>
	/// <returns>An int representing the HTTP status code for this error, usually between 400-599.</returns>
	public abstract int GetStatusCode();

}
