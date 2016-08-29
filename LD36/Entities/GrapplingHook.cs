using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using LD36.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.Entities
{
	internal class GrapplingHook : Entity
	{
		private const int BodyWidth = 4;
		private const int BodyHeight = 4;
		private const int BackOffset = 20;

		private Sprite sprite;
		private Body body;
		private PlayerCharacter player;

		private bool stuck;

		public GrapplingHook(Vector2 position, PlayerCharacter player) : base(position)
		{
			this.player = player;

			sprite = new Sprite("Grapple", position, new Vector2(20, 5));
			body = DIKernel.Get<PhysicsFactory>().CreateRectangle(BodyWidth, BodyHeight, position, Units.Pixels, this);
			body.IsBullet = true;
			body.OnCollision += HandleCollision;
		}

		public Vector2 BackPosition { get; private set; }

		private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			Edge edge = fixtureB.UserData as Edge;

			if (edge != null && contact.IsTouching)
			{
				UpdatePositions();

				body.BodyType = BodyType.Static;
				stuck = true;
				player.RegisterGrappleImpact();

				Body backAnchor = DIKernel.Get<PhysicsFactory>().CreateBody(this);
				backAnchor.Position = PhysicsConvert.ToMeters(BackPosition);

				Rope rope = new Rope(backAnchor, player.Body);
				EntityUtilities.AddEntity(rope);
			}

			return false;
		}

		public void Fire(Vector2 velocity)
		{
			body.LinearVelocity = PhysicsConvert.ToMeters(velocity);
		}

		public override void Update(float dt)
		{
			if (!stuck)
			{
				float rotation = GameFunctions.ComputeAngle(body.LinearVelocity);

				body.Rotation = rotation;
				sprite.Rotation = rotation;

				UpdatePositions();
			}
		}

		private void UpdatePositions()
		{
			Position = PhysicsConvert.ToPixels(body.Position);
			sprite.Position = Position;
			BackPosition = Position - GameFunctions.ComputeDirection(sprite.Rotation) * BackOffset;
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
