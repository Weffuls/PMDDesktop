namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResultDataMissingError(string indexer, Type type) : ResponseResultError
{

	public string Indexer { get; private init; } = indexer;
	public Type Type { get; private init; } = type;

	public override string GetErrorReason() => $"Unable to find a {Type.Name} with '${Indexer}'.";
	public override int GetReturnCode() => 400;

}
