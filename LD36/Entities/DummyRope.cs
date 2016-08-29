using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.Entities
{
	internal class DummyRope : Entity
	{
		private GrapplingHook grapple;
		private Texture2D segmentTexture;
		private Vector2 origin;

		public DummyRope(GrapplingHook grapple) : base(grapple.BackPosition)
		{
			this.grapple = grapple;

			segmentTexture = ContentLoader.LoadTexture("RopeShort");
			origin = new Vector2(segmentTexture.Width, segmentTexture.Height) / 2;
			EndPosition = grapple.BackPosition;
		}

		public Vector2 EndPosition { get; set; }

		public override void Destroy()
		{
		}

		public override void Update(float dt)
		{
		}

		public override void Render(SpriteBatch sb)
		{
			float length = Vector2.Distance(EndPosition, grapple.BackPosition);
			float rotation = GameFunctions.ComputeAngle(EndPosition, grapple.BackPosition);

			Rectangle sourceRect = new Rectangle(0, 0, (int)length, segmentTexture.Height);

			sb.Draw(segmentTexture, EndPosition, sourceRect, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
		}
	}
}
