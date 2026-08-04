namespace PMDDesktop.Server.Assets;

[AttributeUsage(AttributeTargets.Class)]
internal class AssetFileNameAttribute(string fileName) : Attribute
{

	internal string FileName { private init; get; } = fileName.ToLowerInvariant();

}
