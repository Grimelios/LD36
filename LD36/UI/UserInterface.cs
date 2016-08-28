using System.Collections.Generic;
using LD36.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.UI
{
	internal class UserInterface : IDynamic, IRenderable
	{
		public UserInterface()
		{
			Elements = new List<UIElement>();
		}

		public List<UIElement> Elements { get; }
		
		public void Update(float dt)
		{
			Elements.ForEach(element => element.Update(dt));
		}

		public void Render(SpriteBatch sb)
		{
			Elements.ForEach(element => element.Render(sb));
		}
	}
}
