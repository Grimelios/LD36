using LD36.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.UI
{
	internal abstract class UIElement : IDynamic, IRenderable
	{
		public abstract void Update(float dt);
		public abstract void Render(SpriteBatch sb);
	}
}
