#if DEBUG

using Microsoft.Xna.Framework;

namespace PMDDesktop.Client.DebugMenus;

internal interface IDebugMenuItem
{

	Vector2 Size { get; }

	void Draw(bool isHighlighted, Vector2 position);

	void Selected();

}

#endif
