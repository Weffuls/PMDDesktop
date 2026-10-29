namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states a specified object of type <typeparamref name="TObject"/> cannot be found using the provided <paramref name="indexer"/>.
/// </summary>
/// <param name="indexer"></param>
/// <param name="type"></param>
public class ResponseResultIndexMissingError<TObject>(string indexer) : ResponseResultError
{

	public string Indexer { get; private init; } = indexer;

	public override string GetErrorReason() => $"Unable to find a {typeof(Type).Name} with '${Indexer}'.";
	public override int GetReturnCode() => 400;

}
