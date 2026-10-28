using System.Text.Json;

namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResultRequestJsonWrongTypeError(JsonValueKind expectedKind, JsonValueKind? actualKind, string? nameOfJson = null) : ResponseResultRequestError
{

	public JsonValueKind ExpectedKind { get; private init; } = expectedKind;
	public JsonValueKind? ActualKind { get; private init; } = actualKind;
	public string? NameOfJson { get; private init; } = nameOfJson;
	protected override string GetRequestErrorReason() =>
		$"Parsed JSON {(NameOfJson is not null ? $"{NameOfJson} " : "")}was not {ExpectedKind}, but instead was {(ActualKind is not null ? ActualKind : "was missing (null)")}.";

}
