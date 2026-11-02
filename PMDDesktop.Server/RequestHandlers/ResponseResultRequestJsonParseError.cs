using System.Text.Json;

namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> stating that a provided JSON could not be parsed.
/// </summary>
/// <param name="jsonException">The <see cref="JsonException"/> that occurred and resulted in this error.</param>
/// <param name="nameOfJson">Optional name of the json to help with identifying where the problem is.</param>
public class ResponseResultRequestJsonParseError(JsonException jsonException, string? nameOfJson = null) : ResponseResultRequestError
{

	/// <summary>
	/// The <see cref="JsonException"/> that occurred and resulted in this error.
	/// </summary>
	public JsonException JsonException { get; private init; } = jsonException;

	/// <summary>
	/// Optional name of the json to help with identifying where the problem is.
	/// </summary>
	public string? NameOfJson { get; private init; } = nameOfJson;

	protected override string GetRequestErrorReason() => NameOfJson is null
		? $"Unable to parse JSON: {JsonException.Message}"
		: $"Unable to parse JSON '{NameOfJson}': {JsonException.Message}";

}
