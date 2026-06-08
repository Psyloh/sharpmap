using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace SharpMap.Server
{
	public interface IDBUpdate
	{

	}

	public class ServerDB : SQLiteDBConnection
	{
		readonly ConcurrentQueue<IDBUpdate> _changes = [];

		Task? _queueReader;
		readonly SemaphoreSlim _lock = new(0, 1);
		readonly CancellationTokenSource _cancellation = new();

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
			_queueReader = Task.Run(ProcessQueue);

			//connection setup like enabling foreign keys
			//read the inherited class code : some optimizations are already enabled from open flags
		}

		//automatically called on open
		protected override void CreateTablesIfNotExists(SqliteConnection sqliteConn)
		{
			
		}

		public void PushChange(IDBUpdate change)
		{
			ObjectDisposedException.ThrowIf(_cancellation.IsCancellationRequested, this);

			_changes.Enqueue(change);
			_lock.Release();
		}

		async void ProcessQueue()
		{
			while (!_cancellation.IsCancellationRequested)
			{
				while(_changes.TryDequeue(out var change))
				{
					//pattern match if necessary and update DB
				}

				try
				{
					await _lock.WaitAsync(_cancellation.Token);
				}
				catch (OperationCanceledException) { }
			}
		}

		public override void Dispose()
		{
			_cancellation.Cancel();
			_queueReader?.Wait();

			_lock.Dispose();
			_cancellation.Dispose();
			base.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}