using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LD36.Json
{
	internal abstract class JsonListConverter<T> : JsonConverter
	{
		public abstract T ConvertItem(JsonSerializer serializer, string name, string value);

		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			List<T> list = new List<T>();
			JArray array = serializer.Deserialize<JArray>(reader);

			foreach (JToken token in array.Children())
			{
				string name = null;
				string value = null;

				if (token.Type == JTokenType.String)
				{
					JValue jValue = (JValue)token;
					value = jValue.Value.ToString();
				}
				else
				{
					JObject jObject = (JObject)token;
					JProperty jProperty = jObject.Properties().ToList()[0];
					name = jProperty.Name;
					value = jProperty.Value.ToString();
				}

				list.Add(ConvertItem(serializer, value, name));
			}

			return list;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
