using FarseerPhysics.Dynamics;
using LD36.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using static LD36.Constants;

namespace LD36.Entities.Environment
{
	internal class Tilemap : Entity
	{
		private const int TilesPerRow = 8;

		private int width;
		private int height;
		private int[,] tiles;

		private Body body;
		private Texture2D tilesheet;

		public Tilemap(TilemapData data) :
			this(data.Position, data.Width, data.Height, data.Tiles, data.TilesheetFilename)
		{
			PhysicsFactory physicsFactory = DIKernel.Get<PhysicsFactory>();
			body = physicsFactory.CreateBody(this);

			Vector2 convertedPosition = PhysicsConvert.ToMeters(data.Position);

			foreach (Edge edge in data.Edges)
			{
				Vector2 start = convertedPosition + edge.Start;
				Vector2 end = convertedPosition + edge.End;

				physicsFactory.AttachEdge(body, new Edge(start, end, edge.IsGround), Units.Meters);
			}
		}

		public Tilemap(Vector2 position, int width, int height, int[,] tiles, string tilesheetFilename) : base(position)
		{
			this.width = width;
			this.height = height;
			this.tiles = tiles;

			tilesheet = ContentLoader.LoadTexture("Tilesheets/" + tilesheetFilename);
		}

		public override void Update(float dt)
		{
		}

		public override void Render(SpriteBatch sb)
		{
			Rectangle sourceRect = new Rectangle(0, 0, TileSize, TileSize);

			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					int tileValue = tiles[j, i];

					// -1 indicates a blank tile.
					if (tileValue != -1)
					{
						sourceRect.X = (tileValue % TilesPerRow) * TileSize;
						sourceRect.Y = (tileValue / TilesPerRow) * TileSize;

						sb.Draw(tilesheet, Position + new Vector2(j, i) * TileSize, sourceRect, Color.White);
					}
				}
			}
		}
	}
}
