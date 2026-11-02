namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states a specified value cannot be used and can never be used because it is invalid.
/// </summary>
/// <param name="identifier">A name that helps the user understand what the invalid value was; usually this is the name of the parameter the user sent.</param>
/// <param name="providedData">The value that was recieved; this is the string that's invalid. Usually a strict variable.</param>
public class ResponseResultDataInvalidError(string identifier, string? providedData) : ResponseResultError
{

	/// <summary>
	/// A name that helps the user understand what the invalid value was; usually this is the name of the parameter the user sent.
	/// </summary>
	public string DataIdentifier { get; private init; } = identifier;

	/// <summary>
	/// The value that was recieved; this is the string that's invalid. Usually a strict variable.
	/// </summary>
	public string? ProvidedData { get; private init; } = providedData;

	public override string GetErrorReason() => $"'{ProvidedData}' is not valid for '{DataIdentifier}'.";
	public override int GetStatusCode() => 400;

}
