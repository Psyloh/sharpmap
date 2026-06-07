using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Threading;
using Vintagestory.API.Common;

namespace SharpMap.Server
{
	public interface IDBUpdate
	{

	}

	public class ServerDB : SQLiteDBConnection
	{
		readonly ConcurrentQueue<IDBUpdate> _changes = [];

		Thread? _changeReader;
		readonly ManualResetEventSlim _sync = new(false);
		readonly CancellationTokenSource _tokenSource = new();

		public ServerDB(string path) : base(ApiModHelper.Mod.Logger)
		{
			var errorMsg = "";
			if (!OpenOrCreate(path, ref errorMsg, true, true, false))
			{
				ApiModHelper.Error(errorMsg);
			}
		}

		//automatically called on open
		public override void OnOpened()
		{
			_changeReader = new(ProcessQueue)
			{
				IsBackground = true,
				Name = "serverDB"
			};

			//connection setup like enabling foreign keys
			//read the inherited class code : some optimizations are already enabled from open flags
		}

		//automatically called on open
		protected override void CreateTablesIfNotExists(SqliteConnection sqliteConn)
		{
			
		}

		public void PushChange(IDBUpdate change)
		{
			ObjectDisposedException.ThrowIf(_tokenSource.IsCancellationRequested, this);

			_changes.Enqueue(change);
			_sync.Set();
		}

		void ProcessQueue()
		{
			while (!_tokenSource.IsCancellationRequested)
			{
				while(_changes.TryDequeue(out var change))
				{
					//pattern match if necessary and update DB
				}

				try
				{
					_sync.Reset();
					_sync.Wait(_tokenSource.Token);
				}
				catch (OperationCanceledException) { }
			}
		}

		public override void Dispose()
		{
			_tokenSource.Cancel();
			_changeReader?.Join();

			_sync.Dispose();
			_tokenSource.Dispose();
			base.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}