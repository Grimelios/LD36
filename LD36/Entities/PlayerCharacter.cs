using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
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
		private const int BodyWidth = 32;
		private const int BodyHeight = 32;
		private const int Acceleration = 2000;
		private const int Deceleration = 3000;
		private const int MaximumSpeed = 450;
		private const int JumpSpeedInitial = 500;
		private const int JumpSpeedLimited = 100;
		private const int GrappleFireSpeed = 1500;
		private const int GrappleProximityLimit = 50;
		private const int SwingImpulse = 4;
		private const int ClimbAccelerationUp = 1000;
		private const int ClimbAccelerationDown = 1200;
		private const int ClimbDecelerationUp = 800;
		private const int ClimbDecelerationDown = 1000;
		private const int ClimbMaxSpeedUp = 175;
		private const int ClimbMaxSpeedDown = 275;

		private Sprite sprite;
		private Artifact activeArtifact;
		private GrapplingHook grapple;
		private Rope rope;
		private Texture2D ropeShortTexture;

		private bool runningLeft;
		private bool runningRight;
		private bool onGround;
		private bool attachedToRope;
		private bool climbingUp;
		private bool climbingDown;

		private float climbSpeed;

		public PlayerCharacter(Vector2 position) : base(position)
		{
			sprite = new Sprite("Player", position);
			ropeShortTexture = ContentLoader.LoadTexture("RopeShort");
			Body = DIKernel.Get<PhysicsFactory>().CreateRectangle(BodyWidth, BodyHeight, position, Units.Pixels, this);
			Body.Friction = 0;
			Body.FixedRotation = true;
			Body.OnCollision += HandleCollision;
			Body.OnSeparation += HandleSeparation;

			MessageSystem messageSystem = DIKernel.Get<MessageSystem>();
			messageSystem.Subscribe(MessageTypes.Keyboard, this);
			messageSystem.Subscribe(MessageTypes.Mouse, this);
		}

		public Body Body { get; }

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
			HandleJumping(data);

			if (attachedToRope)
			{
				HandleRope(data);
			}
			else
			{
				HandleRunning(data);
			}
		}

		private void HandleInteraction(KeyboardData data)
		{
			if (data.KeysPressedThisFrame.Contains(Keys.E))
			{
				activeArtifact?.InteractionResponse(this);
				activeArtifact = null;
			}
		}

		private void HandleRope(KeyboardData data)
		{
			HandleSwinging(data);
			HandleClimbing(data);
		}

		private void HandleSwinging(KeyboardData data)
		{
			bool aDown = data.KeysDown.Contains(Keys.A);
			bool dDown = data.KeysDown.Contains(Keys.D);

			if (aDown ^ dDown)
			{
				float impulse = PhysicsConvert.ToMeters(SwingImpulse);
				impulse = aDown ? -impulse : impulse;
				Body.ApplyLinearImpulse(new Vector2(impulse, 0));
			}
		}

		private void HandleClimbing(KeyboardData data)
		{
			bool wDown = data.KeysDown.Contains(Keys.W);
			bool sDown = data.KeysDown.Contains(Keys.S);

			if (wDown && !sDown)
			{
				climbingUp = true;
				climbingDown = false;
			}
			else if (sDown && !wDown)
			{
				climbingUp = false;
				climbingDown = true;
			}
			else
			{
				climbingUp = false;
				climbingDown = false;
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
			bool spacePressedThisFrame = data.KeysPressedThisFrame.Contains(Keys.Space);

			if (attachedToRope)
			{
				if (spacePressedThisFrame)
				{
					DetachFromRope();
					Jump();
				}
			}
			else if (onGround)
			{
				if (spacePressedThisFrame)
				{
					Jump();
					onGround = false;
				}
			}

			if (data.KeysReleasedThisFrame.Contains(Keys.Space))
			{
				LimitJump();
			}
		}

		public void HandleMouse(MouseData data)
		{
			HandleGrapple(data);
		}

		private void Jump()
		{
			Vector2 velocity = PhysicsConvert.ToPixels(Body.LinearVelocity);
			velocity.Y -= JumpSpeedInitial;
			velocity.Y = velocity.Y < -JumpSpeedInitial ? -JumpSpeedInitial : velocity.Y;
			Body.LinearVelocity = PhysicsConvert.ToMeters(velocity);
		}

		private void LimitJump()
		{
			Vector2 velocity = PhysicsConvert.ToPixels(Body.LinearVelocity);

			if (velocity.Y < -JumpSpeedLimited)
			{
				velocity.Y = -JumpSpeedLimited;
				Body.LinearVelocity = PhysicsConvert.ToMeters(velocity);
			}

			Body.LinearVelocity = PhysicsConvert.ToMeters(velocity);
		}

		private void HandleGrapple(MouseData data)
		{
			if (grapple == null)
			{
				if (data.LeftClickState == ClickStates.PressedThisFrame)
				{
					FireGrapple(data.WorldPosition);
				}
			}
			else if (data.LeftClickState == ClickStates.ReleasedThisFrame)
			{
				DetachFromRope();
			}
		}

		private void FireGrapple(Vector2 target)
		{
			RayCastResults results = PhysicsUtilities.RayCast(Position, target, GrappleProximityLimit);

			if (results?.Entity is Edge && Vector2.Distance(Position, PhysicsConvert.ToPixels(results.Position)) < GrappleProximityLimit)
			{
				return;
			}
			
			Vector2 grappleVelocity = Vector2.Normalize(target - Position) * GrappleFireSpeed;
			grapple = new GrapplingHook(Position, this);
			grapple.Fire(grappleVelocity);

			EntityUtilities.AddEntity(grapple);
		}

		private void DetachFromRope()
		{
			attachedToRope = false;
			Body.Enabled = true;

			grapple.Release();
			grapple = null;

			if (rope != null)
			{
				rope.RegisterPlayerDetach();
				rope = null;
			}
		}

		public void RegisterGrappleImpact(Rope rope)
		{
			this.rope = rope;

			attachedToRope = true;
		}

		public override void Destroy()
		{
		}

		public override void Update(float dt)
		{
			if (attachedToRope)
			{
				UpdateClimbing(dt);
			}
			else
			{
				UpdateRunning(dt);
			}

			Vector2 convertedPosition = PhysicsConvert.ToPixels(Body.Position);
			Position = new Vector2((int)convertedPosition.X, (int)convertedPosition.Y);
			sprite.Position = Position;
		}

		private void UpdateClimbing(float dt)
		{
			// Like running, these values are mutually exclusive.
			if (climbingUp || climbingDown)
			{
				float acceleration = climbingUp ? -ClimbAccelerationUp : ClimbAccelerationDown;
				climbSpeed += acceleration * dt;
				climbSpeed = MathHelper.Clamp(climbSpeed, -ClimbMaxSpeedUp, ClimbMaxSpeedDown);
			}
			else if (climbSpeed != 0)
			{
				float deceleration = climbSpeed < 0 ? ClimbDecelerationUp : ClimbDecelerationDown;
				int previousSign = Math.Sign(climbSpeed);
				climbSpeed -= deceleration * previousSign * dt;

				if (Math.Sign(climbSpeed) != previousSign)
				{
					climbSpeed = 0;
				}
			}
			
			Body.Position = PhysicsConvert.ToMeters(rope.Climb(ref climbSpeed, dt));
		}

		private void UpdateRunning(float dt)
		{
			Vector2 velocity = PhysicsConvert.ToPixels(Body.LinearVelocity);

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

			Body.LinearVelocity = PhysicsConvert.ToMeters(velocity);
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);

			if (grapple != null && !attachedToRope)
			{
				RenderDummyRope(sb);
			}
		}

		private void RenderDummyRope(SpriteBatch sb)
		{
			float length = Vector2.Distance(Position, grapple.BackPosition);
			float rotation = GameFunctions.ComputeAngle(Position, grapple.BackPosition);

			Rectangle sourceRect = new Rectangle(0, 0, (int)length, ropeShortTexture.Height);

			sb.Draw(ropeShortTexture, Position, sourceRect, Color.White, rotation, Vector2.Zero, 1, SpriteEffects.None, 0);
		}
	}
}
