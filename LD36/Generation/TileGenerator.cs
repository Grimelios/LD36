using LD36.Entities;
using LD36.Entities.Environment;
using Microsoft.Xna.Framework;

namespace LD36.Generation
{
	internal class TileGenerator
	{
		private const string PyramidTilesheet = "Pyramid";
		private const int InnerBackgroundTile = 12;
		private const int OuterBackgroundTile = 13;

		private int pyramidWidth;
		private int pyramidHeight;
		private int scaleMultiplier;

		public TileGenerator(int pyramidWidth, int pyramidHeight, int scaleMultiplier)
		{
			this.pyramidWidth = pyramidWidth;
			this.pyramidHeight = pyramidHeight;
			this.scaleMultiplier = scaleMultiplier;
		}

		public void Generate(int[,] fullTiles)
		{
			int scaledWidth = pyramidWidth * scaleMultiplier;
			int scaledHeight = pyramidHeight * scaleMultiplier;
			int[,] tiles = new int[scaledWidth, scaledHeight];

			for (int i = 0; i < scaledHeight; i++)
			{
				for (int j = 0; j < scaledWidth; j++)
				{
					tiles[j, i] = InnerBackgroundTile;
				}
			}

			EdgePass(fullTiles, tiles);
			CornerPass(fullTiles, tiles);

			EntityUtilities.AddEntity(new Tilemap(Vector2.Zero, scaledWidth, scaledHeight, tiles, PyramidTilesheet));
		}

		private void EdgePass(int[,] fullTiles, int[,] tiles)
		{
			for (int i = 0; i < pyramidHeight; i++)
			{
				for (int j = 0; j < pyramidWidth; j++)
				{
					if (fullTiles[j, i] == -1)
					{
						AddEdgeTiles(fullTiles, tiles, i, j);
					}
				}
			}
		}

		private void AddEdgeTiles(int[,] fullTiles, int[,] tiles, int i, int j)
		{
			int startX = j * scaleMultiplier;
			int startY = i * scaleMultiplier;
			int[] surroundingValues = GetSurroundingValuesEdgeSafe(fullTiles, i, j);

			for (int k = 0; k < scaleMultiplier; k++)
			{
				for (int l = 0; l < scaleMultiplier; l++)
				{
					tiles[startX + l, startY + k] = OuterBackgroundTile;
				}
			}
			
			for (int k = 0; k < surroundingValues.Length; k++)
			{
				if (surroundingValues[k] == -1)
				{
					continue;
				}

				int xIncrement = 0;
				int yIncrement = 0;
				int tileValue = k;

				startX = j * scaleMultiplier;
				startY = i * scaleMultiplier;

				switch (k)
				{
					// Top/bottom.
					case 0:
					case 1:
						xIncrement = 1;
						break;

					// Left/right.
					case 2:
					case 3:
						yIncrement = 1;
						break;
				}

				switch (k)
				{
					// Bottom.
					case 1:
						startY += scaleMultiplier - 1;
						break;

					// Right.
					case 3:
						startX += scaleMultiplier - 1;
						break;
				}

				for (int l = 0; l < scaleMultiplier; l++)
				{
					tiles[startX + xIncrement * l, startY + yIncrement * l] = tileValue;
				}
			}
		}

		private void CornerPass(int[,] fullTiles, int[,] tiles)
		{
			for (int i = 0; i < pyramidHeight; i++)
			{
				for (int j = 0; j < pyramidWidth; j++)
				{
					if (fullTiles[j, i] == -1)
					{
						AddCornerTiles(fullTiles, tiles, i, j);
					}
				}
			}
		}

		private void AddCornerTiles(int[,] fullTiles, int[,] tiles, int i, int j)
		{
			int[] surroundingValues = GetSurroundingValuesEdgeSafe(fullTiles, i, j);

			// The order here is top-left, top-right, bottom-left, bottom-right
			int[] cornerValues = new int[4];
			cornerValues[0] = i == 0 || j == 0 ? -1 : fullTiles[j - 1, i - 1];
			cornerValues[1] = i == 0 || j == pyramidWidth - 1 ? -1 : fullTiles[j + 1, i - 1];
			cornerValues[2] = i == pyramidHeight  - 1 || j == 0 ? -1 : fullTiles[j - 1, i + 1];
			cornerValues[3] = i == pyramidHeight - 1 || j == pyramidWidth - 1 ? -1 : fullTiles[j + 1, i + 1];

			for (int k = 0; k < cornerValues.Length; k++)
			{
				if (cornerValues[k] != -1)
				{
					int tileValue = CheckSurroundingTilesFromCorner(surroundingValues, k);

					if (tileValue != -1)
					{
						int tileX = j * scaleMultiplier;
						int tileY = i * scaleMultiplier;

						switch (k)
						{
							// Top-right.
							case 1:
								tileX += scaleMultiplier - 1;
								break;

							// Bottom-left.
							case 2:
								tileY += scaleMultiplier - 1;
								break;

							// Bottom-right.
							case 3:
								tileX += scaleMultiplier - 1;
								tileY += scaleMultiplier - 1;
								break;
						}

						tiles[tileX, tileY] = tileValue;
					}
				}
			}
		}

		private int CheckSurroundingTilesFromCorner(int[] surroundingTiles, int k)
		{
			int value1 = 0;
			int value2 = 0;

			switch (k)
			{
				// Top-left:
				case 0:
					value1 = surroundingTiles[0];
					value2 = surroundingTiles[2];
					break;

				// Top-right:
				case 1:
					value1 = surroundingTiles[0];
					value2 = surroundingTiles[3];
					break;

				// Bottom-left:
				case 2:
					value1 = surroundingTiles[1];
					value2 = surroundingTiles[2];
					break;

				// Bottom-right:
				case 3:
					value1 = surroundingTiles[1];
					value2 = surroundingTiles[3];
					break;
			}

			if (value1 == -1 && value2 == -1)
			{
				return k + 4;
			}

			if (value1 != -1 && value2 != -1)
			{
				return k + 8;
			}

			return -1;
		}

		private int[] GetSurroundingValuesEdgeSafe(int[,] fullTiles, int i, int j)
		{
			// This code is a bit different than the GenerationUtilities version because it has to account for edge cases.
			int[] surroundingValues = new int[4];
			surroundingValues[0] = i == 0 ? -1 : fullTiles[j, i - 1];
			surroundingValues[1] = i == pyramidHeight - 1 ? -1 : fullTiles[j, i + 1];
			surroundingValues[2] = j == 0 ? -1 : fullTiles[j - 1, i];
			surroundingValues[3] = j == pyramidWidth - 1 ? -1 : fullTiles[j + 1, i];

			return surroundingValues;
		}
	}
}
