using System.Data;
using Npgsql;
using SanskritQuest.Common.Configuration;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Data.Providers
{
	public class DbConnectionFactory(ConnectionStrings connectionStrings) : IDbConnectionFactory
	{
		public IDbConnection GetDefaultDbConnection()
		{
			return _getDbConnection(connectionStrings.Default);
		}

		public IDbConnection GetPocDbConnection()
		{
			return _getDbConnection(connectionStrings.PocDatabase);
		}

		private IDbConnection _getDbConnection(string connectionString)
		{
			var conn = new NpgsqlConnection(connectionString);
			return conn;
		}
	}
}
