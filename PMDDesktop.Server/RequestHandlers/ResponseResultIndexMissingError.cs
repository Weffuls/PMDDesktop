namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states a specified object of type <typeparamref name="TObject"/> cannot be found using the provided <paramref name="indexer"/>.
/// </summary>
/// <param name="indexer">The ID or name that was used to try to find the object; usually a stringified GUID or handle.</param>
/// <typeparam name="TObject">The Type of the object that couldn't be found.</typeparam>
public class ResponseResultIndexMissingError<TObject>(string indexer) : ResponseResultError
{

	/// <summary>
	/// The ID or name that was used to try to find the object; usually a stringified GUID or handle.
	/// </summary>
	public string Indexer { get; private init; } = indexer;

	public override string GetErrorReason() => $"Unable to find a {typeof(Type).Name} with '${Indexer}'.";
	public override int GetStatusCode() => 400;

}
