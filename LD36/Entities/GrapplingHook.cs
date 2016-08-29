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
		private Timer timer;
		private PlayerCharacter player;

		private bool stuck;
		private bool fading;

		public GrapplingHook(Vector2 position, PlayerCharacter player) : base(position)
		{
			this.player = player;

			sprite = new Sprite("Grapple", position, new Vector2(20, 5));
			Body = DIKernel.Get<PhysicsFactory>().CreateRectangle(BodyWidth, BodyHeight, position, Units.Pixels, this);
			Body.IsBullet = true;
			Body.OnCollision += HandleCollision;
		}

		public Body Body { get; }

		public Vector2 BackPosition { get; private set; }

		private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			Edge edge = fixtureB.UserData as Edge;

			if (edge != null && contact.IsTouching)
			{
				UpdatePositions();

				Body.BodyType = BodyType.Static;
				stuck = true;

				Body backAnchor = DIKernel.Get<PhysicsFactory>().CreateBody(this);
				backAnchor.Position = PhysicsConvert.ToMeters(BackPosition);
				rope = new Rope(this, player);
				EntityUtilities.AddEntity(rope);

				player.RegisterGrappleImpact(rope);
			}

			return false;
		}

		public void Fire(Vector2 velocity)
		{
			Body.LinearVelocity = PhysicsConvert.ToMeters(velocity);
		}

		public void Release()
		{
			timer = new Timer(FadeDelay, () =>
			{
				fading = true;
				timer = new Timer(FadeTime, Destroy, false);
			}, false);
		}

		public override void Destroy()
		{
			EntityUtilities.RemoveEntity(this);
			PhysicsUtilities.RemoveBody(Body);

			rope.Destroy();
		}

		public override void Update(float dt)
		{
			if (!stuck)
			{
				float rotation = GameFunctions.ComputeAngle(Body.LinearVelocity);

				Body.Rotation = rotation;
				sprite.Rotation = rotation;

				UpdatePositions();
			}

			if (fading)
			{
				sprite.Tint = Color.Lerp(Color.White, Color.Transparent, timer.Progress);
				rope.SetFade(sprite.Tint);
			}
		}

		private void UpdatePositions()
		{
			Position = PhysicsConvert.ToPixels(Body.Position);
			sprite.Position = Position;
			BackPosition = Position - GameFunctions.ComputeDirection(sprite.Rotation) * BackOffset;
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
