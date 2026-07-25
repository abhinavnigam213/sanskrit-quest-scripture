using System;
using System.Reflection;
using SanskritQuest.Common.Contracts;

namespace SanskritQuest.Common.Utilities
{
	public static class EnumExtensions
	{
		/// <summary>
		/// Gets the Database ID associated with the enum value via the DatabaseIdAttribute.
		/// </summary>
		public static int? GetDatabaseId(this Enum enumValue)
		{
			if (enumValue == null)
				return null;

			Type type = enumValue.GetType();
			string? name = Enum.GetName(type, enumValue);
			if (name == null)
				return null;

			FieldInfo? field = type.GetField(name);
			if (field == null)
				return null;

			var attribute = field.GetCustomAttribute<DatabaseIdAttribute>();
			return attribute?.Id;
		}

		/// <summary>
		/// Finds the enum value of type TEnum that is decorated with the specified Database ID.
		/// </summary>
		public static TEnum? GetEnumByDatabaseId<TEnum>(this int id) where TEnum : struct, Enum
		{
			foreach (TEnum value in Enum.GetValues<TEnum>())
			{
				if (value.GetDatabaseId() == id)
				{
					return value;
				}
			}
			return null;
		}
	}
}
