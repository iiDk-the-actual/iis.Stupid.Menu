using System;
using System.Linq;
using System.Reflection;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC.Converters
{
	internal class EnumSnakeCaseConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType.IsEnum;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null)
			{
				return null;
			}
			object result = null;
			if (TryParseEnum(objectType, (string)reader.Value, out result))
			{
				return result;
			}
			return existingValue;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Type type = value.GetType();
			string value2 = Enum.GetName(type, value);
			foreach (MemberInfo memberInfo in type.GetMembers(BindingFlags.Static | BindingFlags.Public))
			{
				if (memberInfo.Name.Equals(value2))
				{
					object[] customAttributes = memberInfo.GetCustomAttributes(typeof(EnumValueAttribute), true);
					if (customAttributes.Length != 0)
					{
						value2 = ((EnumValueAttribute)customAttributes[0]).Value;
					}
				}
			}
			writer.WriteValue(value2);
		}

		public bool TryParseEnum(Type enumType, string str, out object obj)
		{
			if (str == null)
			{
				obj = null;
				return false;
			}
			Type type = enumType;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = type.GetGenericArguments().First();
			}
			if (!type.IsEnum)
			{
				obj = null;
				return false;
			}
			foreach (MemberInfo memberInfo in type.GetMembers(BindingFlags.Static | BindingFlags.Public))
			{
				foreach (EnumValueAttribute enumValueAttribute in memberInfo.GetCustomAttributes(typeof(EnumValueAttribute), true))
				{
					if (str.Equals(enumValueAttribute.Value))
					{
						obj = Enum.Parse(type, memberInfo.Name, true);
						return true;
					}
				}
			}
			obj = null;
			return false;
		}
	}
}
