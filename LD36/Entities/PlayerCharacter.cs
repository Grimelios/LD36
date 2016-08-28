using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using LD36.Input;
using LD36.Interfaces;
using LD36.Messaging;
using LD36.Messaging.Input;
using LD36.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD36.Entities
{
	internal class PlayerCharacter : Entity, IMessageReceiver
	{
		private const int Acceleration = 2000;
		private const int Deceleration = 1500;
		private const int MaximumSpeed = 800;

		private Sprite sprite;
		private Body body;

		private bool runningLeft;
		private bool runningRight;

		public PlayerCharacter(Vector2 position) : base(position)
		{
			sprite = new Sprite("Player", position);
			body = DIKernel.Get<PhysicsFactory>().CreateRectangle(1, 1, PhysicsConvert.ToMeters(position), Units.Meters, this);
			body.Friction = 0;
			body.FixedRotation = true;

			MessageSystem messageSystem = DIKernel.Get<MessageSystem>();
			messageSystem.Subscribe(MessageTypes.Keyboard, this);
			messageSystem.Subscribe(MessageTypes.Mouse, this);
		}

		public void Receive(GameMessage message)
		{
			switch (message.Type)
			{
				case MessageTypes.Keyboard:
					HandleKeyboard(((KeyboardMessage)message).Data);
					break;

				case MessageTypes.Mouse:
					HandleMouse(((MouseMessage)message).Data);
					break;
			}
		}

		public void HandleKeyboard(KeyboardData data)
		{
			HandleInteraction(data);
			HandleRunning(data);
		}

		private void HandleInteraction(KeyboardData data)
		{
			if (data.KeysPressedThisFrame.Contains(Keys.E))
			{
				ContactEdge contactEdge = body.ContactList;

				while (contactEdge != null)
				{
					IInteractive item = contactEdge.Contact.FixtureB.Body.UserData as IInteractive;

					if (item != null)
					{
						item.InteractionResponse();

						return;
					}

					contactEdge = contactEdge.Next;
				}
			}
		}

		private void HandleRunning(KeyboardData data)
		{
			bool aDown = data.KeysDown.Contains(Keys.A);
			bool dDown = data.KeysDown.Contains(Keys.D);

			if (aDown && !dDown)
			{
				runningLeft = true;
				runningRight = false;
			}
			else if (dDown && !aDown)
			{
				runningRight = true;
				runningLeft = false;
			}
			else
			{
				runningLeft = false;
				runningRight = false;
			}
		}

		public void HandleMouse(MouseData data)
		{
		}

		public override void Update(float dt)
		{
			Vector2 convertedPosition = PhysicsConvert.ToPixels(body.Position);
			Position = new Vector2((int)convertedPosition.X, (int)convertedPosition.Y);
			sprite.Position = Position;

			UpdateRunning(dt);
		}

		private void UpdateRunning(float dt)
		{
			Vector2 velocity = PhysicsConvert.ToPixels(body.LinearVelocity);

			// These values are mutually exclusive.
			if (runningLeft || runningRight)
			{
				float acceleration = runningLeft ? -Acceleration : Acceleration;
				velocity.X += acceleration * dt;

				if (Math.Abs(velocity.X) > MaximumSpeed)
				{
					velocity.X = Math.Sign(velocity.X) * MaximumSpeed;
				}
			}
			else
			{
				int previousSign = Math.Sign(velocity.X);
				velocity.X -= Deceleration * previousSign * dt;

				if (Math.Sign(velocity.X) != previousSign)
				{
					velocity.X = 0;
				}
			}

			body.LinearVelocity = PhysicsConvert.ToMeters(velocity);
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
