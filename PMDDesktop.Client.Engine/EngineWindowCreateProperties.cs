using CSDL.Video;

namespace PMDDesktop.Client.Engine;

public record EngineWindowCreateProperties()
{

	public enum WindowType { BasicFixed, BasicResizable, Utility }

	// Size and Positioning
	public Point? Position { get; set; }
	public Point Size { get; set; } = new(256, 256);

	public WindowType Type { get; set; }
	public string? Title { get; set; }

	internal WindowCreateProperties GetWindowCreateProperties()
	{

		WindowCreateProperties properties = new();

		if (Title is string titleString)
			properties.Title.Set(titleString);

		if (Position is Point posPoint)
		{
			properties.X.Set(posPoint.X);
			properties.Y.Set(posPoint.Y);
		}

		properties.Width.Set(Size.X);
		properties.Height.Set(Size.Y);

		properties.Resizable.Set(Type == WindowType.BasicResizable);

		// Utility window things.
		if (Type == WindowType.Utility)
		{
			properties.Utility.Set(true);
			properties.Borderless.Set(true);
			properties.Transparent.Set(true);
			properties.Focusable.Set(false);
			properties.AlwaysOnTop.Set(true);
		}
		else
		{
			properties.Utility.Set(false);
			properties.Borderless.Set(false);
			properties.Transparent.Set(false);
			properties.Focusable.Set(true);
			properties.AlwaysOnTop.Set(false);
		}

		return properties;

	}

}