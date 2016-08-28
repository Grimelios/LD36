using System;
using System.Collections.Generic;

namespace LD36.Entities
{
	using EntityLayers = Dictionary<Type, List<Entity>>;

	internal static class EntityUtilities
	{
		private static EntityLayers entityLayers;

		public static void Initialize(EntityLayers layers)
		{
			entityLayers = layers;
		}

		public static void AddEntity<T>(T entity) where T : Entity
		{
			entityLayers[typeof(T)].Add(entity);
		}

		public static void AddEntity(Entity entity, Type type)
		{
			entityLayers[type].Add(entity);
		}

		public static void RemoveEntity<T>(T entity) where T : Entity
		{
			entityLayers[typeof(T)].Remove(entity);
		}
	}
}
