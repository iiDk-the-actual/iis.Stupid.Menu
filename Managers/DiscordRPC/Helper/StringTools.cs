using System;
using System.Linq;
using System.Text;

namespace iiMenu.Managers.DiscordRPC.Helper
{
	public static class StringTools
	{
		public static string GetNullOrString(this string str)
		{
			if (str.Length != 0 && !string.IsNullOrEmpty(str.Trim()))
			{
				return str;
			}
			return null;
		}

		public static bool WithinLength(this string str, int bytes)
		{
			return str.WithinLength(bytes, Encoding.UTF8);
		}

		public static bool WithinLength(this string str, int bytes, Encoding encoding)
		{
			return encoding.GetByteCount(str) <= bytes;
		}

		public static string ToCamelCase(this string str)
		{
			if (str == null)
			{
				return null;
			}
			return (from s in str.ToLowerInvariant().Split(new string[]
			{
				"_",
				" "
			}, StringSplitOptions.RemoveEmptyEntries)
			select char.ToUpper(s[0]).ToString() + s.Substring(1, s.Length - 1)).Aggregate(string.Empty, (s1, s2) => s1 + s2);
		}

		public static string ToSnakeCase(this string str)
		{
			if (str == null)
			{
				return null;
			}
			return string.Concat(str.Select(delegate(char x, int i)
			{
				if (i <= 0 || !char.IsUpper(x))
				{
					return x.ToString();
				}
				return "_" + x.ToString();
			}).ToArray()).ToUpperInvariant();
		}
	}
}
