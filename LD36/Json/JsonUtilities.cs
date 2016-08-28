using System;
using System.IO;
using Newtonsoft.Json;

namespace LD36.Json
{
	internal static class JsonUtilities
	{
		private static readonly string JsonPath = Environment.CurrentDirectory + "/Content/Json/";

		public static T Deserialize<T>(string filename)
		{
			return JsonConvert.DeserializeObject<T>(File.ReadAllText(JsonPath + filename));
		}

		public static object Deserialize(string json, JsonSerializer serializer, Type type)
		{
			return serializer.Deserialize(new JsonTextReader(new StringReader(json)), type);
		}
	}
}
