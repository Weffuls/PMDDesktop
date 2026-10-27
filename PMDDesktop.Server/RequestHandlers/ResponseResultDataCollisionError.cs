namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResultDataCollisionError(string identifier, string providedData) : ResponseResultError
{

	public string DataIdentifier { get; private init; } = identifier;
	public string? ProvidedData { get; private init; } = providedData;

	public override string GetErrorReason() => $"The provided value for ''{DataIdentifier}', '{ProvidedData}', cannot be used because it collides or duplicates with other data.";
	public override int GetReturnCode() => 400;

}
