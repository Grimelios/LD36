using FarseerPhysics.Dynamics;
using LD36.Physics;
using Microsoft.Xna.Framework;

namespace LD36.Generation
{
	internal class EdgeGenerator
	{
		private enum EdgeTypes
		{
			Top = 0,
			Bottom = 1,
			Left = 2,
			Right = 3
		}

		private int pyramidWidth;
		private int pyramidHeight;
		private int scaleMultiplier;
		private int entranceColumn;

		private Body body;
		private PhysicsFactory physicsFactory;

		public EdgeGenerator(int pyramidWidth, int pyramidHeight, int scaleMultiplier, int entranceColumn)
		{
			this.pyramidWidth = pyramidWidth;
			this.pyramidHeight = pyramidHeight;
			this.scaleMultiplier = scaleMultiplier;
			this.entranceColumn = entranceColumn;
		}

		public void Generate(int[,] fullTiles)
		{
			GenerateEdges(GenerateEdgeFlags(fullTiles));
		}

		private bool[,,] GenerateEdgeFlags(int[,] fullTiles)
		{
			bool[,,] edgeFlags = new bool[pyramidWidth, pyramidHeight, 4];

			for (int i = 1; i < pyramidHeight - 1; i++)
			{
				for (int j = 1; j < pyramidWidth - 1; j++)
				{
					if (fullTiles[j, i] != -1 && j != entranceColumn)
					{
						int[] surroundingValues = GenerationUtilities.GetSurroundingValues(fullTiles, i, j);

						for (int k = 0; k < surroundingValues.Length; k++)
						{
							if (surroundingValues[k] == -1)
							{
								edgeFlags[j, i, k] = true;
							}
						}
					}
				}
			}

			return edgeFlags;
		}

		private void GenerateEdges(bool[,,] edgeFlags)
		{
			physicsFactory = DIKernel.Get<PhysicsFactory>();
			body = physicsFactory.CreateBody(null);

			bool[,,] visited = new bool[pyramidWidth, pyramidHeight, 4];

			for (int i = 1; i < pyramidHeight - 1; i++)
			{
				for (int j = 1; j < pyramidWidth - 1; j++)
				{
					for (int k = 0; k < 4; k++)
					{
						if (!visited[j, i, k] && edgeFlags[j, i, k])
						{
							GenerateEdge(edgeFlags, visited, i, j, k);
						}
					}
				}
			}
		}

		private void GenerateEdge(bool[,,] edgeFlags, bool[,,] visited, int i, int j, int k)
		{
			int xIncrement = 0;
			int yIncrement = 0;

			EdgeTypes edgeType = (EdgeTypes)k;

			switch (edgeType)
			{
				case EdgeTypes.Top:
				case EdgeTypes.Bottom:
					xIncrement = 1;
					break;

				case EdgeTypes.Left:
				case EdgeTypes.Right:
					yIncrement = 1;
					break;
			}

			int edgeLength = 0;
			int originalI = i;
			int originalJ = j;

			while (edgeFlags[j, i, k])
			{
				visited[j, i, k] = true;
				edgeLength++;
				j += xIncrement;
				i += yIncrement;
			}

			Vector2 start = new Vector2(originalJ, originalI);

			switch (edgeType)
			{
				case EdgeTypes.Bottom:
					start += Vector2.UnitY;
					break;

				case EdgeTypes.Right:
					start += Vector2.UnitX;
					break;
			}

			Vector2 end = start + new Vector2(xIncrement, yIncrement) * edgeLength;
			start *= scaleMultiplier;
			end *= scaleMultiplier;

			physicsFactory.AttachEdge(body, new Edge(start, end), Units.Meters);
		}
	}
}
