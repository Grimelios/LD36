using System;
using FarseerPhysics.Dynamics;
using LD36.Interfaces;
using LD36.Messaging;
using LD36.Physics;
using LD36.Timing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.Entities
{
	internal class Artifact : Entity, IInteractive, IMessageReceiver
	{
		private const int BodyWidth = 2;
		private const int BodyHeight = 3;
		private const int SpinTime = 1500;
		private const int CollectionTime = 750;
		private const int ScaleTime = 100;
		private const int RotationTime = 10000;
		private const int RotationRadius = 70;
		private const float CollectedScale = 0.4f;

		private static int artifactsCollected;

		private static Timer rotationTimer;

		private Body body;
		private Sprite sprite;
		private Vector2 baseCollectionPosition;
		private PlayerCharacter player;
		private Timer spinTimer;
		private Timer collectionTimer;
		private Timer scaleTimer;

		private float baseScaleX;
		private float rotationAngleOffset;
		private float maximumScale;
		private bool collected;
		private int collectionOrder;

		public Artifact(Vector2 position) : base(position)
		{
			sprite = new Sprite("Artifact", position);
			body = DIKernel.Get<PhysicsFactory>().CreateRectangle(BodyWidth, BodyHeight, PhysicsConvert.ToMeters(position), Units.Meters, this);
			body.IgnoreGravity = true;
			body.IsSensor = true;
			maximumScale = 1;

			spinTimer = new Timer(SpinTime, () =>
			{
				sprite.Scale = new Vector2(maximumScale);
			}, true);
		}

		public void InteractionResponse(PlayerCharacter player)
		{
			this.player = player;

			baseCollectionPosition = Position;
			baseScaleX = sprite.Scale.X;
			collectionTimer = new Timer(CollectionTime, Collect, false);
			spinTimer.Paused = true;

			PhysicsUtilities.RemoveBody(body);
		}

		public void Receive(GameMessage message)
		{
			float angleIncrement = MathHelper.TwoPi / artifactsCollected;
			rotationAngleOffset = angleIncrement * collectionOrder;
		}

		private void Collect()
		{
			if (rotationTimer == null)
			{
				rotationTimer = new Timer(RotationTime, () => { }, true);
			}

			collectionTimer = null;
			scaleTimer = new Timer(ScaleTime, () =>
			{
				scaleTimer = null;
				spinTimer.Paused = false;
				spinTimer.Elapsed = 0;
			}, false);

			maximumScale = CollectedScale;
			collectionOrder = artifactsCollected;
			artifactsCollected++;
			collected = true;

			MessageSystem messageSystem = DIKernel.Get<MessageSystem>();
			messageSystem.Subscribe(MessageTypes.Artifact, this);
			messageSystem.Send(new ArtifactMessage());
		}

		public override void Destroy()
		{
		}

		public override void Update(float dt)
		{
			sprite.Position = Position;

			if (collectionTimer != null)
			{
				float amount = collectionTimer.Progress * collectionTimer.Progress;
				float scaleX = MathHelper.Lerp(baseScaleX, 0, amount);
				float scaleY = MathHelper.Lerp(1, 0, amount);

				Position = Vector2.Lerp(baseCollectionPosition, player.Position, amount);
				sprite.Scale = new Vector2(scaleX, scaleY);
			}
			else
			{
				UpdateSpin();

				if (scaleTimer != null)
				{
					sprite.Scale = Vector2.Lerp(Vector2.Zero, new Vector2(CollectedScale), scaleTimer.Progress);
				}

				if (collected)
				{
					float angle = MathHelper.Lerp(0, MathHelper.TwoPi, rotationTimer.Progress) + rotationAngleOffset;
					float x = (float)Math.Cos(angle) * RotationRadius;
					float y = (float)Math.Sin(angle) * RotationRadius;

					Position = player.Position + new Vector2(x, y);
				}
			}
		}

		private void UpdateSpin()
		{
			float progress = spinTimer.Progress;
			float amount = progress <= 0.5f ? progress * 2 : 1 - (progress - 0.5f) * 2;
			amount *= amount;
			
			float spriteScale = MathHelper.Lerp(maximumScale, 0, amount);
			sprite.Scale = new Vector2(spriteScale, sprite.Scale.Y);
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
