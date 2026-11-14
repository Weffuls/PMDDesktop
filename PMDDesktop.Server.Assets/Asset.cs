using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets;

public abstract class Asset
{

	[JsonConstructor]
	internal Asset(DataIDPath location)
	{
		Path = location;
	}

	[JsonIgnore]
	public DataIDPath Path { get; internal set; }

	/// <summary>
	/// The AssetManager this Asset belongs to. Can be null if not yet assigned to an AssetManager.
	/// </summary>
	/// <remarks>
	/// To avoid entering an invalid state, do not interchange Assets between AssetManagers.
	/// </remarks>
	[JsonIgnore]
	public AssetManager? Manager { get; internal set; }

	public override string ToString()
	{
		return $"[Asset:{GetType().Name}:{Path}]";
	}

}
