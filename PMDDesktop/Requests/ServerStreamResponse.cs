namespace PMDDesktop.Requests;

/// <summary>
/// <para>This class is a special-case in ServerResponses where we need to understand that any type of data could be coming through.</para>
/// <para>Usually this is a raw file or image of some sort, so any functions handling <see cref="ServerRequest{}"/>s and <see cref="ServerResponse"/>s need to populate this object with the stream they have.</para>
/// </summary>
public sealed class ServerStreamResponse(Stream stream) : ServerResponse
{

	public Stream Stream { get; private init; } = stream;

}
