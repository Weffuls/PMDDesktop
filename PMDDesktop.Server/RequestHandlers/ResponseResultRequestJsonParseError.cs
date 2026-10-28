using System.Text.Json;

namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResultRequestJsonParseError(JsonException jsonException, string? nameOfJson = null) : ResponseResultRequestError
{

	public JsonException JsonException { get; private init; } = jsonException;
	public string? NameOfJson { get; private init; } = nameOfJson;
	protected override string GetRequestErrorReason() => NameOfJson is null
		? $"Unable to parse JSON: {JsonException.Message}"
		: $"Unable to parse JSON '{NameOfJson}': {JsonException.Message}";

}
