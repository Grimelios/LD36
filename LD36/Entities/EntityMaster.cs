using System;
using System.Collections.Generic;
using LD36.Json;
using Newtonsoft.Json;

namespace LD36.Entities
{
	internal class EntityMaster
	{
		private const string EntityPath = "LD36.Entities.";

		public EntityMaster(List<Type> masterList, List<Type> updateOrder, List<Type> renderOrder)
		{
			MasterList = masterList;
			UpdateOrder = updateOrder;
			RenderOrder = renderOrder;
		}

		[JsonConverter(typeof(TypeListConverter))]
		public List<Type> MasterList { get; }

		[JsonConverter(typeof(TypeListConverter))]
		public List<Type> UpdateOrder { get; }

		[JsonConverter(typeof(TypeListConverter))]
		public List<Type> RenderOrder { get; }

		private class TypeListConverter : JsonListConverter<Type>
		{
			public override Type ConvertItem(JsonSerializer serializer, string value, string name)
			{
				return Type.GetType(EntityPath + value);
			}
		}
	}
}
