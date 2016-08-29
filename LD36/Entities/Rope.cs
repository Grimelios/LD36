using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using LD36.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.Entities
{
	internal class Rope : Entity
	{
		private const int DefaultSegmentLength = 20;

		private List<Body> bodies;
		private List<Sprite> sprites;

		private float segmentLength;

		public Rope(Body anchor1, Body anchor2) : base(Vector2.Zero)
		{
			Vector2 start = PhysicsConvert.ToPixels(anchor1.Position);
			Vector2 end = PhysicsConvert.ToPixels(anchor2.Position);

			float distance = Vector2.Distance(start, end);
			int numSegments = (int)(distance / DefaultSegmentLength);

			if (distance % DefaultSegmentLength > 0)
			{
				numSegments++;
			}

			segmentLength = distance / numSegments;
            bodies = new List<Body>();
			sprites = new List<Sprite>();

			PhysicsFactory physicsFactory = DIKernel.Get<PhysicsFactory>();
			Vector2 direction = Vector2.Normalize(end - start);
			Vector2 positionIncrement = direction * segmentLength;
			Vector2 halfIncrement = positionIncrement / 2;
			Vector2 startPosition = start + direction * halfIncrement;
			Vector2 anchor = new Vector2(segmentLength / 2, 0);

			for (int i = 0; i < numSegments; i++)
			{
				Body body = physicsFactory.CreateRectangle(segmentLength, 2, startPosition + positionIncrement * i, Units.Pixels, this);
				body.OnCollision += HandleCollision;
				bodies.Add(body);

				if (i > 0)
				{
					physicsFactory.CreateRevoluteJoint(body, bodies[i - 1], -anchor, anchor, Units.Pixels);
				}

				sprites.Add(new Sprite("Rope", Vector2.Zero));
			}

			physicsFactory.CreateRevoluteJoint(anchor1, bodies[0], Vector2.Zero, -anchor, Units.Pixels);
			physicsFactory.CreateRevoluteJoint(anchor2, bodies[bodies.Count - 1], Vector2.Zero, anchor, Units.Pixels);
		}

		private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			return fixtureB.UserData is Edge;
		}

		public override void Update(float dt)
		{
		}

		public override void Render(SpriteBatch sb)
		{
			for (int i = 0; i < bodies.Count; i++)
			{
				Body body = bodies[i];
				Sprite sprite = sprites[i];
				sprite.Position = PhysicsConvert.ToPixels(body.Position);
				sprite.Rotation = body.Rotation;
				sprite.Render(sb);
			}
		}
	}
}
