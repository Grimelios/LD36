using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using LD36.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.Entities
{
	internal class Rope : Entity
	{
		private const int DefaultSegmentLength = 20;
		private const float PlayerEndOffset = 0.01f;

		private Body playerBody;
		private List<Body> bodies;
		private List<Sprite> sprites;
		private PhysicsFactory physicsFactory;
		private RevoluteJoint playerJoint;
		
		private float segmentLength;
		private float totalLength;
		private float playerOffset;

		public Rope(GrapplingHook grapple, Vector2 endPosition, Body playerBody) : base(Vector2.Zero)
		{
			this.playerBody = playerBody;

			Vector2 start = grapple.BackPosition;
			Vector2 end = endPosition;

			float distance = Vector2.Distance(start, end);
			int numSegments = (int)(distance / DefaultSegmentLength);

			if (distance % DefaultSegmentLength > 0)
			{
				numSegments++;
			}

			segmentLength = distance / numSegments;
            bodies = new List<Body>();
			sprites = new List<Sprite>();
			physicsFactory = DIKernel.Get<PhysicsFactory>();

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
			
			physicsFactory.CreateRevoluteJoint(grapple.BackBody, bodies[0], Vector2.Zero, -anchor, Units.Pixels);

			if (playerBody != null)
			{
				playerJoint = physicsFactory.CreateRevoluteJoint(playerBody, bodies[bodies.Count - 1], Vector2.Zero, anchor, Units.Pixels);
			}

			playerOffset = distance - PlayerEndOffset;
			totalLength = distance;
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
			PhysicsUtilities.RemoveJoint(playerJoint);
			playerJoint = null;
		}

		public Vector2 Climb(ref float climbSpeed, float dt)
		{
			playerOffset += climbSpeed * dt;

			if (playerOffset < 0 || playerOffset > totalLength)
			{
				playerOffset = MathHelper.Clamp(playerOffset, 0, totalLength - PlayerEndOffset);
				climbSpeed = 0;
			}

			int segmentIndex = (int)(playerOffset / segmentLength);
			float segmentAmount = (playerOffset % segmentLength) / segmentLength;

			Body segmentBody = bodies[segmentIndex];
			Vector2 segmentPosition = PhysicsConvert.ToPixels(segmentBody.Position);
			Vector2 halfLength = new Vector2(segmentLength / 2, 0);
			Vector2 halfVector = Vector2.Transform(halfLength, Matrix.CreateRotationZ(segmentBody.Rotation));
			Vector2 start = segmentPosition - halfVector;
			Vector2 end = segmentPosition + halfVector;
			Vector2 segmentAnchor = Vector2.Lerp(-halfLength, halfLength, segmentAmount);

			PhysicsUtilities.RemoveJoint(playerJoint);
			playerJoint = physicsFactory.CreateRevoluteJoint(playerBody, segmentBody, Vector2.Zero, segmentAnchor, Units.Pixels);

			return Vector2.Lerp(start, end, segmentAmount);
		}

		public override void Destroy()
		{
			bodies.ForEach(PhysicsUtilities.RemoveBody);
			
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
