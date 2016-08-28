using Microsoft.Xna.Framework;

namespace LD36.Physics
{
	internal static class PhysicsConvert
	{
		private const int PixelsPerMeter = 32;

		public static float ToMeters(float value)
		{
			return value / PixelsPerMeter;
		}

		public static float ToPixels(float value)
		{
			return value * PixelsPerMeter;
		}

		public static Vector2 ToMeters(Vector2 value)
		{
			return value / PixelsPerMeter;
		}

		public static Vector2 ToPixels(Vector2 value)
		{
			return value * PixelsPerMeter;
		}
	}
}
