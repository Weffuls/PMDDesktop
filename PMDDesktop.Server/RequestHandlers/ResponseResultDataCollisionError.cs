namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states a specified value cannot be used because it clashes with another object in a collection/list.
/// </summary>
/// <param name="identifier">A name that helps the user understand what the clashing value was; usually this is the name of the parameter the user sent.</param>
/// <param name="providedData">The value that was recieved; this is the string that's causing the clash. Usually a unique ID.</param>
public class ResponseResultDataCollisionError(string identifier, string providedData) : ResponseResultError
{

	/// <summary>
	/// A name that helps the user understand what the clashing value was; usually this is the name of the parameter the user sent.
	/// </summary>
	public string DataIdentifier { get; private init; } = identifier;

	/// <summary>
	/// The value that was recieved; this is the string that's causing the clash. Usually a unique ID.
	/// </summary>
	public string? ProvidedData { get; private init; } = providedData;

	public override string GetErrorReason() => $"The provided value for ''{DataIdentifier}', '{ProvidedData}', cannot be used because it collides or duplicates with other data.";
	public override int GetStatusCode() => 400;

}
