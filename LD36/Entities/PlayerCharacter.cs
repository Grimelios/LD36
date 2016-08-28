﻿using System;
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
		private const int Deceleration = 3000;
		private const int MaximumSpeed = 450;
		private const int JumpSpeedInitial = 500;
		private const int JumpSpeedLimited = 50;

		private Sprite sprite;
		private Body body;
		private Artifact activeArtifact;

		private bool runningLeft;
		private bool runningRight;
		private bool onGround;

		public PlayerCharacter(Vector2 position) : base(position)
		{
			sprite = new Sprite("Player", position);
			body = DIKernel.Get<PhysicsFactory>().CreateRectangle(1, 1, PhysicsConvert.ToMeters(position), Units.Meters, this);
			body.Friction = 0;
			body.FixedRotation = true;
			body.OnCollision += HandleCollision;
			body.OnSeparation += HandleSeparation;

			MessageSystem messageSystem = DIKernel.Get<MessageSystem>();
			messageSystem.Subscribe(MessageTypes.Keyboard, this);
			messageSystem.Subscribe(MessageTypes.Mouse, this);
		}

		private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			Edge edge = fixtureB.UserData as Edge;

			if (edge != null)
			{
				if (edge.IsGround)
				{
					onGround = true;
				}
			}
			else
			{
				activeArtifact = fixtureB.Body.UserData as Artifact;
			}

			return true;
		}

		private void HandleSeparation(Fixture fixtureA, Fixture fixtureB)
		{
			if (fixtureB.Body.UserData == activeArtifact)
			{
				activeArtifact = null;
			}
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
			HandleJumping(data);
		}

		private void HandleInteraction(KeyboardData data)
		{
			if (data.KeysPressedThisFrame.Contains(Keys.E))
			{
				activeArtifact?.InteractionResponse();

				/*
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
				*/
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

		private void HandleJumping(KeyboardData data)
		{
			Vector2 velocity = PhysicsConvert.ToPixels(body.LinearVelocity);

			if (onGround)
			{
				if (data.KeysPressedThisFrame.Contains(Keys.Space))
				{
					velocity.Y = -JumpSpeedInitial;
					//onGround = false;
				}
			}
			else if (data.KeysReleasedThisFrame.Contains(Keys.Space) && velocity.Y < -JumpSpeedLimited)
			{
				velocity.Y = -JumpSpeedLimited;
			}

			body.LinearVelocity = PhysicsConvert.ToMeters(velocity);
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
