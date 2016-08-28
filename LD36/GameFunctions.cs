using System;
using Microsoft.Xna.Framework;

namespace LD36
{
	internal static class GameFunctions
	{
		private static Random random = new Random();

		public static int GetRandomValue(int min, int max)
		{
			return random.Next(max - min + 1) + min;
		}

		public static float ComputeAngle(Vector2 start, Vector2 end)
		{
			float dX = end.X - start.X;
			float dY = end.Y - start.Y;

			return (float)Math.Atan2(dY, dX);
		}
	}
}
