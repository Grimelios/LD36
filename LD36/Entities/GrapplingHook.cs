using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using LD36.Physics;
using LD36.Timing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.Entities
{
	internal class GrapplingHook : Entity
	{
		private const int BodyWidth = 4;
		private const int BodyHeight = 4;
		private const int BackOffset = 20;
		private const int FadeDelay = 10000;
		private const int FadeTime = 250;

		private Sprite sprite;
		private Rope rope;
		private DummyRope dummyRope;
		private Vector2 dummyRopeEndOffset;
		private Timer timer;
		private PlayerCharacter player;

		private bool stuck;
		private bool fading;
		private bool playerHeld;

		public GrapplingHook(Vector2 position, PlayerCharacter player) : base(position)
		{
			this.player = player;

			sprite = new Sprite("Grapple", position, new Vector2(20, 5));
			dummyRope = new DummyRope(this);

			PhysicsFactory physicsFactory = DIKernel.Get<PhysicsFactory>();
			Body = physicsFactory.CreateRectangle(BodyWidth, BodyHeight, position, Units.Pixels, this);
			Body.IsBullet = true;
			Body.OnCollision += HandleCollision;
			BackBody = physicsFactory.CreateBody(this);
		}

		public Body Body { get; }
		public Body BackBody { get; }

		public Vector2 BackPosition { get; private set; }

		private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			Edge edge = fixtureB.UserData as Edge;

			if (edge != null && contact.IsTouching)
			{
				UpdatePositions();

				Body.BodyType = BodyType.Static;
				stuck = true;

				Vector2 ropeEndPosition = playerHeld ? player.Position : BackPosition + dummyRopeEndOffset;
				rope = new Rope(this, ropeEndPosition, playerHeld ? player.Body : null);
				EntityUtilities.AddEntity(rope);

				if (playerHeld)
				{
					player.RegisterGrappleImpact(rope);
				}

				dummyRope = null;
			}

			return false;
		}

		public void Fire(Vector2 velocity)
		{
			Body.LinearVelocity = PhysicsConvert.ToMeters(velocity);
			playerHeld = true;
			dummyRope = new DummyRope(this);
		}

		public void Release()
		{
			playerHeld = false;
			timer = new Timer(FadeDelay, () =>
			{
				fading = true;
				timer = new Timer(FadeTime, Destroy, false);
			}, false);

			if (!stuck)
			{
				dummyRopeEndOffset = player.Position - BackPosition;
			}
		}

		public override void Destroy()
		{
			EntityUtilities.RemoveEntity(this);
			PhysicsUtilities.RemoveBody(Body);

			rope?.Destroy();
		}

		public override void Update(float dt)
		{
			if (!stuck)
			{
				float rotation = GameFunctions.ComputeAngle(Body.LinearVelocity);

				Body.Rotation = rotation;
				sprite.Rotation = rotation;
				dummyRope.Position = BackPosition;
				dummyRope.EndPosition = playerHeld ? player.Position : BackPosition + dummyRopeEndOffset;

				UpdatePositions();
			}

			if (fading)
			{
				sprite.Tint = Color.Lerp(Color.White, Color.Transparent, timer.Progress);
				rope?.SetFade(sprite.Tint);
			}

			BackBody.Position = PhysicsConvert.ToMeters(BackPosition);
		}

		private void UpdatePositions()
		{
			Position = PhysicsConvert.ToPixels(Body.Position);
			sprite.Position = Position;
			BackPosition = Position - GameFunctions.ComputeDirection(sprite.Rotation) * BackOffset;
		}

		public override void Render(SpriteBatch sb)
		{
			dummyRope?.Render(sb);
			sprite.Render(sb);
		}
	}
}
