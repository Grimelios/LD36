using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using LD36.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.Physics
{
	internal class PhysicsDebug : IRenderable
	{
		private World world;

		public PhysicsDebug(World world)
		{
			this.world = world;
		}

		public void Render(SpriteBatch sb)
		{
			foreach (Body body in world.BodyList)
			{
				foreach (Fixture fixture in body.FixtureList)
				{
					Shape shape = fixture.Shape;

					switch (shape.ShapeType)
					{
						case ShapeType.Edge:
							RenderEdge(sb, (EdgeShape)shape);
							break;
					}
				}
			}
		}

		private void RenderEdge(SpriteBatch sb, EdgeShape shape)
		{
			Vector2 start = PhysicsConvert.ToPixels(shape.Vertex1);
			Vector2 end = PhysicsConvert.ToPixels(shape.Vertex2);

			RenderFunctions.RenderLine(sb, start, end, Color.DeepPink);
		}
	}
}
