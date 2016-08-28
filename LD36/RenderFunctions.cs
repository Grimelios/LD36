using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36
{
	internal static class RenderFunctions
	{
		private static Texture2D pixelTexture = ContentLoader.LoadTexture("Pixel");

		public static void RenderLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color)
		{
			float distance = Vector2.Distance(start, end);
			float rotation = GameFunctions.ComputeAngle(start, end);

			Rectangle sourceRect = new Rectangle(0, 0, (int)distance, 1);

			sb.Draw(pixelTexture, start, sourceRect, color, rotation, Vector2.Zero, 1, SpriteEffects.None, 0);
		}
	}
}
