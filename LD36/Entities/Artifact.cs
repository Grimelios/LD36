using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using LD36.Interfaces;
using LD36.Messaging;
using LD36.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.Entities
{
	internal class Artifact : Entity, IInteractive
	{
		private const int BodyWidth = 2;
		private const int BodyHeight = 3;

		private Body body;
		private Sprite sprite;

		public Artifact(Vector2 position) : base(position)
		{
			sprite = new Sprite("Artifact", position);
			body = DIKernel.Get<PhysicsFactory>().CreateRectangle(BodyWidth, BodyHeight, PhysicsConvert.ToMeters(position), Units.Meters, this);
			body.BodyType = BodyType.Static;
			body.OnCollision += HandleCollision;
		}

		private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			return false;
		}

		public void InteractionResponse()
		{
			DIKernel.Get<MessageSystem>().Send(new ArtifactMessage());
			EntityUtilities.RemoveEntity(this);
		}

		public override void Update(float dt)
		{
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
