using Microsoft.Xna.Framework;

namespace LD36.Generation
{
	internal static class GenerationUtilities
	{
		public static int[] GetSurroundingValues(int[,] fullTiles, int i, int j)
		{
			// The order here is up, down, left, right.
			int[] surroundingValues = new int[4];
			surroundingValues[0] = fullTiles[j, i - 1];
			surroundingValues[1] = fullTiles[j, i + 1];
			surroundingValues[2] = fullTiles[j - 1, i];
			surroundingValues[3] = fullTiles[j + 1, i];

			return surroundingValues;
		}

		public static Point[] GetSurroundingPoints(int[,] fullTiles, int i, int j)
		{
			// The order here is up, down, left, right (same as the function above).
			Point[] surroundingPoints = new Point[4];
			surroundingPoints[0] = new Point(j, i - 1);
			surroundingPoints[1] = new Point(j, i + 1);
			surroundingPoints[2] = new Point(j - 1, i);
			surroundingPoints[3] = new Point(j + 1, i);

			return surroundingPoints;
		}
	}
}
