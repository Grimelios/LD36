using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD36
{
	internal static class ContentLoader
	{
		private static ContentManager content;

		public static void Initialize(ContentManager contentManager)
		{
			content = contentManager;
		}

		public static SpriteFont LoadFont(string filename)
		{
			return content.Load<SpriteFont>("Fonts/" + filename);
		}

		public static Texture2D LoadTexture(string filename)
		{
			return content.Load<Texture2D>("Textures/" + filename);
		}
	}
}
