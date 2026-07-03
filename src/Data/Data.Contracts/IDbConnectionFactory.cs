using System.Data;

namespace SanskritQuest.Data.Contracts
{
	public interface IDbConnectionFactory
	{
		IDbConnection GetDefaultDbConnection();

		IDbConnection GetPocDbConnection();
	}
}
