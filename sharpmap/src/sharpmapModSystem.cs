using ProtoBuf;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace SharpMap
{
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class Init
	{

	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class LayerData
	{

	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class LayerUpdate
	{
		public int LayerId {  get; set; }
		public int RegionId { get; set; }
	}

	public class ServerProvider
	{
		public void Process(IServerPlayer player, LayerUpdate update)
		{

		}
	}

	public interface IClientProvider
	{
		void Process(LayerData data);
	}

	public class HeadlessClientProvider : IClientProvider
	{
		public void Process(LayerData data)
		{

		}
	}

	public class ClientProvider : IClientProvider
	{
		public void Process(LayerData data)
		{

		}
	}

	public class LayerManager
	{

	}

	public class SharpMapModSystem : ModSystem
	{
		ServerProvider? _serverProvider;
		IClientProvider? _clientProvider;

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
			_serverProvider = new ServerProvider();

			ApiModHelper.GetServerChannel()
				.SetMessageHandler<LayerUpdate>(_serverProvider.Process);

			api.Event.PlayerJoin += PlayerJoin;
		}

		void PlayerJoin(IServerPlayer player)
		{
			ApiModHelper.GetServerChannel().SendPacket(new Init(), player);
		}

		public override void StartClientSide(ICoreClientAPI api)
		{
			ApiModHelper.GetClientChannel()
				.SetMessageHandler<Init>(data =>
				{
					_clientProvider = new ClientProvider();
				})
				.SetMessageHandler<LayerData>(_clientProvider!.Process);
		}

		public override void AssetsFinalize(ICoreAPI api)
		{
			if (api.Side == EnumAppSide.Server || ApiModHelper.GetClientChannel() != null)
			{
				return;
			}
			_clientProvider = new HeadlessClientProvider();
		}
	}
}