using SharpMap.Client;
using SharpMap.Server;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace SharpMap
{
	public class SharpMapModSystem : ModSystem
	{
		ServerProvider? _serverProvider;
		ClientProvider? _clientProvider;

		public override void Start(ICoreAPI api)
		{
			ApiModHelper.Mod = Mod;

			if (api is ICoreClientAPI capi)
			{
				ApiModHelper.CApi = capi;
			}
			else if (api is ICoreServerAPI sapi)
			{
				ApiModHelper.SApi = sapi;
			}
			api.Network.RegisterChannel(Mod.Info.ModID)
				.RegisterMessageType<Init>()
				.RegisterMessageType<LayerUpdate>();
		}

		public override void StartServerSide(ICoreServerAPI api)
		{
			var db = new ServerDB(GetDBPath());
			_serverProvider = new(db);

			ApiModHelper.GetServerChannel()?
				.SetMessageHandler<LayerUpdate>(_serverProvider.Process);

			api.Event.PlayerJoin += PlayerJoin;
		}

		void PlayerJoin(IServerPlayer player)
		{
			ApiModHelper.GetServerChannel()?.SendPacket(new Init(), player);
		}

		public override void StartClientSide(ICoreClientAPI api)
		{
			var db = new ClientDB(GetDBPath());
			_clientProvider = new(db);

			ApiModHelper.GetClientChannel()?
				.SetMessageHandler<Init>(_clientProvider.Init)
				.SetMessageHandler<LayerData>(_clientProvider.Process);
		}

		public override void AssetsFinalize(ICoreAPI api)
		{
			if (api.Side == EnumAppSide.Server || ApiModHelper.GetClientChannel() != null)
			{
				return;
			}
			_clientProvider?.Init();
		}

		static string GetDBPath()
		{
			string dir = Path.Combine(GamePaths.DataPath, "ModData", ApiModHelper.SaveGameId);
			GamePaths.EnsurePathExists(dir);

			return Path.Combine(dir, "customRegions.db");
		}
	}
}