﻿using FarseerPhysics.Dynamics;

namespace LD36.Physics
{
	internal static class PhysicsUtilities
	{
		private static World world = DIKernel.Get<World>();

		public static void RemoveBody(Body body)
		{
			world.RemoveBody(body);
		}
	}
}
