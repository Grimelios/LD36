using FarseerPhysics.Dynamics;
using LD36.Interfaces;
using LD36.Messaging;
using LD36.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.Entities
{
	internal class Artifact : Entity, IInteractive
	{
		private const int BodyWidth = 1;
		private const int BodyHeight = 1;

		private Body body;
		private Sprite sprite;

		public Artifact(Vector2 position) : base(position)
		{
			sprite = new Sprite("Artifact", position);
			body = DIKernel.Get<PhysicsFactory>().CreateRectangle(BodyWidth, BodyHeight, PhysicsConvert.ToMeters(position), Units.Meters, this);
			body.IgnoreGravity = true;
			body.IsSensor = true;
		}

		public void InteractionResponse()
		{
			DIKernel.Get<MessageSystem>().Send(new ArtifactMessage());
			EntityUtilities.RemoveEntity(this);
		}

		public override void Update(float dt)
		{
			sprite.Position = PhysicsConvert.ToPixels(body.Position);
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
