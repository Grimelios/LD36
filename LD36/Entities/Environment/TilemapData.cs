using System.Collections.Generic;
using LD36.Physics;
using Microsoft.Xna.Framework;

namespace LD36.Entities.Environment
{
	internal class TilemapData
	{
		public TilemapData(Vector2 position, int width, int height, int[,] tiles, string tilesheetFilename, List<Edge> edges)
		{
			Position = position;
			Width = width;
			Height = height;
			Tiles = tiles;
			TilesheetFilename = tilesheetFilename;
			Edges = edges;
		}

		public Vector2 Position { get; }

		public int Width { get; }
		public int Height { get; }
		public int[,] Tiles { get; }
		public string TilesheetFilename { get; }

		public List<Edge> Edges { get; }
	}
}
