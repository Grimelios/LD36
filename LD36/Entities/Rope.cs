using System.Collections.Generic;
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

		public Rope(GrapplingHook grapple, PlayerCharacter player) : base(Vector2.Zero)
		{
			Vector2 start = grapple.BackPosition;
			Vector2 end = player.Position;

			CreateEndWeight(player);

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

			Body backBody = physicsFactory.CreateBody(grapple);
			backBody.Position = PhysicsConvert.ToMeters(grapple.BackPosition);

			physicsFactory.CreateRevoluteJoint(backBody, bodies[0], Vector2.Zero, -anchor, Units.Pixels);
			physicsFactory.CreateRevoluteJoint(EndWeight, bodies[bodies.Count - 1], Vector2.Zero, anchor, Units.Pixels);
		}

		public Body EndWeight { get; private set; }

		private void CreateEndWeight(PlayerCharacter player)
		{
			Body playerBody = player.Body;

			EndWeight = DIKernel.Get<PhysicsFactory>().CreateRectangle(1, 1, playerBody.Position, Units.Meters, this);
			EndWeight.FixedRotation = true;
			EndWeight.LinearVelocity = playerBody.LinearVelocity;
			EndWeight.OnCollision += HandleCollision;
		}

		private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			return fixtureB.UserData is Edge;
		}

		public void SetFade(Color tint)
		{
			sprites.ForEach(s => s.Tint = tint);
		}

		public void RegisterPlayerDetach()
		{
			EndWeight.CollidesWith = Category.None;
		}

		public override void Destroy()
		{
			bodies.ForEach(PhysicsUtilities.RemoveBody);

			PhysicsUtilities.RemoveBody(EndWeight);
			EntityUtilities.RemoveEntity(this);
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
