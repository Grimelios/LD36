using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using LD36.Entities;
using Microsoft.Xna.Framework;

namespace LD36.Physics
{
	internal static class PhysicsUtilities
	{
		private static World world = DIKernel.Get<World>();

		public static void RemoveBody(Body body)
		{
			world.RemoveBody(body);
		}

		public static void RemoveJoint(Joint joint)
		{
			world.RemoveJoint(joint);
		}

		public static RayCastResults RayCast(Vector2 source, Vector2 target, float range)
		{
			source = PhysicsConvert.ToMeters(source);
			target = PhysicsConvert.ToMeters(target);

			Fixture closestFixture = null;
			Vector2 hitPosition = Vector2.Zero;
			Vector2 end = source + Vector2.Normalize(target - source) * range;

			float closestFraction = 1;

			world.RayCast((f, p, n, fr) =>
			{
				if (fr < closestFraction || closestFixture == null)
				{
					closestFraction = fr;
					closestFixture = f;
					hitPosition = p;
				}

				return 1;
			}, source, end);

			return closestFixture == null ? null : new RayCastResults(hitPosition, closestFixture.UserData);
		}
	}
}
