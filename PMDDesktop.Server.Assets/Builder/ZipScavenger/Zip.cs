using System.IO.Compression;
using System.Text.RegularExpressions;

namespace PMDDesktop.Server.Assets.Builder.ZipScavenger;

internal abstract class Zip(string path) : IDisposable, IAsyncDisposable
{

	private ZipArchive _archive = ZipFile.OpenRead(path);
	private ILookup<string, ZipArchiveEntry>? _entryLookup;

	public ZipArchiveEntry GetEntry(string entryPath)
	{

		return _archive.GetEntry(entryPath) ?? throw new KeyNotFoundException($"Could not find an entry for '{entryPath}'");

	}

	protected IEnumerable<ZipArchiveEntry> EnumerateEntries(Regex regex)
	{

		foreach (ZipArchiveEntry entry in _archive.Entries)
		{

			// Skip directories. They aren't real entries.
			if (entry.FullName.EndsWith('/'))
				continue;

			if (regex.IsMatch(entry.FullName))
				yield return entry;

		}

		yield break;

	}

	void IDisposable.Dispose()
	{
		((IDisposable)_archive).Dispose();
	}

	ValueTask IAsyncDisposable.DisposeAsync()
	{
		return ((IAsyncDisposable)_archive).DisposeAsync();
	}

}
