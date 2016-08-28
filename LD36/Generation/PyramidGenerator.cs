using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FarseerPhysics.Dynamics;
using LD36.Entities;
using LD36.Entities.Environment;
using LD36.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.Generation
{
	using ConnectorMap = Dictionary<Tuple<int, int>, List<Point>>;

	internal class PyramidGenerator
	{
		private const int RoomMinimumSize = 6;
		private const int RoomMaximumSize = 12;
		private const int MaximumPlacementAttempts = 12;
		private const int IntroRoomWidth = 12;
		private const int IntroRoomHeight = 8;
		private const float ContinuationDiminishingFactor = 0.9f;

		private enum MazeDirections
		{
			FromTop = 1,
			FromBottom = 0,
            FromLeft = 3,
			FromRight = 2,
			None = 5
		}

		private int pyramidWidth;
		private int pyramidHeight;
		private int scaleMultiplier;
		private int maximumRoomRegion;
		private int[,] fullTiles;

		private Random random;
		private List<Rectangle> rooms;

		public PyramidGenerator(int pyramidSize, int scaleMultiplier)
		{
			this.scaleMultiplier = scaleMultiplier;

			pyramidWidth = pyramidSize * 2;
			pyramidHeight = pyramidSize;
			random = new Random();
		}

		public Texture2D Texture { get; private set; }

		public void Generate()
		{
			rooms = new List<Rectangle>();

			//Rectangle introRoom = new Rectangle(IntroRoomWidth / 2, pyramidHeight - IntroRoomHeight - 1, IntroRoomWidth, IntroRoomHeight);
			//rooms.Add(introRoom);

			GenerateRooms();
			GenerateHallways();
			GenerateConnectors();
			BlockHallways();
			GenerateTexture();

			EdgeGenerator edgeGenerator = new EdgeGenerator(pyramidWidth, pyramidHeight, scaleMultiplier);
			edgeGenerator.Generate(fullTiles);

			TileGenerator tileGenerator = new TileGenerator(pyramidWidth, pyramidHeight, scaleMultiplier);
			tileGenerator.Generate(fullTiles);
		}

		private void GenerateRooms()
		{
			while (true)
			{
				int width = GameFunctions.GetRandomValue(RoomMinimumSize, RoomMaximumSize);
				int height = GameFunctions.GetRandomValue(RoomMinimumSize, RoomMaximumSize);
				int placementAttempts = 0;

				Rectangle tentativeRoom = new Rectangle(0, 0, width, height);

				while (placementAttempts < MaximumPlacementAttempts)
				{
					int x = GameFunctions.GetRandomValue(height + 2, pyramidWidth - height - width - 2);
					int y = GameFunctions.GetRandomValue(ComputeMinimumY(x, width, height), pyramidHeight - height - 1);

					tentativeRoom.X = x;
					tentativeRoom.Y = y;

					if (CheckValidRoom(tentativeRoom))
					{
						rooms.Add(tentativeRoom);

						break;
					}

					placementAttempts++;
				}

				if (placementAttempts == MaximumPlacementAttempts)
				{
					return;
				}
			}
		}

		private int ComputeMinimumY(int x, int width, int height)
		{
			if (x < (pyramidWidth - width) / 2)
			{
				return pyramidHeight - x + 1;
			}

			int rightSide = x + width;

			if (rightSide > (pyramidWidth + width) / 2)
			{
				return pyramidHeight - ComputeEffectiveX(rightSide) + 1;
			}

			return width / 2 + 1;
		}

		private bool CheckValidRoom(Rectangle tentativeRoom)
		{
			return !rooms.Any(room => room.Intersects(tentativeRoom));
		}
		
		private void GenerateHallways()
		{
			int regionCounter = 0;

			fullTiles = new int[pyramidWidth, pyramidHeight];

			for (int i = 0; i < pyramidHeight; i++)
			{
				for (int j = 0; j < pyramidWidth; j++)
				{
					fullTiles[j, i] = -1;
				}
			}

			foreach (Rectangle room in rooms)
			{
				for (int i = room.Y; i < room.Y + room.Height; i++)
				{
					for (int j = room.X; j < room.X + room.Width; j++)
					{
						fullTiles[j, i] = regionCounter;
					}
				}

				regionCounter++;
			}

			maximumRoomRegion = regionCounter - 1;
			
			for (int i = 1; i < pyramidHeight - 1; i++)
			{
				for (int j = 1; j < pyramidWidth - 1; j++)
				{
					if (CheckValidMazeStart(i, j))
					{
						if (GenerateMaze(i, j, regionCounter, MazeDirections.None, 0))
						{
							regionCounter++;
						}
					}
				}
			}
		}

		private bool CheckValidMazeStart(int i, int j)
		{
			// -1 represents an empty tile.
			if (fullTiles[j, i] != -1)
			{
				return false;
			}
			
			return CheckWithinPyramid(j, i);
		}

		private int ComputeEffectiveX(int x)
		{
			return x <= pyramidWidth / 2 ? x : x - (x - pyramidWidth / 2) * 2;
		}

		private bool CheckWithinPyramid(int x, int y)
		{
			return y > pyramidWidth / 2 - ComputeEffectiveX(x) + 1;
		}
		
		private bool GenerateMaze(int i, int j, int regionCounter, MazeDirections fromDirection, float continuationChance)
		{
			if (fullTiles[j, i] != -1 || !CheckValidMazeLocation(i, j, fromDirection))
			{
				return false;
			}

			fullTiles[j, i] = regionCounter;

			// The order here is up, down, left, right (and the order does matter because it matches the enumeration).
			Point[] surroundingPoints = GenerationUtilities.GetSurroundingPoints(fullTiles, i, j);
			List<int> pointIndices = new List<int>(4);

			for (int k = 0; k < pointIndices.Capacity; k++)
			{
				pointIndices.Add(k);
			}

			List<int> pointOrder = new List<int>(4);

			int continuationDirection = (int)fromDirection;

			// This step gives hallways a bias towards continuing in their same direction, with decreasing chance the longer the hallway runs.
			if ((float)random.NextDouble() < continuationChance)
			{
				pointOrder.Add(continuationDirection);
				pointIndices.RemoveAt(continuationDirection);
				continuationChance *= ContinuationDiminishingFactor;
			}
			else
			{
				continuationChance = 1;
			}

			while (pointIndices.Count > 0)
			{
				int index = random.Next(pointIndices.Count);
				pointOrder.Add(pointIndices[index]);
				pointIndices.RemoveAt(index);
			}

			foreach (int index in pointOrder)
			{
				Point point = surroundingPoints[index];
				GenerateMaze(point.Y, point.X, regionCounter, (MazeDirections)index, continuationChance);
			}

			return true;
		}

		private bool CheckValidMazeLocation(int i, int j, MazeDirections fromDirection)
		{
			if (!CheckInBounds(i, j) || !CheckWithinPyramid(j, i))
			{
				return false;
			}

			int nonEmptyCount = 0;

			foreach (int value in GenerationUtilities.GetSurroundingValues(fullTiles, i, j))
			{
				if (value != -1)
				{
					// During this step of the generation process, no hallway can touch any room.
					if (value <= maximumRoomRegion)
					{
						return false;
					}

					nonEmptyCount++;

					if (nonEmptyCount == 2)
					{
						return false;
					}
				}
			}

			return CheckDiagonals(i, j, fromDirection);
		}

		private bool CheckInBounds(int i, int j)
		{
			return i > 0 && i < pyramidHeight - 1 && j > 0 && j < pyramidWidth - 1;
		}

		private bool CheckDiagonals(int i, int j, MazeDirections fromDirection)
		{
			List<Point> checkPoints = new List<Point>();
			Point topLeft = new Point(j - 1, i - 1);
			Point topRight = new Point(j + 1, i - 1);
			Point bottomLeft = new Point(j - 1, i + 1);
			Point bottomRight = new Point(j + 1, i + 1);

			switch (fromDirection)
			{
				case MazeDirections.FromTop:
					checkPoints.Add(bottomLeft);
					checkPoints.Add(bottomRight);

					break;

				case MazeDirections.FromBottom:
					checkPoints.Add(topLeft);
					checkPoints.Add(topRight);

					break;

				case MazeDirections.FromLeft:
					checkPoints.Add(topRight);
					checkPoints.Add(bottomRight);

					break;

				case MazeDirections.FromRight:
					checkPoints.Add(topLeft);
					checkPoints.Add(bottomLeft);

					break;

				case MazeDirections.None:
					checkPoints.Add(topLeft);
					checkPoints.Add(topRight);
					checkPoints.Add(bottomLeft);
					checkPoints.Add(bottomRight);

					break;
			}

			foreach (Point point in checkPoints)
			{
				if (fullTiles[point.X, point.Y] != -1)
				{
					return false;
				}
			}

			return true;
		}

		private void GenerateConnectors()
		{
			ConnectorMap connectorMap = GetConnectorMap();

			foreach (List<Point> connectorPoints in connectorMap.Values)
			{
				Point point = connectorPoints[random.Next(connectorPoints.Count)];
				fullTiles[point.X, point.Y] = 1000;
			}
		}

		private ConnectorMap GetConnectorMap()
		{
			// After spending some time with connectors, Region and Connector objects aren't even needed. Integer region values and connector
			// locations are good enough.
			ConnectorMap connectorMap = new ConnectorMap();

			for (int i = 1; i < pyramidHeight - 1; i++)
			{
				for (int j = 1; j < pyramidWidth - 1; j++)
				{
					if (fullTiles[j, i] == -1)
					{
						// If a point is a connector, it will either connect vertically or horizontally, but not both.
						int[] surroundingValues = GenerationUtilities.GetSurroundingValues(fullTiles, i, j);
						int value1 = -1;
						int value2 = -1;

						// Vertical connector.
						if (surroundingValues[0] != -1 && surroundingValues[1] != -1)
						{
							value1 = surroundingValues[0];
							value2 = surroundingValues[1];
						}
						// Horizontal connectors.
						else if (surroundingValues[2] != -1 && surroundingValues[3] != -1)
						{
							value1 = surroundingValues[2];
							value2 = surroundingValues[3];
						}

						// Both values are always set at the same time, so checking just one value is sufficient.
						if (value1 != -1 && value1 != value2)
						{
							int region1 = MathHelper.Min(value1, value2);
							int region2 = MathHelper.Max(value1, value2);

							Tuple<int, int> regionTuple = new Tuple<int, int>(region1, region2);

							if (!connectorMap.ContainsKey(regionTuple))
							{
								connectorMap.Add(regionTuple, new List<Point>());
							}

							connectorMap[regionTuple].Add(new Point(j, i));
						}
					}
				}
			}

			return connectorMap;
		}

		private void BlockHallways()
		{
			for (int i = 1; i < pyramidHeight - 1; i++)
			{
				for (int j = 1; j < pyramidWidth - 1; j++)
				{
					RecursiveFill(i, j);
				}
			}
		}

		private void RecursiveFill(int i, int j)
		{
			if (CheckInBounds(i, j) && CheckDeadEnd(i, j))
			{
				fullTiles[j, i] = -1;

				foreach (Point point in GenerationUtilities.GetSurroundingPoints(fullTiles, i, j))
				{
					RecursiveFill(point.Y, point.X);
				}
			}
		}

		private bool CheckDeadEnd(int i, int j)
		{
			if (fullTiles[j, i] == -1)
			{
				return false;
			}

			int[] surroundingValues = GenerationUtilities.GetSurroundingValues(fullTiles, i, j);
			int nonEmptyTiles = 0;

			foreach (int value in surroundingValues)
			{
				if (value != -1)
				{
					nonEmptyTiles++;

					if (nonEmptyTiles == 2)
					{
						return false;
					}
				}
			}

			return nonEmptyTiles == 1;
		}

		private void GenerateTexture()
		{
			Color[] data = new Color[pyramidWidth * pyramidHeight];

			for (int i = 0; i < data.Length; i++)
			{
				data[i] = Color.Transparent;
			}

			for (int i = 0; i < pyramidHeight; i++)
			{
				int startingIndex = i * pyramidWidth;

				for (int j = 0; j < pyramidWidth; j++)
				{
					int value = fullTiles[j, i];

					if (value == 1000)
					{
						data[startingIndex + j] = Color.DarkOrange;
					}
					else if (value != -1)
					{
						data[startingIndex + j] = Color.DarkGreen;
					}
				}
			}

			Texture = new Texture2D(DIKernel.Get<GraphicsDevice>(), pyramidWidth, pyramidHeight);
			Texture.SetData(data);
		}
	}
}
