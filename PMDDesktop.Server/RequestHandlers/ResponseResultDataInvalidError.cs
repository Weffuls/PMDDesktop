namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResultDataInvalidError(string identifier, string? providedData) : ResponseResultError
{

	public string DataIdentifier { get; private init; } = identifier;
	public string? ProvidedData { get; private init; } = providedData;

	public override string GetErrorReason() => $"'{ProvidedData}' is not valid for '{DataIdentifier}'.";
	public override int GetReturnCode() => 400;

}
