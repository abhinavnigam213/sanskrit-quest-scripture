using System;

namespace SanskritQuest.Common.Contracts
{
	[AttributeUsage(AttributeTargets.Field)]
	public class DatabaseIdAttribute : Attribute
	{
		public int Id { get; }

		public DatabaseIdAttribute(int id)
		{
			Id = id;
		}
	}
}
