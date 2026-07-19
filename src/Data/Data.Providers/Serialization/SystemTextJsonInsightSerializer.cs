using System;
using System.Data;
using System.Text.Json;
using Insight.Database;
using Insight.Database.Serialization;

namespace SanskritQuest.Data.Providers.Serialization
{
	public class SystemTextJsonInsightSerializer : DbObjectSerializer
	{
		public override bool CanSerialize(Type type, DbType dbType)
		{
			return true;
		}

		public override bool CanDeserialize(Type sourceType, Type targetType)
		{
			return true;
		}

		public override object? SerializeObject(Type type, object? value)
		{
			if (value == null) return null;
			return JsonSerializer.Serialize(value);
		}

		public override object? DeserializeObject(Type type, object? value)
		{
			if (value == null || value == DBNull.Value) return null;
			if (value is string str)
			{
				if (string.IsNullOrWhiteSpace(str)) return null;
				return JsonSerializer.Deserialize(str, type);
			}
			return value;
		}
	}
}
