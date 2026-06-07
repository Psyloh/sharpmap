using Microsoft.Data.Sqlite;
using Vintagestory.API.Common;

namespace SharpMap.Client
{
	public class ClientDB : SQLiteDBConnection
	{
		public ClientDB(string path) : base(ApiModHelper.Mod.Logger)
		{
			var errorMsg = "";
			if (!OpenOrCreate(path, ref errorMsg, true, true, false))
			{
				ApiModHelper.Error(errorMsg);
			}
		}

		public override void OnOpened()
		{

		}

		protected override void CreateTablesIfNotExists(SqliteConnection sqliteConn)
		{
			
		}
	}
}