using System.Text.Json;

namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states that the deserialized value kind of an input JSON does not match the expected value kind of the JSON.
/// </summary>
/// <param name="expectedKind">The <see cref="JsonValueKind"/> we need.</param>
/// <param name="actualKind">The <see cref="JsonValueKind"/> we got.</param>
/// <param name="nameOfJson">Optional name of the json to help with identifying where the problem is.</param>
public class ResponseResultRequestJsonWrongValueKindError(JsonValueKind expectedKind, JsonValueKind? actualKind, string? nameOfJson = null) : ResponseResultRequestError
{

	/// <summary>
	/// The <see cref="JsonValueKind"/> we need.
	/// </summary>
	public JsonValueKind ExpectedKind { get; private init; } = expectedKind;

	/// <summary>
	/// The <see cref="JsonValueKind"/> we got.
	/// </summary>
	public JsonValueKind? ActualKind { get; private init; } = actualKind;

	/// <summary>
	/// Optional name of the json to help with identifying where the problem is.
	/// </summary>
	public string? NameOfJson { get; private init; } = nameOfJson;
	protected override string GetRequestErrorReason() =>
		$"Parsed JSON {(NameOfJson is not null ? $"{NameOfJson} " : "")}was not {ExpectedKind}, but instead was {(ActualKind is not null ? ActualKind : "was missing (null)")}.";

}
