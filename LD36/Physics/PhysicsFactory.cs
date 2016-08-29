using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using LD36.Entities;
using Microsoft.Xna.Framework;

namespace LD36.Physics
{
	internal enum Units
	{
		Meters,
		Pixels
	}

	internal class PhysicsFactory
	{
		private World world;

		public PhysicsFactory(World world)
		{
			this.world = world;
		}

		public Body CreateBody(Entity userData)
		{
			return BodyFactory.CreateBody(world, userData);
		}

		public Body CreateRectangle(float width, float height, Vector2 position, Units units, Entity userData)
		{
			if (units == Units.Pixels)
			{
				width = PhysicsConvert.ToMeters(width);
				height = PhysicsConvert.ToMeters(height);
				position = PhysicsConvert.ToMeters(position);
			}

			Body body = BodyFactory.CreateRectangle(world, width, height, 1, position, userData);
			body.BodyType = BodyType.Dynamic;

			return body;
		}

		public void AttachEdge(Body body, Edge edge, Units units)
		{
			Vector2 start = units == Units.Pixels ? PhysicsConvert.ToMeters(edge.Start) : edge.Start;
			Vector2 end = units == Units.Pixels ? PhysicsConvert.ToMeters(edge.End) : edge.End;

			FixtureFactory.AttachEdge(start, end, body, edge);
		}

		public RevoluteJoint CreateRevoluteJoint(Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB, Units units)
		{
			if (units == Units.Pixels)
			{
				anchorA = PhysicsConvert.ToMeters(anchorA);
				anchorB = PhysicsConvert.ToMeters(anchorB);
			}

			return JointFactory.CreateRevoluteJoint(world, bodyA, bodyB, anchorA, anchorB);
		}
	}
}
